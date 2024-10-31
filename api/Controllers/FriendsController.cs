using System;
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

        [HttpGet("all-with-status/{userId}")]
        public async Task<ActionResult> GetAllUsersWithStatus(int userId)
        {
            try
            {
                var users = await _context.Users
                    .Where(u => u.UserId != userId)
                    .Select(u => new {
                        u.UserId,
                        u.Username,
                        u.ProfilePicture,
                        FriendStatus = _context.Friends
                            .Where(f => (f.UserId1 == userId && f.UserId2 == u.UserId) ||
                                        (f.UserId1 == u.UserId && f.UserId2 == userId))
                            .Select(f => f.Status)
                            .FirstOrDefault() ?? "None" // If no relation, set as "None"
                    })
                    .ToListAsync();

                Console.WriteLine("All users with friend status being sent to frontend:");
                foreach (var user in users)
                {
                    Console.WriteLine($"UserId: {user.UserId}, Username: {user.Username}, Status: {user.FriendStatus}");
                }

                return Ok(users);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Error fetching users with friend status for user ID {userId}: {ex.Message}");
                return StatusCode(500, "An error occurred while fetching users with status.");
            }
        }

        // Endpoint to get friend requests for a user
        [HttpGet("requests/{userId}")]
        public async Task<ActionResult> GetFriendRequests(int userId)
        {
            try
            {
                var friendRequests = await _context.Friends
                    .Where(f => f.UserId2 == userId && f.Status == "Pending")
                    .Include(f => f.User1) // Assuming User1 is the requester
                    .Select(f => new {
                        FriendId = f.FriendId,
                        Username = f.User1 != null ? f.User1.Username : "Unknown User",
                        ProfilePicture = f.User1 != null ? f.User1.ProfilePicture : null,
                        UserId1 = f.UserId1
                    })
                    .ToListAsync();

                // Log each friend request
                Console.WriteLine("Friend requests being sent to frontend:");
                foreach (var friendRequest in friendRequests)
                {
                    Console.WriteLine($"FriendId: {friendRequest.FriendId}, Username: {friendRequest.Username}, ProfilePicture: {friendRequest.ProfilePicture}");
                }

                // Return the result with mutual friends count calculation
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
                Console.WriteLine($"[ERROR] Error fetching friend requests for user ID {userId}: {ex.Message}");
                return StatusCode(500, "An error occurred while fetching friend requests.");
            }
        }

        // Endpoint to get all friends for a user
        [HttpGet("list/{userId}")]
        public async Task<ActionResult> GetFriendsList(int userId)
        {
            try
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

                // Log each friend in the list
                Console.WriteLine("Friends list being sent to frontend:");
                foreach (var friend in friends)
                {
                    Console.WriteLine($"FriendId: {friend.FriendId}, Username: {friend.Username}, ProfilePicture: {friend.ProfilePicture}");
                }

                return Ok(friends);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Error fetching friends list for user ID {userId}: {ex.Message}");
                return StatusCode(500, "An error occurred while fetching the friends list.");
            }
        }

        // Endpoint to get all users except the current user
        [HttpGet("all/{userId}")]
        public async Task<ActionResult> GetAllUsers(int userId)
        {
            try
            {
                var users = await _context.Users
                    .Where(u => u.UserId != userId)
                    .Select(u => new {
                        u.UserId,
                        u.Username,
                        u.ProfilePicture
                    })
                    .ToListAsync();

                Console.WriteLine("All users being sent to frontend:");
                foreach (var user in users)
                {
                    Console.WriteLine($"UserId: {user.UserId}, Username: {user.Username}, ProfilePicture: {user.ProfilePicture}");
                }

                return Ok(users);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Error fetching all users except user ID {userId}: {ex.Message}");
                return StatusCode(500, "An error occurred while fetching all users.");
            }
        }

        // Endpoint to send a friend request
        [HttpPost("send")]
        public async Task<IActionResult> SendFriendRequest([FromBody] FriendRequestDto request)
        {
            try
            {
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
                    UserId1 = request.UserId1,
                    UserId2 = request.UserId2,
                    Status = "Pending"
                };

                _context.Friends.Add(newFriendRequest);
                await _context.SaveChangesAsync();

                return Ok("Friend request sent successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Error sending friend request: {ex.Message}");
                return StatusCode(500, "An error occurred while sending the friend request.");
            }
        }

        // Endpoint to confirm a friend request
        [HttpPost("confirm")]
        public async Task<IActionResult> ConfirmFriendRequest([FromBody] FriendRequestActionDto request)
        {
            try
            {
                var friendRequest = await _context.Friends.FindAsync(request.FriendId);
                if (friendRequest != null && friendRequest.Status == "Pending")
                {
                    friendRequest.Status = "Accepted";
                    await _context.SaveChangesAsync();
                    return Ok("Friend request confirmed.");
                }
                return BadRequest("Unable to confirm friend request.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Error confirming friend request: {ex.Message}");
                return StatusCode(500, "An error occurred while confirming the friend request.");
            }
        }

        // RemoveFriendRequest
        [HttpPost("remove")]
        public async Task<IActionResult> RemoveFriendRequest([FromBody] FriendRequestActionDto request)
        {
            try
            {
                var friendRequest = await _context.Friends.FindAsync(request.FriendId);
                if (friendRequest != null)
                {
                    _context.Friends.Remove(friendRequest);
                    await _context.SaveChangesAsync();
                    return Ok("Friend removed.");
                }
                return BadRequest("Unable to remove friend.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Error removing friend: {ex.Message}");
                return StatusCode(500, "An error occurred while removing the friend.");
            }
        }

        private int GetMutualFriendsCount(int userId1, int userId2)
        {
            return _context.Friends
                .Where(f => (f.UserId1 == userId1 || f.UserId2 == userId1) &&
                            (f.UserId1 == userId2 || f.UserId2 == userId2) &&
                            f.Status == "Accepted")
                .Count();
        }
    }
}
