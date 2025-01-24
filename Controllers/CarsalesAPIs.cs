using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Net.Security;
using System.Text.Json;
using System.Net.Http.Headers;

namespace Melbon_Car_Sale_Management_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarSalesAPIs(IHttpClientFactory httpClientFactory, ILogger<CarSalesAPIs> logger) : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
        private readonly ILogger<CarSalesAPIs> _logger = logger;
        private const string API_KEY = "0cfc0ddb99msh8039bf00ce5e085p193f0djsn93b114603afb";

        [HttpGet]
        public async Task<IActionResult> GetCarSales()
        {
            var client = _httpClientFactory.CreateClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri("https://car-api2.p.rapidapi.com/api/makes?direction=asc&sort=id"),
                Headers =
                {
                    { "x-rapidapi-key", API_KEY },
                    { "x-rapidapi-host", "car-api2.p.rapidapi.com" },
                },
            };

            try
            {
                var response = await client.SendAsync(request);
                string responseBody = await response.Content.ReadAsStringAsync();

                _logger.LogInformation("API Response Status: {StatusCode}", response.StatusCode);

                if (response.IsSuccessStatusCode)
                {
                    return Ok(responseBody);
                }

                return StatusCode((int)response.StatusCode, "Error fetching data from the API");
            }
            catch (HttpRequestException httpEx)
            {
                _logger.LogError("HTTP Request Error: {Error}", httpEx.Message);
                return StatusCode(500, $"Internal server error: {httpEx.Message}");
            }
        }
    }
}
