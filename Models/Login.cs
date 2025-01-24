using System.ComponentModel.DataAnnotations;

namespace Melbon_Car_Sale_Management_System.Models
{
    public class Login
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress]
        public required string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public required string Password { get; set; }

    }
}
