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
    public class GroupCommentController : ControllerBase
    {
        private readonly AppDbContext _context;

        public GroupCommentController(AppDbContext context)
        {
            _context = context;
        }

        // Get comments for a post in a group
        [HttpGet("post/{postId}")]
        public async Task<IActionResult> GetCommentsForPost(int postId)
        {
            var comments = await _context.GroupComments
                .Where(c => c.PostId == postId)
                .Include(c => c.User) // Include user who created the comment
                .ToListAsync();

            return Ok(comments);
        }

        // Create a new comment for a post
        [HttpPost]
        public async Task<IActionResult> CreateGroupComment([FromBody] GroupComment newComment)
        {
            if (newComment == null || string.IsNullOrEmpty(newComment.Content))
            {
                return BadRequest("Comment content is required.");
            }

            _context.GroupComments.Add(newComment);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCommentsForPost), new { postId = newComment.PostId }, newComment);
        }
    }
}
