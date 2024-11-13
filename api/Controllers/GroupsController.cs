using api.Data;
using api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
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
        [HttpPost]
        public async Task<IActionResult> CreateGroup([FromBody] Group newGroup)
        {
            if (newGroup == null || string.IsNullOrEmpty(newGroup.GroupName))
            {
                return BadRequest("Group name is required.");
            }

            _context.Groups.Add(newGroup);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAllGroups), new { id = newGroup.GroupId }, newGroup);
        }

        // Join a group (via GroupMember)
        [HttpPost("join/{groupId}")]
        public async Task<IActionResult> JoinGroup(int groupId, [FromBody] int userId)
        {
            var group = await _context.Groups.FindAsync(groupId);
            if (group == null)
            {
                return NotFound("Group not found.");
            }

            var existingMember = await _context.GroupMembers
                .FirstOrDefaultAsync(gm => gm.GroupId == groupId && gm.UserId == userId);

            if (existingMember != null)
            {
                return BadRequest("User already a member.");
            }

            var groupMember = new GroupMember
            {
                GroupId = groupId,
                UserId = userId
            };

            _context.GroupMembers.Add(groupMember);
            await _context.SaveChangesAsync();

            return Ok("User joined the group.");
        }
    }
}
