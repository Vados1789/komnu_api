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
    public class MessagesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public MessagesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/messages
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Message>>> GetAllMessages()
        {
            try
            {
                // Retrieve all messages from the database asynchronously
                var messages = await _context.Messages.ToListAsync();

                // Check if there are any messages
                if (messages == null || messages.Count == 0)
                {
                    return NoContent(); // Return 204 No Content if there are no messages
                }

                // Return the messages as a JSON response
                return Ok(messages);
            }
            catch (Exception ex)
            {
                // Log the error (you can use a logging framework here)
                Console.WriteLine($"Error fetching messages: {ex.Message}");
                return StatusCode(500, "An internal server error occurred while retrieving messages.");
            }
        }
    }
}
