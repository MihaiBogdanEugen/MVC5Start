using Microsoft.AspNet.Identity;
using MVC5Start.Infrastructure.Identity.Stores;
using MVC5Start.Models.Identity;

namespace MVC5Start.Infrastructure.Identity.Managers
{
    public class AppRoleManager : RoleManager<Role, int>
    {
        public AppRoleManager(DbConnectionInfo dbConnectionInfo) : base(new AppRoleStore(dbConnectionInfo)) { }
    }
}