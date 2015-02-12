using Microsoft.AspNet.Identity.EntityFramework;
using MVC5Start.Models.Identity;

namespace MVC5Start.Infrastructure
{
    public class WebAppDbContext : IdentityDbContext<User, Role, int, UserLogin, UserRole, UserClaim>
    {
        public WebAppDbContext()
            : base("DefaultConnection")
        {
        }

        public static WebAppDbContext Create()
        {
            return new WebAppDbContext();
        }
    }
}