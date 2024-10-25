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
using Microsoft.Extensions.Logging;

namespace api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly EmailService _emailService;
        private readonly ILogger<LoginController> _logger;

        public LoginController(AppDbContext context, EmailService emailService, ILogger<LoginController> logger)
        {
            _context = context;
            _emailService = emailService;
            _logger = logger;
        }

        // Existing authentication endpoint
        [HttpPost("authenticate")]
        public async Task<ActionResult> Authenticate([FromBody] LoginRequestDto loginRequest)
        {
            try
            {
                _logger.LogInformation($"[INFO] Attempting to authenticate user: {loginRequest.Username}");

                var userLogin = await _context.Logins
                    .Include(l => l.User)
                    .FirstOrDefaultAsync(l => l.LoginValue == loginRequest.Username);

                if (userLogin == null)
                {
                    _logger.LogWarning($"[WARN] User not found: {loginRequest.Username}");
                    return Unauthorized("Invalid username or password.");
                }

                string hashedPassword = HashPassword(loginRequest.Password);
                if (userLogin.PasswordHash != hashedPassword)
                {
                    _logger.LogWarning($"[WARN] Invalid password for user: {loginRequest.Username}");
                    return Unauthorized("Invalid username or password.");
                }

                if (userLogin.IsTwoFaEnabled)
                {
                    var token = GenerateVerificationCode();
                    var expiresAt = AdjustToDenmarkTimeZone(DateTime.UtcNow.AddMinutes(5));

                    var twoFaToken = new TwoFaToken
                    {
                        UserId = userLogin.UserId,
                        Token = token,
                        ExpiresAt = expiresAt
                    };

                    _context.TwoFaTokens.Add(twoFaToken);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation($"[INFO] Sending 2FA token to email: {userLogin.User.Email}");

                    // Send token via email
                    await _emailService.SendEmailAsync(userLogin.User.Email, "Your 2FA Code", $"Your verification code is: {token}");

                    return Ok(new TwoFaResponseDto { Message = "2FA Required", RequiresTwoFa = true, UserId = userLogin.UserId });
                }

                return Ok(new { message = "Login successful", userId = userLogin.UserId });
            }
            catch (Exception ex)
            {
                _logger.LogError($"[ERROR] Authentication error: {ex.Message}");
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
                    _logger.LogWarning($"[WARN] Invalid or expired 2FA code for user ID: {verifyTwoFaDto.UserId}");
                    return Unauthorized("Invalid or expired 2FA code.");
                }

                // Remove the token after successful verification
                _context.TwoFaTokens.Remove(twoFaToken);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"[INFO] Successful 2FA verification for user ID: {verifyTwoFaDto.UserId}");
                return Ok(new { message = "2FA Verification successful", userId = verifyTwoFaDto.UserId });
            }
            catch (Exception ex)
            {
                _logger.LogError($"[ERROR] Two-Factor Authentication error: {ex.Message}");
                return StatusCode(500, "An internal server error occurred.");
            }
        }

        // New method for generating verification codes
        private string GenerateVerificationCode(int length = 6)
        {
            var random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        // Adjust time to Denmark's local time (CET/CEST)
        private DateTime AdjustToDenmarkTimeZone(DateTime utcTime)
        {
            var denmarkTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");
            return TimeZoneInfo.ConvertTimeFromUtc(utcTime, denmarkTimeZone);
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
