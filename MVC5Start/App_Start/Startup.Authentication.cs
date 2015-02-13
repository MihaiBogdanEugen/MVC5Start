using System;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.DataProtection;
using Owin;
using MVC5Start.Infrastructure.Identity.Managers;
using MVC5Start.Models.Identity;

namespace MVC5Start
{
    public partial class Startup
    {
        internal static IDataProtectionProvider DataProtectionProvider { get; private set; }

        private static void ConfigureAuthentication(IAppBuilder application)
        {
            Startup.DataProtectionProvider = application.GetDataProtectionProvider();

            application.CreatePerOwinContext(() => DependencyResolver.Current.GetService<AppUserManager>());

            application.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/login"),
                LogoutPath = new PathString("/logout"),
                CookieName = "MyDocShelfCookie",
                Provider = DefaultCookieAuthenticationProvider,
                //CookieSecure = CookieSecureOption.Always,
                //CookieHttpOnly = true,
            });            

            application.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            application.UseTwoFactorSignInCookie(DefaultAuthenticationTypes.TwoFactorCookie, TimeSpan.FromMinutes(5));

            application.UseTwoFactorRememberBrowserCookie(DefaultAuthenticationTypes.TwoFactorRememberBrowserCookie);
        }

        private static CookieAuthenticationProvider DefaultCookieAuthenticationProvider
        {
            get
            {
                return new CookieAuthenticationProvider
                {
                    OnValidateIdentity =
                        SecurityStampValidator.OnValidateIdentity<AppUserManager, User, int>(
                            validateInterval: TimeSpan.FromMinutes(30),
                            regenerateIdentityCallback: (manager, user) => manager.GenerateUserIdentityAsync(user),
                            getUserIdCallback: (claim) => int.Parse(claim.GetUserId()))
                };
            }
        }
    }
}