using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using api.Data;
using api.Models;
using api.DTOs;
using System;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text;

namespace api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public LoginsController(AppDbContext context)
        {
            _context = context;
        }

        // POST: api/logins
        [HttpPost]
        public async Task<ActionResult> CreateLogin([FromBody] CreateLoginDto createLoginDto)
        {
            try
            {
                // Log the data received from the frontend
                Console.WriteLine($"[INFO] Received CreateLoginDto:");
                Console.WriteLine($"- UserId: {createLoginDto.UserId}");
                Console.WriteLine($"- Password: {createLoginDto.Password}");
                Console.WriteLine($"- IsTwoFaEnabled: {createLoginDto.IsTwoFaEnabled}");
                Console.WriteLine($"- TwoFaMethod: {createLoginDto.TwoFaMethod}");

                if (createLoginDto == null)
                {
                    return BadRequest("Invalid login data.");
                }

                // Check if the user exists
                var user = await _context.Users.FindAsync(createLoginDto.UserId);
                if (user == null)
                {
                    return NotFound("User not found.");
                }

                // Hash the password
                string hashedPassword = HashPassword(createLoginDto.Password);

                // Create a new login entry
                var login = new Login
                {
                    UserId = createLoginDto.UserId,
                    PasswordHash = hashedPassword,
                    IsTwoFaEnabled = createLoginDto.IsTwoFaEnabled,
                    TwoFaMethod = createLoginDto.TwoFaMethod,
                    LoginType = "default",
                    LoginValue = user.Email, // or another identifier if needed
                    CreatedAt = DateTime.Now
                };

                _context.Logins.Add(login);
                await _context.SaveChangesAsync();

                return Ok("Login settings saved successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Error saving login settings: {ex.Message}");
                return StatusCode(500, "An internal server error occurred while saving login settings.");
            }
        }

        private string HashPassword(string password)
        {
            // Simple hash example using SHA256. Replace with more secure hash (like bcrypt) in production.
            using (var sha256 = SHA256.Create())
            {
                var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                var builder = new StringBuilder();
                foreach (var b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}
