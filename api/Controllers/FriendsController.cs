using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.DTOs;
using api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FriendsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public FriendsController(AppDbContext context)
        {
            _context = context;
        }

        // Endpoint to get friend requests for a user
        [HttpGet("requests/{userId}")]
        public async Task<ActionResult> GetFriendRequests(int userId)
        {
            try
            {
                // First, retrieve the basic friend request information
                var friendRequests = await _context.Friends
                    .Where(f => f.UserId2 == userId && f.Status == "Pending")
                    .Include(f => f.User1) // Assuming User1 is the requester
                    .Select(f => new {
                        FriendId = f.FriendId,
                        Username = f.User1 != null ? f.User1.Username : "Unknown User",
                        ProfilePicture = f.User1 != null ? f.User1.ProfilePicture : null,
                        UserId1 = f.UserId1 // Store the requester UserId for mutual friend calculation
                    })
                    .ToListAsync();

                // Loop over each friend request to calculate mutual friends
                var result = friendRequests.Select(f => new {
                    f.FriendId,
                    f.Username,
                    f.ProfilePicture,
                    MutualFriends = GetMutualFriendsCount(userId, f.UserId1)
                }).ToList();

                return Ok(result);
            }
            catch (Exception ex)
            {
                // Log the detailed error and return a specific error message
                Console.WriteLine($"[ERROR] Error fetching friend requests for user ID {userId}: {ex.Message}");
                return StatusCode(500, "An error occurred while fetching friend requests.");
            }
        }

        // Endpoint to get all friends for a user
        [HttpGet("list/{userId}")]
        public async Task<ActionResult> GetFriendsList(int userId)
        {
            var friends = await _context.Friends
                .Where(f => (f.UserId1 == userId || f.UserId2 == userId) && f.Status == "Accepted")
                .Include(f => f.User1)
                .Include(f => f.User2)
                .Select(f => new {
                    FriendId = f.FriendId,
                    Username = f.UserId1 == userId ? f.User2.Username : f.User1.Username,
                    ProfilePicture = f.UserId1 == userId ? f.User2.ProfilePicture : f.User1.ProfilePicture
                })
                .ToListAsync();

            // Log the friends list to the console
            Console.WriteLine("Friends list being sent to frontend:");
            foreach (var friend in friends)
            {
                Console.WriteLine($"FriendId: {friend.FriendId}, Username: {friend.Username}, ProfilePicture: {friend.ProfilePicture}");
            }

            return Ok(friends);
        }

        [HttpGet("all/{userId}")]
        public async Task<ActionResult> GetAllUsers(int userId)
        {
            System.Console.WriteLine($"bob {userId}");
            var users = await _context.Users
                .Where(u => u.UserId != userId)
                .Select(u => new {
                    u.UserId,
                    u.Username,
                    u.ProfilePicture
                })
                .ToListAsync();

            return Ok(users);
        }

        // Endpoint to send a friend request
        [HttpPost("send")]
        public async Task<IActionResult> SendFriendRequest([FromBody] FriendRequestDto request)
        {
            // Check if a pending or accepted friend request already exists
            var existingRequest = await _context.Friends
                .FirstOrDefaultAsync(f =>
                    ((f.UserId1 == request.UserId1 && f.UserId2 == request.UserId2) ||
                     (f.UserId1 == request.UserId2 && f.UserId2 == request.UserId1)) &&
                    (f.Status == "Pending" || f.Status == "Accepted"));

            if (existingRequest != null)
            {
                return BadRequest("Friend request already exists or you are already friends.");
            }

            var newFriendRequest = new Friend
            {
                UserId1 = request.UserId1, // Sender
                UserId2 = request.UserId2, // Receiver
                Status = "Pending"
            };

            _context.Friends.Add(newFriendRequest);
            await _context.SaveChangesAsync();
            return Ok();
        }


        // Endpoint to confirm a friend request
        [HttpPost("confirm")]
        public async Task<IActionResult> ConfirmFriendRequest([FromBody] int friendId)
        {
            var friendRequest = await _context.Friends.FindAsync(friendId);
            if (friendRequest != null && friendRequest.Status == "Pending")
            {
                friendRequest.Status = "Accepted";
                await _context.SaveChangesAsync();
                return Ok();
            }
            return BadRequest("Unable to confirm friend request.");
        }

        // Endpoint to remove a friend request or an existing friend
        [HttpPost("remove")]
        public async Task<IActionResult> RemoveFriendRequest([FromBody] int friendId)
        {
            var friendRequest = await _context.Friends.FindAsync(friendId);
            if (friendRequest != null)
            {
                _context.Friends.Remove(friendRequest);
                await _context.SaveChangesAsync();
                return Ok();
            }
            return BadRequest("Unable to remove friend request.");
        }

        private int GetMutualFriendsCount(int userId1, int userId2)
        {
            // Placeholder for actual mutual friends calculation
            return _context.Friends
                .Where(f => (f.UserId1 == userId1 || f.UserId2 == userId1) &&
                            (f.UserId1 == userId2 || f.UserId2 == userId2) &&
                            f.Status == "Accepted")
                .Count();
        }
    }
}
