// GroupPostsController.cs
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.DTOs;
using api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GroupPostsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public GroupPostsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/GroupPosts/{groupId}
        [HttpGet("{groupId}")]
        public async Task<IActionResult> GetGroupPosts(int groupId)
        {
            Console.WriteLine($"[INFO] Fetching posts for group with ID: {groupId}");
            var posts = await _context.GroupPosts
                .Where(p => p.GroupId == groupId)
                .Include(p => p.User) // Include user data with each post
                .OrderByDescending(p => p.CreatedAt)
                .Select(p => new
                {
                    p.PostId,
                    p.GroupId,
                    p.UserId,
                    p.Content,
                    p.ImagePath,
                    p.CreatedAt,
                    User = new
                    {
                        p.User.UserId,
                        p.User.Username,
                        p.User.ProfilePicture // Assuming ProfilePicture is a property in the User model
                    }
                })
                .ToListAsync();

            if (posts == null || posts.Count == 0)
            {
                Console.WriteLine($"[INFO] No posts found for group ID: {groupId}");
                return NotFound("No posts found for this group.");
            }

            Console.WriteLine($"[INFO] Retrieved {posts.Count} posts for group ID: {groupId}");

            return Ok(posts);
        }

        // POST: api/GroupPosts/add
        [HttpPost("add")]
        public async Task<IActionResult> AddPost([FromForm] GroupPostDto newPostDto)
        {
            Console.WriteLine("[INFO] AddPost endpoint hit.");

            if (newPostDto == null)
            {
                Console.WriteLine("[ERROR] newPostDto is null.");
                return BadRequest("Request data is missing.");
            }

            Console.WriteLine($"[INFO] Received post data - GroupId: {newPostDto.GroupId}, UserId: {newPostDto.UserId}, Content: {newPostDto.Content}");

            if (string.IsNullOrEmpty(newPostDto.Content))
            {
                Console.WriteLine("[ERROR] Post content is empty.");
                return BadRequest("Post content cannot be empty.");
            }

            // Handle image upload if it exists
            string imagePath = null;
            if (newPostDto.Image != null && newPostDto.Image.Length > 0)
            {
                Console.WriteLine("[INFO] Image detected in request.");
                var extension = Path.GetExtension(newPostDto.Image.FileName).ToLower();
                if (!new[] { ".jpg", ".jpeg", ".png", ".gif" }.Contains(extension))
                {
                    Console.WriteLine($"[ERROR] Unsupported file type: {extension}");
                    return BadRequest("Unsupported file type. Only .jpg, .jpeg, .png, and .gif are allowed.");
                }

                var fileName = Path.GetRandomFileName() + extension;
                var imagesFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
                var filePath = Path.Combine(imagesFolder, fileName);

                Directory.CreateDirectory(imagesFolder);

                try
                {
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await newPostDto.Image.CopyToAsync(stream);
                    }

                    imagePath = $"/images/{fileName}";
                    Console.WriteLine($"[INFO] Image uploaded successfully. Path: {imagePath}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[ERROR] Image upload failed: {ex.Message}");
                    return StatusCode(500, "Error saving image.");
                }
            }
            else
            {
                Console.WriteLine("[INFO] No image provided in request.");
            }

            var newPost = new GroupPost
            {
                GroupId = newPostDto.GroupId,
                UserId = newPostDto.UserId,
                Content = newPostDto.Content,
                ImagePath = imagePath, // This will be null if no image is provided
                CreatedAt = DateTime.Now
            };

            try
            {
                _context.GroupPosts.Add(newPost);
                await _context.SaveChangesAsync();
                Console.WriteLine("[INFO] Post added successfully.");
            }
            catch (DbUpdateException dbEx)
            {
                Console.WriteLine($"[ERROR] Database update error: {dbEx.InnerException?.Message ?? dbEx.Message}");
                return StatusCode(500, "Error saving post to the database.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to save post: {ex.Message}");
                return StatusCode(500, "Error saving post.");
            }

            return CreatedAtAction(nameof(GetGroupPosts), new { groupId = newPost.GroupId }, newPost);
        }

        // POST: api/GroupPosts/addText
        [HttpPost("addText")]
        public async Task<IActionResult> AddTextPost([FromBody] GroupTextPostDto newPostDto)
        {
            Console.WriteLine("[INFO] AddTextPost endpoint hit.");

            if (newPostDto == null)
            {
                Console.WriteLine("[ERROR] newPostDto is null.");
                return BadRequest("Request data is missing.");
            }

            if (string.IsNullOrEmpty(newPostDto.Content))
            {
                Console.WriteLine("[ERROR] Post content is empty.");
                return BadRequest("Post content cannot be empty.");
            }

            var newPost = new GroupPost
            {
                GroupId = newPostDto.GroupId,
                UserId = newPostDto.UserId,
                Content = newPostDto.Content,
                CreatedAt = DateTime.Now
            };

            try
            {
                _context.GroupPosts.Add(newPost);
                await _context.SaveChangesAsync();
                Console.WriteLine("[INFO] Text-only post added successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to save text-only post: {ex.Message}");
                return StatusCode(500, "Error saving post.");
            }

            return CreatedAtAction(nameof(GetGroupPosts), new { groupId = newPost.GroupId }, newPost);
        }

        // DELETE: api/GroupPosts/delete/{postId}
        [HttpDelete("delete/{postId}")]
        public async Task<IActionResult> DeletePost(int postId)
        {
            var post = await _context.GroupPosts.FindAsync(postId);
            if (post == null)
            {
                return NotFound("Post not found.");
            }

            // Remove the associated image file if it exists
            if (!string.IsNullOrEmpty(post.ImagePath))
            {
                var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", post.ImagePath.TrimStart('/'));
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                    Console.WriteLine($"[INFO] Deleted image file at path: {imagePath}");
                }
            }

            _context.GroupPosts.Remove(post);
            await _context.SaveChangesAsync();

            Console.WriteLine("[INFO] Post deleted successfully.");
            return Ok("Post deleted.");
        }
    }
}
