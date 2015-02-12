using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using MVC5Start.Infrastructure.Identity.Stores;
using MVC5Start.Models.Identity;

namespace MVC5Start.Infrastructure.Identity.Managers
{
    public class ApplicationRoleManager : RoleManager<Role, int>
    {
        public ApplicationRoleManager(IRoleStore<Role, int> roleStore) : base(roleStore)
        {

        }

        public static ApplicationRoleManager Create(IdentityFactoryOptions<ApplicationRoleManager> options, IOwinContext context)
        {
            return new ApplicationRoleManager(new RoleStore(context.Get<DbConnectionInfo>()));
        }
    }
}