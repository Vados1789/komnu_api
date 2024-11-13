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
    public class GroupReactionController : ControllerBase
    {
        private readonly AppDbContext _context;

        public GroupReactionController(AppDbContext context)
        {
            _context = context;
        }

        // Get reactions for a post in a group
        [HttpGet("post/{postId}")]
        public async Task<IActionResult> GetReactionsForPost(int postId)
        {
            var reactions = await _context.GroupReactions
                .Where(r => r.PostId == postId)
                .Include(r => r.User) // Include user who reacted
                .ToListAsync();

            return Ok(reactions);
        }

        // Get reactions for a comment in a group
        [HttpGet("comment/{commentId}")]
        public async Task<IActionResult> GetReactionsForComment(int commentId)
        {
            var reactions = await _context.GroupReactions
                .Where(r => r.CommentId == commentId)
                .Include(r => r.User) // Include user who reacted
                .ToListAsync();

            return Ok(reactions);
        }

        // Create a new reaction for a post or comment
        [HttpPost]
        public async Task<IActionResult> CreateGroupReaction([FromBody] GroupReaction newReaction)
        {
            if (newReaction == null || !newReaction.ReactionType.Equals("like") && !newReaction.ReactionType.Equals("dislike"))
            {
                return BadRequest("Invalid reaction type.");
            }

            _context.GroupReactions.Add(newReaction);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetReactionsForPost), new { postId = newReaction.PostId }, newReaction);
        }
    }
}
