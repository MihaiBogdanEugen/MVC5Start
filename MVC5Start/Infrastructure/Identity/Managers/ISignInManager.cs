using System.Security.Claims;
using System.Threading.Tasks;
using MVC5Start.Models.Identity;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;

namespace MVC5Start.Infrastructure.Identity.Managers
{
    public interface ISignInManager
    {
        Task SignInAsync(User user, bool isPersistent, bool rememberBrowser);
        
        Task<EnhancedSignInStatus> EnhancedPasswordSignInAsync(string email, string password, bool isPersistent, bool shouldLockout = true);
        
        Task<EnhancedSignInStatus> EnhancedTwoFactorSignInAsync(string provider, string code, bool isPersistent, bool rememberBrowser);

        Task<ClaimsIdentity> CreateUserIdentityAsync(User user);
        
        Task<bool> SendTwoFactorCodeAsync(string provider);
        
        Task<int> GetVerifiedUserIdAsync();
        
        Task<bool> HasBeenVerifiedAsync();
        
        Task<SignInStatus> TwoFactorSignInAsync(string provider, string code, bool isPersistent, bool rememberBrowser);
        
        Task<SignInStatus> ExternalSignInAsync(ExternalLoginInfo loginInfo, bool isPersistent);

        Task<SignInStatus> PasswordSignInAsync(string userName, string password, bool isPersistent, bool shouldLockout);

        string AuthenticationType { get; set; }

        UserManager<User, int> UserManager { get; set; }

        IAuthenticationManager AuthenticationManager { get; set; }
    }
}