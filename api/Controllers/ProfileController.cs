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

        [HttpPut("{id}/update")]
        public async Task<IActionResult> UpdateProfile(
            int id,
            [FromForm] IFormFile profilePicture,
            [FromForm] string username,
            [FromForm] string email,
            [FromForm] string bio,
            [FromForm] string dateOfBirth,
            [FromForm] string phoneNumber) // Include phoneNumber
        {
            _logger.LogInformation("Received profile update request for user ID {UserId}", id);

            if (id <= 0)
            {
                _logger.LogWarning("Invalid user ID {UserId} provided.", id);
                return BadRequest(new { message = "Invalid user ID." });
            }

            try
            {
                // Check if the user exists in the database
                var user = await _context.Users.FindAsync(id);
                if (user == null)
                {
                    _logger.LogWarning("User with ID {UserId} not found in the database.", id);
                    return NotFound(new { message = "User not found." });
                }

                // Update textual profile information
                user.Username = username ?? user.Username;
                user.Email = email ?? user.Email;
                user.Bio = bio ?? user.Bio;
                user.PhoneNumber = phoneNumber ?? user.PhoneNumber; // Update phoneNumber

                if (DateTime.TryParse(dateOfBirth, out var parsedDate))
                {
                    user.DateOfBirth = parsedDate;
                }

                // Handle profile picture upload if provided
                if (profilePicture != null && profilePicture.Length > 0)
                {
                    if (profilePicture.Length > MaxFileSize)
                    {
                        _logger.LogWarning("File size exceeded for user ID {UserId}. Maximum allowed size is 2 MB.", id);
                        return BadRequest(new { message = "File size exceeded. Maximum allowed is 2 MB." });
                    }

                    var extension = Path.GetExtension(profilePicture.FileName).ToLower();
                    if (!allowedExtensions.Contains(extension))
                    {
                        _logger.LogWarning("Unsupported file type {Extension} for user ID {UserId}.", extension, id);
                        return BadRequest(new { message = "Unsupported file type. Only .jpg, .jpeg, .png, and .gif are allowed." });
                    }

                    // Generate a unique file name for the profile picture
                    var fileName = $"{Guid.NewGuid()}{extension}";
                    var imagesFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
                    Directory.CreateDirectory(imagesFolder);
                    var filePath = Path.Combine(imagesFolder, fileName);

                    // Save the profile picture to the server
                    _logger.LogInformation("Saving profile picture for user ID {UserId} at {FilePath}.", id, filePath);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await profilePicture.CopyToAsync(stream);
                    }

                    // Update the user's profile picture URL in the database
                    user.ProfilePicture = $"/images/{fileName}";
                }

                _context.Entry(user).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                // Generate the full URL for the profile picture
                var baseUrl = $"{Request.Scheme}://{Request.Host}";
                var profilePictureUrl = !string.IsNullOrEmpty(user.ProfilePicture) ? $"{baseUrl}{user.ProfilePicture}" : null;

                _logger.LogInformation("Successfully updated profile for user ID {UserId}.", id);

                return Ok(new
                {
                    message = "Profile updated successfully.",
                    ProfilePictureUrl = profilePictureUrl,
                    user
                });
            }
            catch (IOException ioEx)
            {
                _logger.LogError(ioEx, "File system error while saving profile picture for user ID {UserId}.", id);
                return StatusCode(500, new { message = "File system error occurred while updating the profile picture." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating profile for user ID {UserId}.", id);
                return StatusCode(500, new { message = "An internal server error occurred while updating the profile." });
            }
        }
    }
}
