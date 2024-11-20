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

        // Get post details with comments
        [HttpGet("post/{postId}")]
        public async Task<IActionResult> GetPostWithComments(int postId)
        {
            // Fetch post details with user and comments
            var post = await _context.GroupPosts
                .Include(p => p.User) // Include User who created the post
                .Include(p => p.Group)
                .Include(p => p.Comments) // Include Comments for this post
                    .ThenInclude(c => c.User) // Include the User for each comment
                .Include(p => p.Comments) // Include Comments again to get Replies
                    .ThenInclude(c => c.Replies) // Include Replies for each comment
                .FirstOrDefaultAsync(p => p.PostId == postId);

            if (post == null)
                return NotFound("Post not found.");

            // Organize comments into a nested structure
            var nestedComments = post.Comments
                .Where(c => c.ParentCommentId == null)
                .Select(c => new
                {
                    c.CommentId,
                    c.Content,
                    c.CreatedAt,
                    c.User.Username,
                    Replies = c.Replies.Select(r => new
                    {
                        r.CommentId,
                        r.Content,
                        r.CreatedAt,
                        r.User.Username
                    })
                });

            // Return both post details and comments
            return Ok(new
            {
                Post = new
                {
                    post.PostId,
                    post.Content,
                    post.CreatedAt,
                    post.User.Username,
                    post.ImagePath
                },
                Comments = nestedComments
            });
        }
    }
}
