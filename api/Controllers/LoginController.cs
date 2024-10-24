using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using api.Data;
using api.Models;
using System;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text;
using api.DTOs;

namespace api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly AppDbContext _context;

        public LoginController(AppDbContext context)
        {
            _context = context;
        }

        // POST: api/login/authenticate
        [HttpPost("authenticate")]
        public async Task<ActionResult> Authenticate([FromBody] LoginRequestDto loginRequest)
        {
            try
            {
                // Log received login request
                Console.WriteLine($"[INFO] Attempting to authenticate user: {loginRequest.Username}");

                // Check if the user exists
                var userLogin = await _context.Logins
                    .Include(l => l.User)
                    .FirstOrDefaultAsync(l => l.LoginValue == loginRequest.Username);

                if (userLogin == null)
                {
                    return Unauthorized("Invalid username or password.");
                }

                // Verify the password
                string hashedPassword = HashPassword(loginRequest.Password);
                if (userLogin.PasswordHash != hashedPassword)
                {
                    return Unauthorized("Invalid username or password.");
                }

                // Successful login
                return Ok(new { message = "Login successful", userId = userLogin.UserId });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Authentication error: {ex.Message}");
                return StatusCode(500, "An internal server error occurred.");
            }
        }

        private string HashPassword(string password)
        {
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
