using System.Data.Entity;
using Microsoft.AspNet.Identity.EntityFramework;
using MVC5Start.Models.Identity;

namespace MVC5Start.Infrastructure.Identity.Stores
{
    public class RoleStore : RoleStore<Role, int, UserRole>
    {
        public RoleStore()
            : base(new WebAppDbContext())
        {
            base.DisposeContext = true;
        }

        public RoleStore(DbContext context)
            : base(context)
        {
        }
    }
}