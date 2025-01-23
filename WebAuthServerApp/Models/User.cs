namespace WebAuthServerApp.Models
{
    public class Role
    {
        public string Title { get; set; } = "";
    }
    public class User
    {
        public string Login { get; set; } = "";
        public string Password { get; set; } = "";
        public Role Role { get; set; } = null!;
    }
}
