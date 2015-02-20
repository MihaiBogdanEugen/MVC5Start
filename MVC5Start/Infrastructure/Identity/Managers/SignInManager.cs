using System.Globalization;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using MVC5Start.Models.Identity;

namespace MVC5Start.Infrastructure.Identity.Managers
{
    public sealed class SignInManager : SignInManager<User, int>, ISignInManager
    {
        #region Constructors

        public SignInManager(UserManager<User, int> userManager, IAuthenticationManager authenticationManager) : base(userManager, authenticationManager) { }

        #endregion Constructors

        #region Overridden Members

        public override async Task SignInAsync(User user, bool isPersistent, bool rememberBrowser)
        {
            this.AuthenticationManager.AllSignOut();

            var identity = await this.UserManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
            identity.AddUserClaims(user);

            if (rememberBrowser)
            {
                var userIdAsString = user.Id.ToString(CultureInfo.InvariantCulture);
                var rememberBrowserIdentity = this.AuthenticationManager.CreateTwoFactorRememberBrowserIdentity(userIdAsString);

                this.AuthenticationManager.SignIn(new AuthenticationProperties {IsPersistent = isPersistent}, identity, rememberBrowserIdentity);
            }
            else
            {
                this.AuthenticationManager.SignIn(new AuthenticationProperties {IsPersistent = isPersistent}, identity);
            }

            var applicationUserManager = this.UserManager as UserManager;
            if (applicationUserManager == null)
                return;

            await applicationUserManager.RecordLastLoginAtAsync(user);
        }

        #endregion Overridden Members

        #region IEnhancedSignIn Members

        public async Task<EnhancedSignInStatus> EnhancedPasswordSignInAsync(string email, string password, bool isPersistent, bool shouldLockout = true)
        {
            var applicationUserManager = this.UserManager as UserManager;
            if (applicationUserManager == null)
                return EnhancedSignInStatus.Failure;

            var user = await applicationUserManager.FindByEmailAsync(email);
            if (user == null)
                return EnhancedSignInStatus.Unknown;

            var userId = user.Id;

            if (await applicationUserManager.IsLockedOutAsync(userId))
                return EnhancedSignInStatus.LockedOut;

            if (await applicationUserManager.CheckPasswordAsync(user, password))
            {
                if (await applicationUserManager.IsDisabledAsync(userId))
                    return EnhancedSignInStatus.Disabled;

                switch (await this.SignInOrTwoFactor(user, isPersistent))
                {
                    case SignInStatus.Failure:
                        return EnhancedSignInStatus.Failure;
                    case SignInStatus.LockedOut:
                        return EnhancedSignInStatus.LockedOut;
                    case SignInStatus.RequiresVerification:
                        return EnhancedSignInStatus.RequiresVerification;
                    case SignInStatus.Success:
                    {
                        await this.UserManager.ResetAccessFailedCountAsync(userId);
                        return EnhancedSignInStatus.Success;
                    }
                }

            }
            else if (shouldLockout)
            {
                await this.UserManager.AccessFailedAsync(userId);

                if (await this.UserManager.IsLockedOutAsync(userId))
                    return EnhancedSignInStatus.LockedOut;
            }

            return EnhancedSignInStatus.Failure;
        }

        public async Task<EnhancedSignInStatus> EnhancedTwoFactorSignInAsync(string provider, string code, bool isPersistent, bool rememberBrowser)
        {
            var applicationUserManager = this.UserManager as UserManager;
            if (applicationUserManager == null)
                return EnhancedSignInStatus.Failure;

            var userId = await this.GetVerifiedUserIdAsync();
            if (userId < 1)
                return EnhancedSignInStatus.Failure;

            var user = await this.UserManager.FindByIdAsync(userId);
            if (user == null)
                return EnhancedSignInStatus.Unknown;

            if (await applicationUserManager.IsLockedOutAsync(userId))
                return EnhancedSignInStatus.LockedOut;            

            if (await applicationUserManager.VerifyTwoFactorTokenAsync(userId, provider, code))
            {
                if (await applicationUserManager.IsDisabledAsync(userId))
                    return EnhancedSignInStatus.Disabled;

                  await this.UserManager.ResetAccessFailedCountAsync(userId);
                  await this.SignInAsync(user, isPersistent, rememberBrowser);
                  return EnhancedSignInStatus.Success;            
            }
            
            await this.UserManager.AccessFailedAsync(user.Id);
            return EnhancedSignInStatus.Failure;
        }

        #endregion IEnhancedSignIn Members

        #region Private Members

        private async Task<SignInStatus> SignInOrTwoFactor(User user, bool isPersistent)
        {
            var userId = user.Id;
            
            var isTwoFactorEnabled = await this.UserManager.GetTwoFactorEnabledAsync(userId);
            if (isTwoFactorEnabled == false)
            {
                await this.SignInAsync(user, isPersistent, false);
                return SignInStatus.Success;            
            }

            var twoFactorProviders = await this.UserManager.GetValidTwoFactorProvidersAsync(userId);
            if (twoFactorProviders.Count <= 0)
            {
                await this.SignInAsync(user, isPersistent, false);
                return SignInStatus.Success;            
            }

            var userIdAsString = userId.ToString(CultureInfo.InvariantCulture);
            var twoFactorStatus = await this.AuthenticationManager.TwoFactorBrowserRememberedAsync(userIdAsString);
            if (twoFactorStatus)
            {
                await this.SignInAsync(user, isPersistent, false);
                return SignInStatus.Success;            
            }

            var claimsIdentity = new ClaimsIdentity("TwoFactorCookie");
            claimsIdentity.AddClaim(new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier", userIdAsString));
            this.AuthenticationManager.SignIn(new[] { claimsIdentity });
                
            return SignInStatus.RequiresVerification;
        }

        #endregion Private Members
    }
}