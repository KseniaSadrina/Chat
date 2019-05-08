using Microsoft.AspNetCore.Identity;

namespace Models
{
    public enum Roles
    {
        Administrator,
        Trainee,
        Instructor,
        Bot
    }
    public class Role : IdentityRole<int>
    {
    }
}
