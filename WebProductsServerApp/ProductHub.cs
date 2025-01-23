using Microsoft.AspNetCore.SignalR;

namespace WebProductsServerApp
{
    public class ProductHub : Hub
    {
        //public async Task SendProduct(Product product)
        //{
        //    //Console.WriteLine($"{product.Name}");
        //    await Clients.All.SendAsync("Receive", product);
        //}
    }
}
