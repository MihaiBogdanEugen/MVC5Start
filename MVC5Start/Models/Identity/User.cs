using Microsoft.AspNet.Identity.EntityFramework;

namespace MVC5Start.Models.Identity
{
    public class User : IdentityUser<int, UserLogin,  UserRole, UserClaim> { }
}