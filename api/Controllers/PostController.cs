using Microsoft.AspNetCore.Mvc;
using api.Data;
using api.Models;
using System.Collections.Generic;
using System.Linq;
using System;

namespace api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PostsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PostsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("all")]
        public ActionResult<IEnumerable<Post>> GetAllPosts()
        {
            try
            {
                // Retrieve all posts from the database
                var posts = _context.Posts.ToList();

                // Check if there are any posts
                if (posts == null || posts.Count == 0)
                {
                    return NotFound("No posts found.");
                }

                // Return the posts as a JSON response
                return Ok(posts);
            }
            catch (Exception ex)
            {
                // Log the error (optional) and return a 500 status code
                Console.WriteLine($"Error fetching posts: {ex.Message}");
                return StatusCode(500, "An internal server error occurred while retrieving posts.");
            }
        }
    }
}
