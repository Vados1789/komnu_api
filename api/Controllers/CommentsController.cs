using api.Data;
using api.DTOs;
using api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

[ApiController]
[Route("api/[controller]")]
public class CommentsController : ControllerBase
{
    private readonly AppDbContext _context;

    public CommentsController(AppDbContext context)
    {
        _context = context;
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
        // Fetch all comments related to the post, including the User and Replies
        var allComments = await _context.Comments
            .Where(c => c.PostId == postId)
            .Include(c => c.User) // Include user information
            .Include(c => c.Replies)
                .ThenInclude(r => r.User) // Include user information for replies
            .ToListAsync();

        // Filter top-level comments (ParentCommentId is null)
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
                .Where(r => r.ParentCommentId == comment.CommentId) // Find replies with the current comment's ID as their parent
                .Select(r => MapCommentWithReplies(r, allComments)) // Recursively map each reply
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
            ParentCommentId = createCommentDto.ParentCommentId // Set ParentCommentId if it's a reply
        };

        _context.Comments.Add(comment);
        await _context.SaveChangesAsync();

        var commentDto = new CommentDto
        {
            CommentId = comment.CommentId,
            PostId = comment.PostId,
            UserId = comment.UserId,
            Content = comment.Content,
            CreatedAt = comment.CreatedAt,
            ParentCommentId = comment.ParentCommentId
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

    [HttpDelete("{commentId}")]
    public async Task<IActionResult> DeleteComment(int commentId)
    {
        var comment = await _context.Comments.FindAsync(commentId);
        if (comment == null)
        {
            return NotFound();
        }

        _context.Comments.Remove(comment);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
