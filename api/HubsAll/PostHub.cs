using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using api.Models;

namespace api.HubsAll
{
    public class PostHub : Hub
    {
        // Method to send a new post to all clients
        public async Task SendNewPost(Post post)
        {
            Console.WriteLine($"[INFO] Broadcasting new post with ID {post.PostId} to all clients.");
            await Clients.All.SendAsync("ReceiveNewPost", post);
        }

        // Method to notify all clients when a post is deleted
        public async Task SendPostDeleted(int postId)
        {
            Console.WriteLine($"[INFO] Broadcasting post deletion with ID {postId} to all clients.");
            await Clients.All.SendAsync("ReceivePostDeleted", postId);
        }

        // Method to notify all clients when a post is updated
        public async Task SendPostUpdated(Post post)
        {
            Console.WriteLine($"[INFO] Broadcasting post update with ID {post.PostId} to all clients.");
            await Clients.All.SendAsync("ReceivePostUpdated", post);
        }
    }
}
