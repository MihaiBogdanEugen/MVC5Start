using System;
using System.Web;
using System.Web.Mvc;
using Microsoft.Owin.Security;
using MVC5Start.Infrastructure.Identity.Managers;

namespace MVC5Start.Controllers.Definitions
{
    public abstract class BaseIdentityController : BaseController
    {
        protected BaseIdentityController(SignInManager signInManager, UserManager userManager)
        {
            if (signInManager == null)
                throw new ArgumentNullException("signInManager");

            if (userManager == null)
                throw new ArgumentNullException("userManager");

            this.SignInManager = signInManager;
            this.UserManager = userManager;
        }

        protected SignInManager SignInManager { get; private set; }

        protected UserManager UserManager { get; private set; }

        protected IAuthenticationManager AuthenticationManager
        {
            get { return this.HttpContext.GetOwinContext().Authentication; }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.SignInManager != null)
                {
                    this.SignInManager.Dispose();
                    this.SignInManager = null;
                }

                if (this.UserManager != null)
                {
                    this.UserManager.Dispose();
                    this.UserManager = null;
                }
            }

            base.Dispose(disposing);
        }
    }
}