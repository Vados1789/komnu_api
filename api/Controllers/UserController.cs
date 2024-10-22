using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using api.Data;
using api.Models;
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
        public async Task<ActionResult<IEnumerable<User>>> GetAllUsers()
        {
            try
            {
                // Retrieve all users from the database asynchronously
                var users = await _context.Users.ToListAsync();

                // Check if there are any users
                if (users == null || users.Count == 0)
                {
                    return NotFound("No users found.");
                }

                // Return the users as a JSON response
                return Ok(users);
            }
            catch (Exception ex)
            {
                // Log the error (you can use a logging framework here)
                Console.WriteLine($"Error fetching users: {ex.Message}");
                return StatusCode(500, "An internal server error occurred while retrieving users.");
            }
        }
    }
}
