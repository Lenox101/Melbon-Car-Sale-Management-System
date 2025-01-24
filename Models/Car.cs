using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Melbon_Car_Sale_Management_System.Models
{
    public class Car
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Make is required")]
        [Display(Name = "Car Make")]
        public string Make { get; set; }

        [Required(ErrorMessage = "Model is required")]
        [Display(Name = "Car Model")]
        public string Model { get; set; }

        [Required(ErrorMessage = "Year is required")]
        [Display(Name = "Year of Manufacture")]
        [Range(1900, 2100, ErrorMessage = "Please enter a valid year")]
        public int Year { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Display(Name = "Price (KES)")]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Current mileage is required")]
        [Display(Name = "Current Mileage (KM)")]
        [Range(0, double.MaxValue, ErrorMessage = "Mileage must be greater than 0")]
        public int Mileage { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Gearbox type is required")]
        [Display(Name = "Gearbox Type")]
        public string Gearbox { get; set; }

        [Required(ErrorMessage = "Fuel type is required")]
        [Display(Name = "Fuel Type")]
        public string FuelType { get; set; }

        [Required(ErrorMessage = "Fuel economy is required")]
        [Display(Name = "Average Fuel Economy (KM/L)")]
        [Range(0, 100, ErrorMessage = "Please enter a valid fuel economy value between 0 and 100 KM/L")]
        public double AverageFuelEconomy { get; set; }

        [Required(ErrorMessage = "Category is required")]
        public string Category { get; set; }

        [Required(ErrorMessage = "Engine size is required")]
        [Display(Name = "Engine Size (cc)")]
        [Range(100, 10000, ErrorMessage = "Please enter a valid engine size between 100cc and 10000cc")]
        public int EngineSize { get; set; }

        public List<CarImage> Images { get; set; } = new List<CarImage>();

        [Required(ErrorMessage = "Contact phone number is required")]
        [Phone(ErrorMessage = "Please enter a valid phone number")]
        [Display(Name = "Contact Phone Number")]
        public string ContactPhoneNumber { get; set; }

        [Required(ErrorMessage = "Contact email is required")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        [Display(Name = "Contact Email")]
        public string ContactEmail { get; set; }
    }

    public class CarImage
    {
        public int Id { get; set; }
        public int CarId { get; set; }
        public byte[] ImageData { get; set; }
        public Car Car { get; set; }
    }
} 