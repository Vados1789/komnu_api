using api.Data;
using api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GroupPostController : ControllerBase
    {
        private readonly AppDbContext _context;

        public GroupPostController(AppDbContext context)
        {
            _context = context;
        }

        // Get posts for a group
        [HttpGet("group/{groupId}")]
        public async Task<IActionResult> GetGroupPosts(int groupId)
        {
            var posts = await _context.GroupPosts
                .Where(p => p.GroupId == groupId)
                .Include(p => p.User) // Include user who created the post
                .ToListAsync();

            return Ok(posts);
        }

        // Create a new post in a group
        [HttpPost]
        public async Task<IActionResult> CreateGroupPost([FromBody] GroupPost newPost)
        {
            if (newPost == null || string.IsNullOrEmpty(newPost.Content))
            {
                return BadRequest("Post content is required.");
            }

            _context.GroupPosts.Add(newPost);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetGroupPosts), new { groupId = newPost.GroupId }, newPost);
        }
    }
}
