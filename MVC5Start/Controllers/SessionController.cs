using System.Threading.Tasks;
using System.Web.Mvc;
using MVC5Start.Controllers.Definitions;
using MVC5Start.Infrastructure.Identity;
using MVC5Start.Infrastructure.Identity.Managers;
using MVC5Start.ViewModels.Session;

namespace MVC5Start.Controllers
{
    public class SessionController : BaseIdentityController
    {
        #region Constructors

        public SessionController(SignInManager signInManager, UserManager userManager) 
            : base(signInManager, userManager) { }

        #endregion Constructors

        #region Login

        [AllowAnonymous, HttpGet, Route("~/login", Name = "login")]
        public ActionResult Login(string returnUrl)
        {
            //TODO: if user is already authenticated, redirect to home page.

            return this.View(new LoginViewModel { ReturnUrl =  returnUrl});
        }

        [AllowAnonymous, HttpPost, Route("~/login"), ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model)
        {
            //TODO: if user is already authenticated, redirect to home page.

            if (this.ModelState.IsValid == false)
                return this.View(model);

            var emailInfo = await this.UserManager.FindInfoByEmailAsync(model.Email);
            if (emailInfo == null)
                return this.RedirectToInvalidModel(model, "Invalid username or password!");
            
            if (emailInfo.EmailConfirmed == false)
            {
                model.ResendVerificationCode = true;
                return this.RedirectToInvalidModel(model, "You must have a confirmed email to log on!");
            }

            var result = await this.SignInManager.EnhancedPasswordSignInAsync(model.Email, model.Password, model.RememberMe);
            switch (result)
            {
                case EnhancedSignInStatus.Success:
                    return this.RedirectToLocalUrl(model.ReturnUrl);
                case EnhancedSignInStatus.Disabled:
                    return this.View("Disabled");
                case EnhancedSignInStatus.LockedOut:
                    return this.View("LockedOut");
                case EnhancedSignInStatus.RequiresVerification:
                    return this.RedirectToAction("VerifyCode", new {model.ReturnUrl, model.RememberMe });
                default:
                    return this.RedirectToInvalidModel(model, "Invalid username or password!");
            }
        }

        #endregion Login

        #region Logout

        [Authorize, HttpPost, Route("~/logout", Name = "logout"), ValidateAntiForgeryToken]
        public ActionResult Logout()
        {
            this.AuthenticationManager.AllSignOut();

            return this.RedirectToHome;
        }

        #endregion Logout

        #region VerifyCode

        [HttpGet, Route("~/verify-code")]
        public async Task<ActionResult> VerifyCode(string returnUrl, bool? rememberMe)
        {
            var userId = await this.SignInManager.GetVerifiedUserIdAsync(); //TODO: find out why...
            if (userId < 1)
                this.RedirectToError("Invalid UserId: {0}", userId);

            var twoFactorProviders = await this.UserManager.GetValidTwoFactorProvidersAsync(userId);
            if (twoFactorProviders.Count != 1)
                this.RedirectToError("Unexpected no. of Two-Factor Authetication providers");

            // Require that the user has already logged in via username/password or external login
            var hasBeenVerified = await this.SignInManager.HasBeenVerifiedAsync();
            if (hasBeenVerified == false)
                return this.RedirectToError("The user is supposed to be verified but it's not");

            return this.View(new VerifyCodeViewModel
            {
                RememberMe = rememberMe.HasValue && rememberMe.Value,
                ReturnUrl = returnUrl,
                SelectedProvider = twoFactorProviders[0]
            });
        }

        [HttpPost, Route("~/verify-code"), ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (this.ModelState.IsValid == false)
                return this.View(model);

            var result = await this.SignInManager.EnhancedTwoFactorSignInAsync(model.SelectedProvider, model.Code,  model.RememberMe, model.RememberBrowser);
            switch (result)
            {
                case EnhancedSignInStatus.Success:
                    return this.RedirectToLocalUrl(model.ReturnUrl);
                case EnhancedSignInStatus.Disabled:
                    return this.View("Disabled");
                case EnhancedSignInStatus.LockedOut:
                    return this.View("LockedOut");
                default:
                    return this.RedirectToInvalidModel(model, "Invalid code!");
            }
        }

        #endregion VerifyCode
    }
}