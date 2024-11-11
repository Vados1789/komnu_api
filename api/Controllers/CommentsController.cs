using api.Data;
using api.DTOs;
using api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using api.HubsAll;
using Microsoft.AspNetCore.SignalR;

[ApiController]
[Route("api/[controller]")]
public class CommentsController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IHubContext<CommentHub> _commentHub;

    public CommentsController(AppDbContext context, IHubContext<CommentHub> commentHub)
    {
        _context = context;
        _commentHub = commentHub;
    }

    // GET: api/comments/post/{postId}
    [HttpGet("post/{postId}")]
    public async Task<IActionResult> GetPostById(int postId)
    {
        var post = await _context.Posts
            .Include(p => p.User) // Include user info if needed
            .FirstOrDefaultAsync(p => p.PostId == postId);

        if (post == null)
        {
            return NotFound();
        }

        var postDto = new PostDto // Map to a DTO if preferred
        {
            PostId = post.PostId,
            UserId = post.UserId,
            Content = post.Content,
            ImagePath = post.ImagePath,
            CreatedAt = post.CreatedAt,
            Username = post.User?.Username
        };

        return Ok(postDto);
    }

    // GET: api/comments/{postId}
    [HttpGet("{postId}")]
    public async Task<IActionResult> GetCommentsByPost(int postId)
    {
        var allComments = await _context.Comments
            .Where(c => c.PostId == postId)
            .Include(c => c.User) // Include user information
            .Include(c => c.Replies)
                .ThenInclude(r => r.User) // Include user information for replies
            .ToListAsync();

        var topLevelComments = allComments
            .Where(c => c.ParentCommentId == null)
            .Select(c => MapCommentWithReplies(c, allComments))
            .ToList();

        return Ok(topLevelComments);
    }

    // Recursive function to map comments with their nested replies
    private CommentDto MapCommentWithReplies(Comment comment, List<Comment> allComments)
    {
        return new CommentDto
        {
            CommentId = comment.CommentId,
            PostId = comment.PostId,
            UserId = comment.UserId,
            Content = comment.Content,
            CreatedAt = comment.CreatedAt,
            Username = comment.User?.Username,
            ProfileImagePath = comment.User?.ProfilePicture,
            Replies = allComments
                .Where(r => r.ParentCommentId == comment.CommentId)
                .Select(r => MapCommentWithReplies(r, allComments))
                .ToList()
        };
    }

    // POST: api/comments
    [HttpPost]
    public async Task<IActionResult> CreateComment([FromBody] CreateCommentDto createCommentDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var comment = new Comment
        {
            PostId = createCommentDto.PostId,
            UserId = createCommentDto.UserId,
            Content = createCommentDto.Content,
            CreatedAt = DateTime.UtcNow,
            ParentCommentId = createCommentDto.ParentCommentId
        };

        _context.Comments.Add(comment);
        await _context.SaveChangesAsync();

        // Load the user info to include in the broadcasted comment
        var user = await _context.Users.FindAsync(comment.UserId);
        var broadcastComment = new
        {
            comment.CommentId,
            comment.PostId,
            comment.UserId,
            comment.Content,
            comment.CreatedAt,
            comment.ParentCommentId,
            Username = user?.Username,
            ProfileImagePath = user?.ProfilePicture
        };

        // Broadcast the new comment to all clients
        await _commentHub.Clients.All.SendAsync("ReceiveNewComment", broadcastComment);
        Console.WriteLine($"Broadcasted new comment to all clients: {broadcastComment.CommentId}, Content: {broadcastComment.Content}");

        var commentDto = new CommentDto
        {
            CommentId = comment.CommentId,
            PostId = comment.PostId,
            UserId = comment.UserId,
            Content = comment.Content,
            CreatedAt = comment.CreatedAt,
            ParentCommentId = comment.ParentCommentId,
            Username = user?.Username,
            ProfileImagePath = user?.ProfilePicture
        };

        return CreatedAtAction(nameof(GetCommentsByPost), new { postId = comment.PostId }, commentDto);
    }

    // GET: api/comments/{postId}/count
    [HttpGet("{postId}/count")]
    public async Task<IActionResult> GetCommentCount(int postId)
    {
        var count = await _context.Comments.CountAsync(c => c.PostId == postId);
        return Ok(count);
    }

    // DELETE: api/comments/{commentId}
    [HttpDelete("{commentId}")]
    public async Task<IActionResult> DeleteComment(int commentId)
    {
        var comment = await _context.Comments.FindAsync(commentId);
        if (comment == null)
        {
            return NotFound();
        }

        _context.Comments.Remove(comment);
        try
        {
            await _context.SaveChangesAsync();

            // Broadcast the deletion to all clients
            await _commentHub.Clients.All.SendAsync("DeleteComment", commentId);
            Console.WriteLine($"Broadcasted delete event for comment {commentId}");

            return NoContent(); // Indicates successful deletion
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting comment: {ex.Message}");
            return StatusCode(500, "Internal server error");
        }
    }
}
