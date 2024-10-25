﻿using System.Linq;
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
            var friendRequests = await _context.Friends
                .Where(f => f.UserId2 == userId && f.Status == "Pending")
                .Include(f => f.User1) // Assuming User1 is the requester
                .Select(f => new {
                    FriendId = f.FriendId,
                    Username = f.User1.Username,
                    ProfilePicture = f.User1.ProfilePicture,
                    MutualFriends = GetMutualFriendsCount(userId, f.UserId1)
                })
                .ToListAsync();

            return Ok(friendRequests);
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

        // Endpoint to get a list of all users (for adding friends)
        [HttpGet("all")]
        public async Task<ActionResult> GetAllUsers()
        {
            var users = await _context.Users
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
