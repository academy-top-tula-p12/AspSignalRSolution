using Microsoft.AspNetCore.SignalR;
using WebProductsServerApp;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR();

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapPost("create", async (Product product, IHubContext<ProductHub> context) =>
{
    await context.Clients.All.SendAsync("Receive", product); //$"Product name: {product.Name}, price: {product.Price}");
});

app.MapHub<ProductHub>("/shop");

app.Run();
