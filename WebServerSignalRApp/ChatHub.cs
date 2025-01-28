using Microsoft.AspNetCore.SignalR;

namespace WebServerSignalRApp
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(string user, string message)
        {
            await this.Clients.All.SendAsync("Reseive", user, message);
        }

        public override async Task OnConnectedAsync()
        {
            //await this.Clients.All.SendAsync("Notify", $"{Context.ConnectionId} logged into the chat");
            await this.Clients.Others.SendAsync("Notify", $"other logged into the chat");
            await this.Clients.Caller.SendAsync("Notify", $"you logged into the chat");
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await this.Clients.All.SendAsync("Notify", $"{Context.ConnectionId} leaves the chat");
            await base.OnDisconnectedAsync(exception);
        }
    }
}
