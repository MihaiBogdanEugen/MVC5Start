using System.Data.Entity;
using Microsoft.AspNet.Identity.EntityFramework;
using MVC5Start.Models.Identity;

namespace MVC5Start.Infrastructure.Identity.Stores
{
    public class UserStore : UserStore<User, Role, int, UserLogin, UserRole, UserClaim>
    {
        public UserStore() 
            : base(new WebAppDbContext())
        {
            base.DisposeContext = true;
        }

        public UserStore(DbContext context) 
            : base(context)
        {
        }
    }
}