using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using api.Data;
using api.DTOs;
using api.Models;
using api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly EmailService _emailService;

        public LoginController(AppDbContext context, EmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        // Existing authentication endpoint
        [HttpPost("authenticate")]
        public async Task<ActionResult> Authenticate([FromBody] LoginRequestDto loginRequest)
        {
            try
            {
                Console.WriteLine($"[INFO] Attempting to authenticate user: {loginRequest.Username}");

                var userLogin = await _context.Logins
                    .Include(l => l.User)
                    .FirstOrDefaultAsync(l => l.LoginValue == loginRequest.Username);

                if (userLogin == null)
                {
                    return Unauthorized("Invalid username or password.");
                }

                string hashedPassword = HashPassword(loginRequest.Password);
                if (userLogin.PasswordHash != hashedPassword)
                {
                    return Unauthorized("Invalid username or password.");
                }

                if (userLogin.IsTwoFaEnabled)
                {
                    var token = GenerateToken();
                    var expiresAt = DateTime.UtcNow.AddMinutes(5);

                    var twoFaToken = new TwoFaToken
                    {
                        UserId = userLogin.UserId,
                        Token = token,
                        ExpiresAt = expiresAt
                    };

                    _context.TwoFaTokens.Add(twoFaToken);
                    await _context.SaveChangesAsync();

                    // Send token via email
                    await _emailService.SendEmailAsync(userLogin.User.Email, "Your 2FA Code", $"Your verification code is: {token}");

                    return Ok(new { message = "2FA Required", requiresTwoFa = true, userId = userLogin.UserId });
                }

                return Ok(new { message = "Login successful", userId = userLogin.UserId });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Authentication error: {ex.Message}");
                return StatusCode(500, "An internal server error occurred.");
            }
        }

        // New endpoint to verify 2FA code
        [HttpPost("verify-2fa")]
        public async Task<ActionResult> VerifyTwoFa([FromBody] VerifyTwoFaDto verifyTwoFaDto)
        {
            try
            {
                var twoFaToken = await _context.TwoFaTokens
                    .FirstOrDefaultAsync(t => t.UserId == verifyTwoFaDto.UserId && t.Token == verifyTwoFaDto.Token && t.ExpiresAt > DateTime.UtcNow);

                if (twoFaToken == null)
                {
                    return Unauthorized("Invalid or expired 2FA code.");
                }

                // Remove the token after successful verification
                _context.TwoFaTokens.Remove(twoFaToken);
                await _context.SaveChangesAsync();

                return Ok(new { message = "2FA Verification successful", userId = verifyTwoFaDto.UserId });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Two-Factor Authentication error: {ex.Message}");
                return StatusCode(500, "An internal server error occurred.");
            }
        }

        private string GenerateToken()
        {
            using (var rng = new RNGCryptoServiceProvider())
            {
                var tokenData = new byte[4];
                rng.GetBytes(tokenData);
                int token = BitConverter.ToUInt16(tokenData, 0) % 1000000; // Generate 6-digit token
                return token.ToString("D6");
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
