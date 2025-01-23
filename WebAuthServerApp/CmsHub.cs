using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace WebAuthServerApp
{
    [Authorize]
    public class CmsHub : Hub
    {
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("Receive", user, message);
        }

        [Authorize(Roles = "admin")]
        public async Task NotifyMessage(string message)
        {
            await Clients.All.SendAsync("Notify", "admin", message);
        }
    }
}
