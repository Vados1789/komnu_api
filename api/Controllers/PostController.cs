using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using api.Data;
using api.Models;
using api.HubsAll;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using api.DTOs;
using Microsoft.AspNetCore.SignalR;

namespace api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PostsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IHubContext<PostHub> _postHub;
        private readonly string[] allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };

        public PostsController(AppDbContext context, IHubContext<PostHub> postHub)
        {
            _context = context;
            _postHub = postHub;
        }

        // GET: api/posts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Post>>> GetAllPosts()
        {
            try
            {
                var posts = await _context.Posts
                    .Include(p => p.User) // Include the user who created the post
                    .OrderByDescending(p => p.CreatedAt)
                    .ToListAsync();

                return Ok(posts);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching posts: {ex.Message}");
                return StatusCode(500, "An internal server error occurred while retrieving posts.");
            }
        }

        // GET: api/posts/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Post>> GetPostById(int id)
        {
            try
            {
                var post = await _context.Posts.Include(p => p.User).FirstOrDefaultAsync(p => p.PostId == id);

                if (post == null)
                {
                    return NotFound("Post not found.");
                }

                return Ok(post);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching post: {ex.Message}");
                return StatusCode(500, "An internal server error occurred while retrieving the post.");
            }
        }

        // POST: api/posts
        [HttpPost]
        public async Task<ActionResult<Post>> CreatePost([FromForm] CreatePostDto createPostDto)
        {
            Console.WriteLine($"[INFO] Received request to create post for user ID: {createPostDto.UserId}");

            try
            {
                if (!ModelState.IsValid)
                {
                    Console.WriteLine("[WARNING] Model state is invalid. Returning BadRequest.");
                    return BadRequest(ModelState);
                }

                string imagePath = "";
                if (createPostDto.Image != null)
                {
                    var extension = Path.GetExtension(createPostDto.Image.FileName).ToLower();
                    if (!Array.Exists(allowedExtensions, ext => ext == extension))
                    {
                        return BadRequest("Unsupported file type. Only .jpg, .jpeg, .png, and .gif are allowed.");
                    }

                    var fileName = Path.GetRandomFileName() + extension;
                    var imagesFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
                    var filePath = Path.Combine(imagesFolder, fileName);

                    Directory.CreateDirectory(imagesFolder);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await createPostDto.Image.CopyToAsync(stream);
                    }

                    imagePath = $"/images/{fileName}";
                }

                var newPost = new Post
                {
                    UserId = createPostDto.UserId,
                    Content = createPostDto.Content,
                    ImagePath = imagePath,
                    CreatedAt = DateTime.Now
                };

                _context.Posts.Add(newPost);
                await _context.SaveChangesAsync();

                // Broadcast the new post to all connected clients
                await _postHub.Clients.All.SendAsync("ReceiveNewPost", newPost);

                return CreatedAtAction(nameof(GetPostById), new { id = newPost.PostId }, newPost);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Error creating post: {ex.Message}");
                return StatusCode(500, "An internal server error occurred while creating the post.");
            }
        }

        // DELETE: api/posts/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePost(int id)
        {
            try
            {
                var post = await _context.Posts.Include(p => p.Comments).FirstOrDefaultAsync(p => p.PostId == id);

                if (post == null)
                {
                    return NotFound("Post not found.");
                }

                // Delete image file if it exists
                if (!string.IsNullOrEmpty(post.ImagePath))
                {
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", post.ImagePath.TrimStart('/'));
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                }

                // Remove comments and the post
                _context.Comments.RemoveRange(post.Comments);
                _context.Posts.Remove(post);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting post: {ex.Message}");
                return StatusCode(500, "An internal server error occurred while deleting the post.");
            }
        }


        // PUT: api/posts/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePost(int id, [FromForm] UpdatePostDto updatePostDto)
        {
            try
            {
                var post = await _context.Posts.FindAsync(id);
                if (post == null)
                {
                    return NotFound("Post not found.");
                }

                post.Content = updatePostDto.Content ?? post.Content;

                // If a new image is provided, save it and update the image path
                if (updatePostDto.Image != null)
                {
                    var extension = Path.GetExtension(updatePostDto.Image.FileName).ToLower();
                    if (!Array.Exists(allowedExtensions, ext => ext == extension))
                    {
                        return BadRequest("Unsupported file type. Only .jpg, .jpeg, .png, and .gif are allowed.");
                    }

                    var fileName = Path.GetRandomFileName() + extension;
                    var imagesFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
                    var filePath = Path.Combine(imagesFolder, fileName);

                    Directory.CreateDirectory(imagesFolder);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await updatePostDto.Image.CopyToAsync(stream);
                    }

                    // Update the post's image path with the new file
                    post.ImagePath = $"/images/{fileName}";
                }

                _context.Entry(post).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return NoContent(); // Return no content on successful update
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating post: {ex.Message}");
                return StatusCode(500, "An internal server error occurred while updating the post.");
            }
        }

    }
}
