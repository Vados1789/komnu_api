using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using api.Data;
using api.Models;
using api.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;

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

                // Convert to DTOs before sending
                var userDtos = users.ConvertAll(u => new UserDto
                {
                    UserId = u.UserId,
                    Username = u.Username,
                    Email = u.Email,
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
        public async Task<ActionResult<UserDto>> CreateUser([FromBody] CreateUserDto createUserDto)
        {
            Console.WriteLine("Hello world");
            // Log when the method is called and display received data
            Console.WriteLine($"[INFO] Received request to create user with username: {createUserDto.Username}");

            try
            {
                // Log ModelState validation check
                if (!ModelState.IsValid)
                {
                    Console.WriteLine("[WARNING] Model state is invalid. Returning BadRequest.");
                    return BadRequest(ModelState);
                }

                // Log details of the user being created
                Console.WriteLine($"[INFO] Creating new user: {createUserDto.Username}, Email: {createUserDto.Email}");

                var newUser = new User
                {
                    Username = createUserDto.Username,
                    Email = createUserDto.Email,
                    ProfilePicture = createUserDto.ProfilePicture,
                    Bio = createUserDto.Bio,
                    DateOfBirth = createUserDto.DateOfBirth ?? DateTime.MinValue // Handle null by providing a default value
                };

                // Log before saving to the database
                Console.WriteLine("[INFO] Adding new user to the database...");

                _context.Users.Add(newUser);
                await _context.SaveChangesAsync();

                // Log after successful save
                Console.WriteLine($"[SUCCESS] User '{newUser.Username}' created successfully with ID: {newUser.UserId}");

                // Convert to UserDto before returning
                var userDto = new UserDto
                {
                    UserId = newUser.UserId,
                    Username = newUser.Username,
                    Email = newUser.Email,
                    ProfilePicture = newUser.ProfilePicture,
                    Bio = newUser.Bio,
                    DateOfBirth = newUser.DateOfBirth
                };

                // Return the created user details
                return CreatedAtAction(nameof(GetUserById), new { id = newUser.UserId }, userDto);
            }
            catch (Exception ex)
            {
                // Log error details, including inner exception if any
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

                // Convert to DTO before returning
                var userDto = new UserDto
                {
                    UserId = user.UserId,
                    Username = user.Username,
                    Email = user.Email,
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
