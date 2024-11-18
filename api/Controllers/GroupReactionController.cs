using api.Data;
using api.DTOs;
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

                Console.WriteLine($"Returning JSON response: {{ LikeCount: {likeCount}, DislikeCount: {dislikeCount} }}");

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
        public async Task<IActionResult> AddOrUpdateReaction([FromBody] PostReactionDto reactionDto)
        {
            if (reactionDto == null || string.IsNullOrEmpty(reactionDto.ReactionType))
                return BadRequest("Invalid reaction.");

            // Check if an existing reaction from the user for the specific post already exists
            var existingReaction = await _context.GroupReactions
                .FirstOrDefaultAsync(r => r.PostId == reactionDto.PostId && r.UserId == reactionDto.UserId);

            if (existingReaction != null)
            {
                // If the reaction type is the same, remove the reaction (toggle off)
                if (existingReaction.ReactionType == reactionDto.ReactionType)
                {
                    _context.GroupReactions.Remove(existingReaction);
                }
                else
                {
                    // Otherwise, update the reaction type
                    existingReaction.ReactionType = reactionDto.ReactionType;
                }
            }
            else
            {
                // If no existing reaction, add a new one
                var newReaction = new GroupReaction
                {
                    PostId = reactionDto.PostId,
                    UserId = reactionDto.UserId,
                    ReactionType = reactionDto.ReactionType
                };
                _context.GroupReactions.Add(newReaction);
            }

            // Save changes
            await _context.SaveChangesAsync();

            // Fetch updated like and dislike counts
            var reactions = await _context.GroupReactions
                .Where(r => r.PostId == reactionDto.PostId)
                .ToListAsync();

            var likeCount = reactions.Count(r => r.ReactionType == "like");
            var dislikeCount = reactions.Count(r => r.ReactionType == "dislike");

            // Return updated counts
            return Ok(new
            {
                LikeCount = likeCount,
                DislikeCount = dislikeCount
            });
        }
    }
}
