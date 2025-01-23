using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;

using WebAuthServerApp;
using WebAuthServerApp.Models;

Role adminRole = new Role() { Title = "admin" };
Role memberRole = new Role() { Title = "member" };

List<User> users = new List<User>()
{
    new(){ Login = "bobby", Password = "qwerty", Role = adminRole },
    new(){ Login = "jonny", Password = "12345", Role = memberRole },
    new(){ Login = "sammy", Password = "asddsa", Role = memberRole },
};

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options => options.LoginPath = "/login");
builder.Services.AddAuthorization();
builder.Services.AddSignalR();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", [Authorize] async (HttpContext context) =>
                    await SendHtmlFile(context, "wwwroot/index.html"));


app.Run();




async Task SendHtmlFile(HttpContext context, string path)
{
    context.Response.ContentType = "text/html; charset=utf-8";
    await context.Response.SendFileAsync(path);
}
