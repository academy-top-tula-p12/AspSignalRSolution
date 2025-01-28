using Microsoft.AspNetCore.SignalR;
using WebFilterApplication;

var builder = WebApplication.CreateBuilder(args);
//builder.Services.AddSignalR(options => options.AddFilter<HubFilter>());

//builder.Services.AddSignalR(options => options.AddFilter<HubFilter>())
//                .AddHubOptions<ChatHub>(options => options.AddFilter<HubFilter>());

builder.Services.AddSignalR(options => options.AddFilter(new HubFilter()))
                .AddHubOptions<ChatHub>(options => options.AddFilter(new HubFilter()));

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();


app.MapHub<ChatHub>("/chat");

app.Run();
