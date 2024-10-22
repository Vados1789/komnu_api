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
    public class EventsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public EventsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/events
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Event>>> GetAllEvents()
        {
            try
            {
                // Retrieve all events from the database asynchronously
                var events = await _context.Events.ToListAsync();

                // Check if there are any events
                if (events == null || events.Count == 0)
                {
                    return NoContent(); // Return 204 No Content if there are no events
                }

                // Return the events as a JSON response
                return Ok(events);
            }
            catch (Exception ex)
            {
                // Log the error (you can use a logging framework here)
                Console.WriteLine($"Error fetching events: {ex.Message}");
                return StatusCode(500, "An internal server error occurred while retrieving events.");
            }
        }
    }
}
