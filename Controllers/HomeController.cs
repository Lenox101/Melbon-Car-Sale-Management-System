using System.Diagnostics;
using Melbon_Car_Sale_Management_System.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Text.Json;
using System.Net.Http;
using System.Threading.Tasks;

namespace Melbon_Car_Sale_Management_System.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UsersContext _context;
        private readonly IHttpClientFactory _httpClientFactory;

        public HomeController(ILogger<HomeController> logger, UsersContext context, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _context = context;
            _httpClientFactory = httpClientFactory;
        }

        public IActionResult Index() //Populating the index page with information
        {
            var cars = _context.Cars.Include(c => c.Images).ToList();
            
            // Calculate category counts
            ViewBag.CategoryCounts = new Dictionary<string, int>
            {
                { "Sedan", cars.Count(c => c.Category == "Sedan") },
                { "Hatchback", cars.Count(c => c.Category == "Hatchback") },
                { "SUV", cars.Count(c => c.Category == "SUV") },
                { "Coupe", cars.Count(c => c.Category == "Coupe") },
                { "Pickup", cars.Count(c => c.Category == "Pickup") },
                { "Semi", cars.Count(c => c.Category == "Semi") },
                { "Bus", cars.Count(c => c.Category == "Bus") },
                { "Van", cars.Count(c => c.Category == "Van") }
            };

            ViewBag.Makes = cars.Select(c => c.Make)
                .Distinct()
                .OrderBy(m => m)
                .ToList();

            ViewBag.Cars = cars;
            return View();
        }

        [HttpGet]
        public IActionResult Search(string searchMake, string searchModel, string searchPriceRange)
        {
            try
            {
                // Get all cars with their images
                var query = _context.Cars.Include(c => c.Images).AsQueryable();

                // Apply make filter
                if (!string.IsNullOrEmpty(searchMake))
                {
                    query = query.Where(c => c.Make == searchMake);
                    // Get models for the selected make
                    ViewBag.Models = _context.Cars
                        .Where(c => c.Make == searchMake)
                        .Select(c => c.Model)
                        .Distinct()
                        .OrderBy(m => m)
                        .ToList();
                }
                else
                {
                    ViewBag.Models = new List<string>();
                }

                // Apply model filter
                if (!string.IsNullOrEmpty(searchModel))
                {
                    query = query.Where(c => c.Model == searchModel);
                }

                // Apply price range filter
                if (!string.IsNullOrEmpty(searchPriceRange))
                {
                    var priceRangeParts = searchPriceRange.Split('-');
                    if (priceRangeParts.Length == 2 &&
                        decimal.TryParse(priceRangeParts[0], out decimal minPrice) &&
                        decimal.TryParse(priceRangeParts[1], out decimal maxPrice))
                    {
                        // Get cars and filter by price in memory
                        var allCars = query.ToList();
                        var filteredCars = allCars.Where(c => c.Price >= minPrice && c.Price <= maxPrice).ToList();
                        
                        // Update ViewBag.Cars with filtered results
                        ViewBag.Cars = filteredCars;

                        // Set section title
                        string priceRangeText;
                        if (minPrice >= 5000000)
                        {
                            priceRangeText = "Above KES 5,000,000";
                        }
                        else
                        {
                            priceRangeText = $"KES {minPrice:N0} - {maxPrice:N0}";
                        }
                        ViewData["SectionTitle"] = $"Cars {priceRangeText}";

                        // Get makes for dropdown
                        ViewBag.Makes = _context.Cars
                            .Select(c => c.Make)
                            .Distinct()
                            .OrderBy(m => m)
                            .ToList();

                        // Calculate category counts
                        ViewBag.CategoryCounts = GetCategoryCounts();

                        return View("Index");
                    }
                }

                // If no filters or invalid price range, show all cars from the query
                var cars = query.ToList();
                ViewBag.Cars = cars;
                
                // Get makes for dropdown
                ViewBag.Makes = _context.Cars
                    .Select(c => c.Make)
                    .Distinct()
                    .OrderBy(m => m)
                    .ToList();

                ViewBag.CategoryCounts = GetCategoryCounts();

                return View("Index");
            }
            catch (Exception)
            {
                ViewData["SearchError"] = "An error occurred while searching. Please try again.";
                return View("Error");
            }
        }

        //Category Search API Endpoint
        [HttpGet]
        public IActionResult CategorySearch(string category)
        {
            try
            {
                var query = _context.Cars.Include(c => c.Images)
                    .Where(c => c.Category == category);

                var cars = query.ToList();
                ViewBag.Cars = cars;
                
                // Get makes for dropdown
                ViewBag.Makes = _context.Cars
                    .Select(c => c.Make)
                    .Distinct()
                    .OrderBy(m => m)
                    .ToList();

                // Calculate category counts
                ViewBag.CategoryCounts = GetCategoryCounts();

                // Set section title
                ViewData["SectionTitle"] = $"{category} Cars";

                return View("Index");
            }
            catch (Exception ex)
            {
                ViewData["SearchError"] = "An error occurred while searching. Please try again.";
                return RedirectToAction("Index");
            }
        }

        //Api endpoint to add a new car
        [HttpPost]
        public async Task<IActionResult> AddCar(Car car, List<IFormFile> ImageUpload)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                    {
                        _logger.LogError($"Validation error: {error.ErrorMessage}");
                    }
                    return View("Index", car);
                }

                if (ImageUpload == null || !ImageUpload.Any())
                {
                    ModelState.AddModelError("ImageUpload", "Please select at least one image");
                    return View("Index", car);
                }

                if (ImageUpload.Count > 7)
                {
                    ModelState.AddModelError("ImageUpload", "Maximum 7 images allowed");
                    return View("Index", car);
                }

                // Process images
                foreach (var imageFile in ImageUpload)
                {
                    if (imageFile.Length > 0)
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            await imageFile.CopyToAsync(memoryStream);
                            car.Images.Add(new CarImage
                            {
                                ImageData = memoryStream.ToArray()
                            });
                        }
                    }
                }

                _context.Cars.Add(car);
                await _context.SaveChangesAsync();
                
                TempData["Success"] = "Car added successfully!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in AddCar: {ex.Message}");
                ModelState.AddModelError("", "There was an error saving the car. Please try again.");
                return View("Index", car);
            }
        }

        //Navigate to About Page
        public IActionResult About()
        {
            return View();
        }

        //Navigate to Privacy Page
        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult ViewCars()
        {
            var cars = _context.Cars.ToList();
            return View(cars);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private Dictionary<string, int> GetCategoryCounts()
        {
            return new Dictionary<string, int>
            {
                { "Sedan", _context.Cars.Count(c => c.Category == "Sedan") },
                { "Hatchback", _context.Cars.Count(c => c.Category == "Hatchback") },
                { "SUV", _context.Cars.Count(c => c.Category == "SUV") },
                { "Coupe", _context.Cars.Count(c => c.Category == "Coupe") },
                { "Pickup", _context.Cars.Count(c => c.Category == "Pickup") },
                { "Semi", _context.Cars.Count(c => c.Category == "Semi") },
                { "Bus", _context.Cars.Count(c => c.Category == "Bus") },
                { "Van", _context.Cars.Count(c => c.Category == "Van") }
            };
        }

        // Add an endpoint to get models for a specific make
        [HttpGet]
        public IActionResult GetModels(string make)
        {
            if (string.IsNullOrEmpty(make))
                return Json(new List<string>());

            var models = _context.Cars
                .Where(c => c.Make == make)
                .Select(c => c.Model)
                .Distinct()
                .OrderBy(m => m)
                .ToList();

            return Json(models);
        }

        public async Task<IActionResult> Market()
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                var baseUrl = $"{Request.Scheme}://{Request.Host}";
                var response = await client.GetAsync($"{baseUrl}/api/CarSalesAPIs");
                
                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();

                    try
                    {
                        var apiResponse = JsonSerializer.Deserialize<CarApiResponse>(jsonString, new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });

                        if (apiResponse?.Data != null)
                        {
                            return View(apiResponse.Data);
                        }
                    }
                    catch (JsonException jsonEx)
                    {
                        _logger.LogError("JSON Deserialization error: {Error}", jsonEx.Message);
                        ViewBag.Error = "Error processing the market data";
                    }
                }
                
                ViewBag.Error = $"Unable to fetch market data. Status: {response.StatusCode}";
                return View(new List<MarketCarData>());
            }
            catch (Exception ex)
            {
                _logger.LogError("Market action error: {Error}", ex.Message);
                ViewBag.Error = "An error occurred while fetching market data";
                return View(new List<MarketCarData>());
            }
        }
    }
}
