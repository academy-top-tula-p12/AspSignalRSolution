using Microsoft.AspNetCore.SignalR;

namespace WebServerSignalRApp
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(string user, string message)
        {
            await this.Clients.All.SendAsync("Reseive", user, message);
        }
    }
}
