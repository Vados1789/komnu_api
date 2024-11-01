using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using api.Data;
using api.Models;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProfileController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ProfileController> _logger;
        private readonly string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif" };
        private const long MaxFileSize = 2 * 1024 * 1024; // 2 MB limit

        public ProfileController(AppDbContext context, ILogger<ProfileController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpPut("{id}/upload-picture")]
        public async Task<IActionResult> UploadProfilePicture(int id, [FromForm] IFormFile profilePicture)
        {
            System.Console.WriteLine($"bob, {profilePicture}");
            System.Console.WriteLine($"id, {id}");
            if (id <= 0)
            {
                _logger.LogWarning("Invalid user ID {UserId} provided for profile picture upload.", id);
                return BadRequest(new { message = "Invalid user ID." });
            }

            if (profilePicture == null || profilePicture.Length == 0)
            {
                _logger.LogWarning("No profile picture provided for user ID {UserId}.", id);
                return BadRequest(new { message = "No profile picture provided." });
            }

            if (profilePicture.Length > MaxFileSize)
            {
                _logger.LogWarning("File size exceeded for user ID {UserId}.", id);
                return BadRequest(new { message = "File size exceeded. Maximum allowed is 2 MB." });
            }

            _logger.LogInformation("Profile picture received for user ID {UserId} with file name {FileName}", id, profilePicture.FileName);

            try
            {
                var extension = Path.GetExtension(profilePicture.FileName).ToLower();
                if (!allowedExtensions.Contains(extension))
                {
                    _logger.LogWarning("Unsupported file type: {Extension}", extension);
                    return BadRequest(new { message = "Unsupported file type. Only .jpg, .jpeg, .png, and .gif are allowed." });
                }

                var fileName = $"{Guid.NewGuid()}{extension}";
                var imagesFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
                Directory.CreateDirectory(imagesFolder);
                var filePath = Path.Combine(imagesFolder, fileName);

                _logger.LogInformation("Saving profile picture to {FilePath}", filePath);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await profilePicture.CopyToAsync(stream);
                }

                var user = await _context.Users.FindAsync(id);
                if (user == null)
                {
                    _logger.LogWarning("User with ID {UserId} not found.", id);
                    return NotFound(new { message = "User not found." });
                }

                user.ProfilePicture = $"/images/{fileName}";
                _context.Entry(user).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                var baseUrl = $"{Request.Scheme}://{Request.Host}";
                var profilePictureUrl = $"{baseUrl}/images/{fileName}";

                return Ok(new { message = "Profile picture updated successfully.", ProfilePictureUrl = profilePictureUrl, user });
            }
            catch (IOException ioEx)
            {
                _logger.LogError(ioEx, "File system error while saving profile picture for user ID {UserId}", id);
                return StatusCode(500, new { message = "File system error occurred while updating the profile picture." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating profile picture for user ID {UserId}", id);
                return StatusCode(500, new { message = "An internal server error occurred while updating the profile picture." });
            }
        }
    }
}
