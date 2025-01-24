using Melbon_Car_Sale_Management_System.Models; //Where the database models are defined
using Microsoft.EntityFrameworkCore; //Library for interacting with the database
using Microsoft.AspNetCore.Identity; //A library for managing users, passwords, and roles
using Microsoft.AspNetCore.Authentication.Cookies; //A tool to manage logging in and logging out using cookies.

namespace Melbon_Car_Sale_Management_System
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args); //This is like setting up a workspace for your app. It gathers all the tools and settings you'll need.

            // Add services to the container.
            builder.Services.AddControllersWithViews(); //Adds support for handling web pages (HTML views)
            builder.Services.AddHttpClient(); // Adds services for the API

            builder.Services.AddDbContext<UsersContext>(options =>
            {
                options.UseSqlServer("Server=DESKTOP-B07VAF9;Database=MelbonCarsDB;Trusted_Connection=True;TrustServerCertificate=True");
            });//Setting up database connection

            // Register the password hasher
            builder.Services.AddScoped<IPasswordHasher<Register>, PasswordHasher<Register>>();

            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/AccountAuth/Login";
                    options.LogoutPath = "/AccountAuth/Logout";
                });
            //This lets your app handle logging in and out using cookies. Cookies are small files stored in the browser to keep users logged in.

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection(); // Ensures secure communication using HTTPS.
            app.UseStaticFiles(); //Allows the app to serve things like images, CSS, and JavaScript.

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=AccountAuth}/{action=Login}/{id?}");

            app.Run();
        }
    }
}
