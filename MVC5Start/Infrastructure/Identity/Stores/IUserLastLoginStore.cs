using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

namespace MVC5Start.Infrastructure.Identity.Stores
{
    public interface IUserLastLoginStore<in TUser, in TKey> where TUser : class, IUser<TKey>
    {
        Task RecordLastLoginAtAsync(TUser user);
    }
}
