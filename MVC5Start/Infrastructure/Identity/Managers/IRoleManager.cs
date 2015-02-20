using System.Linq;
using System.Threading.Tasks;
using MVC5Start.Models.Identity;
using Microsoft.AspNet.Identity;

namespace MVC5Start.Infrastructure.Identity.Managers
{
    public interface IRoleManager
    {
        Task<IdentityResult> CreateAsync(Role role);

        Task<IdentityResult> UpdateAsync(Role role);

        Task<IdentityResult> DeleteAsync(Role role);

        Task<bool> RoleExistsAsync(string roleName);

        Task<Role> FindByIdAsync(int roleId);

        Task<Role> FindByNameAsync(string roleName);

        IIdentityValidator<Role> RoleValidator { get; set; }

        IQueryable<Role> Roles { get; }
    }
}