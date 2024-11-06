using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using api.Data;
using api.Models;
using System.Linq;
using System.Threading.Tasks;
using api.DTOs;

namespace api.Controllers
{
    [ApiController]
    [Route("api/post-reactions")]
    public class PostReactionController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PostReactionController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/post-reactions/{postId}
        [HttpGet("{postId}")]
        public async Task<ActionResult<object>> GetReactionsForPost(int postId)
        {
            var likeCount = await _context.PostReactions.CountAsync(r => r.PostId == postId && r.ReactionType == "like");
            var dislikeCount = await _context.PostReactions.CountAsync(r => r.PostId == postId && r.ReactionType == "dislike");

            return Ok(new { likeCount, dislikeCount });
        }

        // GET: api/post-reactions/{postId}/user/{userId}
        [HttpGet("{postId}/user/{userId}")]
        public async Task<ActionResult<object>> GetUserReaction(int postId, int userId)
        {
            var reaction = await _context.PostReactions
                                         .FirstOrDefaultAsync(r => r.PostId == postId && r.UserId == userId);

            if (reaction == null)
            {
                // Return an empty reaction if the user has not reacted
                return Ok(new { reactionType = (string)null });
            }

            return Ok(new { reactionType = reaction.ReactionType });
        }

        // POST: api/post-reactions
        [HttpPost]
        public async Task<IActionResult> AddOrUpdateReaction([FromBody] PostReactionDto reactionDto)
        {
            Console.WriteLine($"Received reaction for PostId: {reactionDto.PostId}, UserId: {reactionDto.UserId}, ReactionType: {reactionDto.ReactionType}");

            if (reactionDto.ReactionType != "like" && reactionDto.ReactionType != "dislike")
            {
                return BadRequest("Invalid reaction type.");
            }

            var existingReaction = await _context.PostReactions
                .FirstOrDefaultAsync(r => r.PostId == reactionDto.PostId && r.UserId == reactionDto.UserId);

            if (existingReaction != null)
            {
                if (existingReaction.ReactionType == reactionDto.ReactionType)
                {
                    _context.PostReactions.Remove(existingReaction);
                }
                else
                {
                    existingReaction.ReactionType = reactionDto.ReactionType;
                    _context.PostReactions.Update(existingReaction);
                }
            }
            else
            {
                var newReaction = new PostReaction
                {
                    PostId = reactionDto.PostId,
                    UserId = reactionDto.UserId,
                    ReactionType = reactionDto.ReactionType
                };
                _context.PostReactions.Add(newReaction);
            }

            await _context.SaveChangesAsync();

            var likeCount = await _context.PostReactions.CountAsync(r => r.PostId == reactionDto.PostId && r.ReactionType == "like");
            var dislikeCount = await _context.PostReactions.CountAsync(r => r.PostId == reactionDto.PostId && r.ReactionType == "dislike");

            return Ok(new { likeCount, dislikeCount });
        }
    }
}
