using Microsoft.AspNetCore.Mvc;
using Melbon_Car_Sale_Management_System.Models;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

namespace Melbon_Car_Sale_Management_System.Controllers
{
    public class AccountAuthController : Controller
    {
        private readonly UsersContext _context; //database
        private readonly IPasswordHasher<Register> _passwordHasher;  //for password hashing in the Post Registration Request

        public AccountAuthController(UsersContext context, IPasswordHasher<Register> passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        //Post request for the user
        [HttpPost]
        public async Task<IActionResult> Login(Login model)
        {
            if (ModelState.IsValid)
            {
                // Check if the user exists in the database
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email == model.Email);

                if (user != null)
                {
                    // Verify the hashed password
                    var result = _passwordHasher.VerifyHashedPassword(user, user.Password, model.Password);
                    if (result == PasswordVerificationResult.Success)
                    {
                        // Create claims and sign in the user
                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, user.Email)
                        };

                        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                        return RedirectToAction("Index", "Home");
                    }
                }

                // User not found or password incorrect, set an error message
                ViewData["ErrorMessage"] = "Invalid email or password.";
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //Navigate to the Registration Page
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        //Post request For Registration of the user
        [HttpPost]
        public async Task<IActionResult> Register(Register model) //in the argument we pass the model
        {
            if (ModelState.IsValid)
            {
                // Hash the password before saving
                model.Password = _passwordHasher.HashPassword(model, model.Password);
                model.ConfirmPassword = _passwordHasher.HashPassword(model, model.ConfirmPassword);

                // Add the new user to the database
                _context.Users.Add(model);
                await _context.SaveChangesAsync();

                // Redirect to  login page
                return RedirectToAction("Login");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "AccountAuth");
        }
    }
} 