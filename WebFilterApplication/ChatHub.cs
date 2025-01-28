using Microsoft.AspNetCore.SignalR;

namespace WebFilterApplication
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(string username, string message)
        {
            await Clients.All.SendAsync("Receive", username, message);
        }
    }
}
