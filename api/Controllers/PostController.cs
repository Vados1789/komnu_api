﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using api.Data;
using api.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using api.DTOs;

namespace api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PostsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly string[] allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };

        public PostsController(AppDbContext context)
        {
            _context = context;
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
                var post = await _context.Posts.FindAsync(id);
                if (post == null)
                {
                    return NotFound("Post not found.");
                }

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
    }
}
