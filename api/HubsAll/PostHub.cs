using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using api.Models;

namespace api.HubsAll
{
    public class PostHub : Hub
    {
        public async Task SendNewPost(Post post)
        {
            await Clients.All.SendAsync("ReceiveNewPost", post);
        }
    }
}
