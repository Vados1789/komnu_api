using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using api.Data;
using api.Models;
using api.HubsAll;
using System.Linq;
using System.Threading.Tasks;
using api.DTOs;
using Microsoft.AspNetCore.SignalR;

namespace api.Controllers
{
    [ApiController]
    [Route("api/post-reactions")]
    public class PostReactionController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IHubContext<PostHub> _postHub;

        public PostReactionController(AppDbContext context, IHubContext<PostHub> postHub)
        {
            _context = context;
            _postHub = postHub;
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

            await _postHub.Clients.All.SendAsync("ReceiveReactionUpdate", new
            {
                postId = reactionDto.PostId,
                likeCount,
                dislikeCount
            });

            return Ok(new { likeCount, dislikeCount });
        }
    }
}
