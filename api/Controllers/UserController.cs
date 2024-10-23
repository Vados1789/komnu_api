using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using api.Data;
using api.Models;
using api.DTOs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly string[] allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };

        public UsersController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/users/all
        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetAllUsers()
        {
            try
            {
                var users = await _context.Users.ToListAsync();

                if (users == null || users.Count == 0)
                {
                    return NotFound("No users found.");
                }

                var userDtos = users.ConvertAll(u => new UserDto
                {
                    UserId = u.UserId,
                    Username = u.Username,
                    Email = u.Email,
                    PhoneNumber = u.PhoneNumber,
                    ProfilePicture = u.ProfilePicture,
                    Bio = u.Bio,
                    DateOfBirth = u.DateOfBirth
                });

                return Ok(userDtos);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching users: {ex.Message}");
                return StatusCode(500, "An internal server error occurred while retrieving users.");
            }
        }

        // POST: api/users
        [HttpPost]
        public async Task<ActionResult<UserDto>> CreateUser([FromForm] CreateUserDto createUserDto)
        {
            Console.WriteLine("Hello world");
            Console.WriteLine($"[INFO] Received request to create user with username: {createUserDto.Username}");

            try
            {
                if (!ModelState.IsValid)
                {
                    Console.WriteLine("[WARNING] Model state is invalid. Returning BadRequest.");
                    return BadRequest(ModelState);
                }

                string profilePicturePath = "";
                if (createUserDto.ProfilePicture != null)
                {
                    var extension = Path.GetExtension(createUserDto.ProfilePicture.FileName).ToLower();
                    if (!Array.Exists(allowedExtensions, ext => ext == extension))
                    {
                        return BadRequest("Unsupported file type. Only .jpg, .jpeg, .png, and .gif are allowed.");
                    }

                    var fileName = Path.GetRandomFileName() + extension;
                    var filePath = Path.Combine("images", fileName);

                    Directory.CreateDirectory("images"); // Ensure the directory exists
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await createUserDto.ProfilePicture.CopyToAsync(stream);
                    }

                    profilePicturePath = $"/images/{fileName}";
                }

                var newUser = new User
                {
                    Username = createUserDto.Username,
                    Email = createUserDto.Email,
                    PhoneNumber = createUserDto.PhoneNumber, // Add this line
                    ProfilePicture = profilePicturePath,
                    Bio = createUserDto.Bio,
                    DateOfBirth = createUserDto.DateOfBirth ?? DateTime.MinValue
                };

                Console.WriteLine("[INFO] Adding new user to the database...");

                _context.Users.Add(newUser);
                await _context.SaveChangesAsync();

                Console.WriteLine($"[SUCCESS] User '{newUser.Username}' created successfully with ID: {newUser.UserId}");

                var userDto = new UserDto
                {
                    UserId = newUser.UserId,
                    Username = newUser.Username,
                    Email = newUser.Email,
                    PhoneNumber = newUser.PhoneNumber, // Add this line
                    ProfilePicture = newUser.ProfilePicture,
                    Bio = newUser.Bio,
                    DateOfBirth = newUser.DateOfBirth
                };

                return CreatedAtAction(nameof(GetUserById), new { id = newUser.UserId }, userDto);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Error creating user: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"[ERROR] Inner Exception: {ex.InnerException.Message}");
                }
                return StatusCode(500, "An internal server error occurred while creating the user.");
            }
        }

        // GET: api/users/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetUserById(int id)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);

                if (user == null)
                {
                    return NotFound("User not found.");
                }

                var userDto = new UserDto
                {
                    UserId = user.UserId,
                    Username = user.Username,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    ProfilePicture = user.ProfilePicture,
                    Bio = user.Bio,
                    DateOfBirth = user.DateOfBirth
                };

                return Ok(userDto);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching user: {ex.Message}");
                return StatusCode(500, "An internal server error occurred while retrieving the user.");
            }
        }

        // PUT: api/users/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserDto updateUserDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var user = await _context.Users.FindAsync(id);

                if (user == null)
                {
                    return NotFound("User not found.");
                }

                user.Username = updateUserDto.Username ?? user.Username;
                user.Email = updateUserDto.Email ?? user.Email;
                user.PhoneNumber = updateUserDto.PhoneNumber ?? user.PhoneNumber;
                user.ProfilePicture = updateUserDto.ProfilePicture ?? user.ProfilePicture;
                user.Bio = updateUserDto.Bio ?? user.Bio;
                user.DateOfBirth = updateUserDto.DateOfBirth ?? user.DateOfBirth;

                _context.Entry(user).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating user: {ex.Message}");
                return StatusCode(500, "An internal server error occurred while updating the user.");
            }
        }

        // DELETE: api/users/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);

                if (user == null)
                {
                    return NotFound("User not found.");
                }

                _context.Users.Remove(user);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting user: {ex.Message}");
                return StatusCode(500, "An internal server error occurred while deleting the user.");
            }
        }
    }
}
