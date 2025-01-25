//using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;

using WebAuthServerApp;
using WebAuthServerApp.Models;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;

Role adminRole = new Role() { Title = "admin" };
Role memberRole = new Role() { Title = "member" };

List<User> users = new List<User>()
{
    new(){ Login = "bobby", Password = "qwerty", Role = adminRole },
    new(){ Login = "jonny", Password = "12345", Role = memberRole },
    new(){ Login = "sammy", Password = "asddsa", Role = memberRole },
};

var builder = WebApplication.CreateBuilder(args);

//builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
//                .AddCookie(options => options.LoginPath = "/login");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new()
                    {
                        ValidateIssuer = true,
                        ValidIssuer = AuthOptions.Issuer,
                        ValidateAudience = true,
                        ValidAudience = AuthOptions.Audience,

                        ValidateLifetime = true,
                        IssuerSigningKey = AuthOptions.SymmetricSecurityKey,
                        ValidateIssuerSigningKey = true
                    };

                    options.Events = new JwtBearerEvents()
                    {
                        OnMessageReceived = context =>
                        {
                            var accessToken = context.Request.Query["access_token"];

                            var path = context.HttpContext.Request.Path;
                            if (!String.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/cms"))
                            {
                                context.Token = accessToken;
                            }
                            return Task.CompletedTask;
                        }
                    };
                });

builder.Services.AddAuthorization();
builder.Services.AddSignalR();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", [Authorize] async (HttpContext context) =>
                    await SendHtmlFile(context, "wwwroot/index.html"));

app.MapGet("/login", async (HttpContext context) =>
                    await SendHtmlFile(context, "wwwroot/login.html"));

app.MapPost("/login", async (string? redirectUrl ,HttpContext context) => 
{ 
    var loginForm = context.Request.Form;
    if (!loginForm.ContainsKey("login") || !loginForm.ContainsKey("password"))
        return Results.BadRequest("Login or password not defined");

    string login = loginForm["login"]!;
    string password = loginForm["password"]!;

    User? user = users.FirstOrDefault(u => u.Login == login && u.Password == password);
    if (user is null) return Results.Unauthorized();

    var claims = new List<Claim>()
    {
        new Claim(ClaimsIdentity.DefaultNameClaimType, user.Login),
        new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Role.Title)
    };
    ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "Cookies");
    ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

    JwtSecurityToken token = new(
        issuer: AuthOptions.Issuer,
        audience: AuthOptions.Audience,
        claims: claims,
        expires: DateTime.Now.Add(TimeSpan.FromMinutes(1)),
        signingCredentials: new SigningCredentials(AuthOptions.SymmetricSecurityKey, 
                                                   SecurityAlgorithms.HmacSha256));

    string tokenHandler = new JwtSecurityTokenHandler().WriteToken(token);

    var response = new
    {
        access_token = tokenHandler,
        login = user.Login,
    };

    //await context.SignInAsync(claimsPrincipal);

    //return Results.Redirect(redirectUrl ?? "/");
    return Results.Json(response);
});

app.MapGet("/admin", [Authorize(Roles = "admin")] async (HttpContext context) =>
                    await SendHtmlFile(context, "wwwroot/admin.html"));

app.MapGet("/logout", async (HttpContext context) =>
{
    //await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    return Results.Redirect("/");
});

app.MapHub<CmsHub>("/cms");


app.Run();




async Task SendHtmlFile(HttpContext context, string path)
{
    context.Response.ContentType = "text/html; charset=utf-8";
    await context.Response.SendFileAsync(path);
}


public class AuthOptions
{
    public static string Issuer = "MainIssuer";
    public static string Audience = "MainAudience";
    static string Key = "IkZhM5nxqlOVDbz23DnqlGSKkOn3Te";
    public static SymmetricSecurityKey SymmetricSecurityKey
        => new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Key));

}


