using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using api.Models;

namespace api.HubsAll
{
    public class CommentHub : Hub
    {
        public async Task SendNewComment(Comment comment)
        {
            await Clients.All.SendAsync("ReceiveNewComment", comment);
        }
    }
}
