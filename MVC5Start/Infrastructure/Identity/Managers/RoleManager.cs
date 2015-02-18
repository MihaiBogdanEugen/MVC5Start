using System;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using MVC5Start.Infrastructure.Identity.Stores;
using MVC5Start.Models.Identity;

namespace MVC5Start.Infrastructure.Identity.Managers
{
    public class RoleManager : RoleManager<Role, int>
    {
        #region Constructors

        public RoleManager(DbConnectionInfo dbConnectionInfo) : base(new RoleStore(dbConnectionInfo)) { }

        #endregion Constructors

        #region Overriden Members

        public override async Task<IdentityResult> CreateAsync(Role role)
        {
            if (role == null)
                throw new ArgumentNullException("role");

            var result = role.IsValid();
            if (result.Succeeded == false)
                return result.IdentityResult;

            await this.Store.CreateAsync(role);

            return IdentityResult.Success;
        }

        public override async Task<IdentityResult> UpdateAsync(Role role)
        {
            if (role == null)
                throw new ArgumentNullException("role");

            var result = role.IsValid();
            if (result.Succeeded == false)
                return result.IdentityResult;

            await this.Store.UpdateAsync(role);

            return IdentityResult.Success;
        }

        #endregion Overriden Members
    }
}