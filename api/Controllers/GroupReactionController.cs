using api.Data;
using api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace api.Controllers
{
    [Route("api/GroupPostReactions")]
    [ApiController]
    public class GroupReactionController : ControllerBase
    {
        private readonly AppDbContext _context;

        public GroupReactionController(AppDbContext context)
        {
            _context = context;
        }

        // Get reactions for a post
        [HttpGet("post/{postId}")]
        public async Task<IActionResult> GetReactionsForPost(int postId)
        {
            try
            {
                Console.WriteLine($"Fetching reactions for postId: {postId}");

                var reactions = await _context.GroupReactions
                    .Where(r => r.PostId == postId)
                    .ToListAsync();

                Console.WriteLine($"Found {reactions.Count} reactions for postId: {postId}");

                var likeCount = reactions.Count(r => r.ReactionType == "like");
                var dislikeCount = reactions.Count(r => r.ReactionType == "dislike");

                return Ok(new
                {
                    LikeCount = likeCount,
                    DislikeCount = dislikeCount
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching reactions for post {postId}: {ex}");
                return StatusCode(500, "An error occurred while fetching reactions.");
            }
        }



        // Get user's reaction for a post
        [HttpGet("post/{postId}/user/{userId}")]
        public async Task<IActionResult> GetUserReactionForPost(int postId, int userId)
        {
            try
            {
                var userReaction = await _context.GroupReactions
                    .Where(r => r.PostId == postId && r.UserId == userId)
                    .FirstOrDefaultAsync();

                if (userReaction == null)
                {
                    return Ok(new { ReactionType = (string)null }); // No reaction yet
                }

                return Ok(new { ReactionType = userReaction.ReactionType });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching user reaction for post {postId} by user {userId}: {ex.Message}");
                return StatusCode(500, "An error occurred while fetching the user's reaction.");
            }
        }


        // Add or update a reaction
        [HttpPost]
        public async Task<IActionResult> AddOrUpdateReaction([FromBody] GroupReaction newReaction)
        {
            if (newReaction == null) return BadRequest("Invalid reaction.");

            var existingReaction = await _context.GroupReactions
                .FirstOrDefaultAsync(r => r.PostId == newReaction.PostId && r.UserId == newReaction.UserId);

            if (existingReaction != null)
            {
                if (existingReaction.ReactionType == newReaction.ReactionType)
                {
                    _context.GroupReactions.Remove(existingReaction);
                }
                else
                {
                    existingReaction.ReactionType = newReaction.ReactionType;
                }
            }
            else
            {
                _context.GroupReactions.Add(newReaction);
            }

            await _context.SaveChangesAsync();

            return Ok(new { Success = true });
        }
    }
}
