using api.Data;
using api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GroupMemberController : ControllerBase
    {
        private readonly AppDbContext _context;

        public GroupMemberController(AppDbContext context)
        {
            _context = context;
        }

        // Get all groups the user is a member of
        [HttpGet("my-groups/{userId}")]
        public async Task<IActionResult> GetUserGroups(int userId)
        {
            var userGroups = await _context.GroupMembers
                .Where(gm => gm.UserId == userId)
                .Include(gm => gm.Group) // Include group details
                .Select(gm => gm.Group)  // Select only the group data
                .ToListAsync();

            if (userGroups == null || userGroups.Count == 0)
            {
                return NotFound("No groups found for the user.");
            }

            return Ok(userGroups);
        }

        // Leave a group
        [HttpPost("leave/{userId}/{groupId}")]
        public async Task<IActionResult> LeaveGroup(int userId, int groupId)
        {
            var groupMember = await _context.GroupMembers
                .FirstOrDefaultAsync(gm => gm.UserId == userId && gm.GroupId == groupId);

            if (groupMember == null)
            {
                return NotFound("Membership not found.");
            }

            _context.GroupMembers.Remove(groupMember);
            await _context.SaveChangesAsync();

            return Ok("User has left the group.");
        }
    }
}
