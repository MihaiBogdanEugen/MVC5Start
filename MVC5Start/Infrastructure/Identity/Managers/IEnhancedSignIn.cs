using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVC5Start.Infrastructure.Identity.Managers
{
    public interface IEnhancedSignIn
    {
        Task<EnhancedSignInStatus> EnhancedPasswordSignInAsync(string email, string password, bool isPersistent, bool shouldLockout);

        Task<EnhancedSignInStatus> EnhancedTwoFactorSignInAsync(string provider, string code, bool isPersistent, bool rememberBrowser);
    }
}
