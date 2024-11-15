using api.Data;
using api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace api.Controllers
{
    [Route("api/GroupPostComments")]
    [ApiController]
    public class GroupCommentController : ControllerBase
    {
        private readonly AppDbContext _context;

        public GroupCommentController(AppDbContext context)
        {
            _context = context;
        }

        // Add this method in GroupCommentController
        [HttpGet("{postId}/count")]
        public async Task<IActionResult> GetCommentCountForPost(int postId)
        {
            var count = await _context.GroupComments.CountAsync(c => c.PostId == postId);
            return Ok(count);
        }


        // Get comments for a post
        [HttpGet("{postId}")]
        public async Task<IActionResult> GetCommentsForPost(int postId)
        {
            var comments = await _context.GroupComments
                .Where(c => c.PostId == postId)
                .Include(c => c.User)
                .Include(c => c.ParentComment)
                .ToListAsync();

            var nestedComments = comments
                .Where(c => c.ParentCommentId == null)
                .Select(c => new
                {
                    c.CommentId,
                    c.Content,
                    c.CreatedAt,
                    c.User.Username,
                    Replies = comments
                        .Where(r => r.ParentCommentId == c.CommentId)
                        .Select(r => new
                        {
                            r.CommentId,
                            r.Content,
                            r.CreatedAt,
                            r.User.Username
                        })
                });

            return Ok(nestedComments);
        }

        // Create a new comment
        [HttpPost("add")]
        public async Task<IActionResult> AddComment([FromBody] GroupComment newComment)
        {
            if (newComment == null || string.IsNullOrEmpty(newComment.Content))
                return BadRequest("Comment content is required.");

            _context.GroupComments.Add(newComment);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCommentsForPost), new { postId = newComment.PostId }, newComment);
        }

        // Delete a comment
        [HttpDelete("delete/{commentId}")]
        public async Task<IActionResult> DeleteComment(int commentId)
        {
            var comment = await _context.GroupComments.FindAsync(commentId);
            if (comment == null) return NotFound("Comment not found.");

            _context.GroupComments.Remove(comment);
            await _context.SaveChangesAsync();

            return Ok(new { Success = true });
        }
    }
}
