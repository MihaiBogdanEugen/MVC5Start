using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

namespace MVC5Start.Infrastructure.Identity.Stores
{
    public interface IUserDisabledStore<in TUser, in TKey> where TUser : class, IUser<TKey>
    {
        Task<bool> IsDisabledAsync(TUser user);

        Task DisableAsync(TUser user);

        Task EnableAsync(TUser user);
    }
}