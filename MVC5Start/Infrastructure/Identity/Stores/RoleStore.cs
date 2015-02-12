using System;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNet.Identity;
using MVC5Start.Models.Identity;

namespace MVC5Start.Infrastructure.Identity.Stores
{
    public class RoleStore : BaseStore, IQueryableRoleStore<Role, int>
    {
        public RoleStore(DbConnectionInfo dbConnectionInfo) : base(dbConnectionInfo) { }

        #region IRoleStore Members

        public async Task CreateAsync(Role role)
        {
            if (role == null)
                throw new ArgumentNullException("role");

            await this.Connection.ExecuteAsync(Sql.Roles.Insert, new { role.Name, role.Description });
        }

        public async Task UpdateAsync(Role role)
        {
            if (role == null)
                throw new ArgumentNullException("role");

            await this.Connection.ExecuteAsync(Sql.Roles.Update, new { role.Name, role.Description, role.Id });
        }

        public async Task DeleteAsync(Role role)
        {
            if (role == null)
                throw new ArgumentNullException("role");

            using (var transaction = this.Connection.BeginTransaction())
            {
                var success = true;
                try
                {
                    await this.Connection.ExecuteAsync(Sql.UserRoles.DeleteByRoleId, new { RoleId = role.Id }, transaction);

                    await this.Connection.ExecuteAsync(Sql.Roles.Delete, new { role.Id }, transaction);
                }
                catch
                {
                    success = false;
                }

                if (success)
                    transaction.Commit();
                else
                    transaction.Rollback();
            }
        }

        public async Task<Role> FindByIdAsync(int roleId)
        {
            if (roleId < 1)
                throw new ArgumentException("Invalid roleId provided!", "roleId");

            return (await this.Connection.QueryAsync<Role>(Sql.Roles.FindById, new {Name = roleId})).FirstOrDefault();
        }

        public async Task<Role> FindByNameAsync(string roleName)
        {
            if (string.IsNullOrEmpty(roleName))
                throw new ArgumentNullException("roleName");

            return (await this.Connection.QueryAsync<Role>(Sql.Roles.FindByName, new { Name = roleName })).FirstOrDefault();
        }

        #endregion IRoleStore Members

        #region IQueryableRoleStore Members

        public IQueryable<Role> Roles
        {
            get
            {
                return this.Connection.Query<Role>(Sql.Roles.Select).AsQueryable();
            }
        }

        #endregion IQueryableRoleStore Members
    }
}