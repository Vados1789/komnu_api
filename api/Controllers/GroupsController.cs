using api.Data;
using api.Models;
using api.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Threading.Tasks;

namespace api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GroupsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public GroupsController(AppDbContext context)
        {
            _context = context;
        }

        // Get all groups
        [HttpGet]
        public async Task<IActionResult> GetAllGroups()
        {
            var groups = await _context.Groups
                .Include(g => g.Members) // Include members
                .ToListAsync();

            return Ok(groups);
        }

        // Create a new group
        [HttpPost("create")]
        public async Task<IActionResult> CreateGroup([FromForm] CreateGroupDto createGroupDto)
        {
            Console.WriteLine($"[INFO] Creating a new group: {createGroupDto.GroupName}, by user {createGroupDto.UserId}");

            if (createGroupDto == null || string.IsNullOrEmpty(createGroupDto.GroupName))
            {
                return BadRequest("Group name is required.");
            }

            // Handle the image file if it exists
            string imagePath = string.Empty; // Default to an empty string if no image is provided
            if (createGroupDto.Image != null)
            {
                var extension = Path.GetExtension(createGroupDto.Image.FileName).ToLower();
                if (!Array.Exists(new[] { ".jpg", ".jpeg", ".png", ".gif" }, ext => ext == extension))
                {
                    return BadRequest("Unsupported file type. Only .jpg, .jpeg, .png, and .gif are allowed.");
                }

                var fileName = Path.GetRandomFileName() + extension;
                var imagesFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
                var filePath = Path.Combine(imagesFolder, fileName);

                Directory.CreateDirectory(imagesFolder);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await createGroupDto.Image.CopyToAsync(stream);
                }

                imagePath = $"/images/{fileName}";
            }

            // Create the group object from the DTO
            var newGroup = new Group
            {
                GroupName = createGroupDto.GroupName,
                Description = createGroupDto.Description,
                ImageUrl = imagePath,
                CreatorUserId = createGroupDto.UserId,
                CreatedAt = DateTime.Now
            };

            // Save the new group
            _context.Groups.Add(newGroup);
            await _context.SaveChangesAsync();

            // Optionally: Add the creator as a member of the group
            var groupMember = new GroupMember
            {
                GroupId = newGroup.GroupId,
                UserId = createGroupDto.UserId,
                JoinedAt = DateTime.Now
            };
            _context.GroupMembers.Add(groupMember);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAllGroups), new { id = newGroup.GroupId }, newGroup);
        }

        // Join a group (via GroupMember)
        [HttpPost("join/{groupId}")]
        public async Task<IActionResult> JoinGroup(int groupId, [FromBody] int userId)
        {
            Console.WriteLine($"[JoinGroup] Request received: groupId = {groupId}, userId = {userId}");

            var group = await _context.Groups.FindAsync(groupId);
            if (group == null)
            {
                Console.WriteLine("[JoinGroup] Group not found.");
                return NotFound("Group not found.");
            }

            var existingMember = await _context.GroupMembers
                .FirstOrDefaultAsync(gm => gm.GroupId == groupId && gm.UserId == userId);

            if (existingMember != null)
            {
                Console.WriteLine("[JoinGroup] User already a member.");
                return BadRequest("User already a member.");
            }

            var groupMember = new GroupMember
            {
                GroupId = groupId,
                UserId = userId,
                JoinedAt = DateTime.Now
            };

            _context.GroupMembers.Add(groupMember);
            await _context.SaveChangesAsync();

            Console.WriteLine("[JoinGroup] User successfully joined the group.");
            return Ok("User joined the group.");
        }

        // Leave a group (remove from GroupMember)
        [HttpPost("leave/{groupId}")]
        public async Task<IActionResult> LeaveGroup(int groupId, [FromBody] int userId)
        {
            Console.WriteLine($"[LeaveGroup] Request received: groupId = {groupId}, userId = {userId}");

            var groupMember = await _context.GroupMembers
                .FirstOrDefaultAsync(gm => gm.GroupId == groupId && gm.UserId == userId);

            if (groupMember == null)
            {
                Console.WriteLine("[LeaveGroup] User is not a member of this group.");
                return NotFound("You are not a member of this group.");
            }

            _context.GroupMembers.Remove(groupMember);
            await _context.SaveChangesAsync();

            Console.WriteLine("[LeaveGroup] User successfully left the group.");
            return Ok("User left the group.");
        }
    }
}
