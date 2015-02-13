using System;
using System.Globalization;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using Base32;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using MVC5Start.Controllers.Definitions;
using MVC5Start.Infrastructure.Identity.Managers;
using MVC5Start.Infrastructure.Services;
using MVC5Start.Models.Identity;
using MVC5Start.ViewModels;
using MVC5Start.ViewModels.Account;
using MVC5Start.ViewModels.Session;
using Newtonsoft.Json;
using OtpSharp;
using Constants = MVC5Start.Infrastructure.Constants;

namespace MVC5Start.Controllers
{
    [RoutePrefix("account")]
    public class AccountController : BaseIdentityController
    {
        #region Constructors

        public AccountController(AppSignInManager signInManager, AppUserManager userManager) 
            : base(signInManager, userManager) { }

        #endregion Constructors

        #region Manage

        [Authorize, HttpGet, Route("manage")]
        public async Task<ActionResult> Manage(ManageViewModel model)
        {
            var personalInfo = await this.UserManager.GetUserPersonalInfoAsync(this.UserId);

            model.FirstName = personalInfo.FirstName;
            model.LastName = personalInfo.LastName;
            model.Roles = personalInfo.Roles;
            model.Email = personalInfo.Email;
            model.TwoFactorEnabled = personalInfo.TwoFactorEnabled;
            model.PhoneNumber = personalInfo.PhoneNumber;
            model.IsBrowserRemembered = await this.AuthenticationManager.TwoFactorBrowserRememberedAsync(this.UserIdAsString);

            return this.View(model);
        }

        #endregion Manage

        #region ChangePersonalInfo

        [Authorize, HttpGet, Route("change-personal-info")]
        public async Task<ActionResult> ChangePersonalInfo()
        {
            var model = await this.UserManager.GetUserPersonalInfoAsync(this.UserId);
            return this.View(model);
        }

        [Authorize, HttpPost, Route("change-personal-info"), ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePersonalInfo(PersonalInfoViewModel model)
        {
            if (this.ModelState.IsValid == false)
                return this.View(model);

            var result = await this.UserManager.SaveProfileInfoAsync(model, this.UserId);
            if (result == false)
                this.RedirectToError("Profile info save failed!");

            return this.RedirectToAction("Manage", new ManageViewModel { Message = "Your profile has been saved!" });
        }

        #endregion ChangePersonalInfo

        #region ChangePersonalSettings

        [Authorize, HttpGet, Route("change-personal-settings")]
        public async Task<ActionResult> ChangePersonalSettings()
        {
            var model = await this.UserManager.GetUserPersonalSettingsAsync(this.UserId);

            this.ViewBag.DateFormats = this.GetSelectListDateFormats(model.DateFormat);
            this.ViewBag.Languages = this.GetSelectListLanguages(model.LanguageCode);
            this.ViewBag.TimeFormats = this.GetSelectListTimeFormats(model.TimeFormat);
            this.ViewBag.TimeZones = this.GetSelectListTimeZones(model.TimeZoneCode);

            return this.View(model);
        }

        [Authorize, HttpPost, Route("change-personal-settings"), ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePersonalSettings(PersonalSettingsViewModel model)
        {
            if (this.ModelState.IsValid == false)
            {
                this.ViewBag.DateFormats = this.GetSelectListDateFormats(model.DateFormat);
                this.ViewBag.Languages = this.GetSelectListLanguages(model.LanguageCode);
                this.ViewBag.TimeFormats = this.GetSelectListTimeFormats(model.TimeFormat);
                this.ViewBag.TimeZones = this.GetSelectListTimeZones(model.TimeZoneCode);

                return this.View(model);                
            }
            
            var result = await this.UserManager.SavePersonalSettingsAsync(model, this.UserId);
            if (result == false)
                this.RedirectToError("Personal settings save failed!");

            return this.RedirectToAction("Manage", new ManageViewModel { Message = "Your personal settings has been saved!" });
        }

        #endregion ChangePersonalSettings

        #region ChangePassword

        [Authorize, HttpGet, Route("change-password")]
        public ActionResult ChangePassword()
        {
            return this.View();
        }

        [Authorize, HttpPost, Route("change-password"), ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (this.ModelState.IsValid == false)
                return this.View(model);

            var user = await this.UserManager.FindByIdAsync(this.UserId);
            if (user == null)
                return this.RedirectToError("Invalid UserId: {0}", this.UserId);

            var result = await UserManager.ChangePasswordAsync(this.UserId, model.OldPassword, model.NewPassword);
            if (result.Succeeded == false)
                return this.RedirectToInvalidModel(model, result.Errors);

            await this.SignInManager.SignInAsync(user, false, false);

            return this.RedirectToAction("Manage", new ManageViewModel { Message = "Your password has been changed!" });
        }

        #endregion ChangePassword

        #region TwoFactorAuthentication

        [Authorize, HttpPost, Route("disable-google-authenticator"), ValidateAntiForgeryToken]
        public async Task<ActionResult> DisableGoogleAuthenticator()
        {
            var user = await this.UserManager.FindByIdAsync(this.UserId);
            if (user == null)
                return this.RedirectToError("Invalid UserId: {0}", this.UserId);

            await this.UserManager.SetTwoFactorEnabledAsync(this.UserId, false);

            user.GoogleAuthenticatorSecretKey = null;
            await this.UserManager.UpdateAsync(user);

            await this.SignInManager.SignInAsync(user, false, false);

            return this.RedirectToAction("Manage", new ManageViewModel
            {
                Message = "Your two-factor authentication provider has been disabled!"
            });
        }

        [Authorize, HttpGet, Route("enable-google-authenticator")]
        public ActionResult EnableGoogleAuthenticator()
        {
            var secretKey = KeyGeneration.GenerateRandomKey(20);
            var userName = this.User.Identity.GetUserName();
            var barcodeUrl = KeyUrl.GetTotpUrl(secretKey, userName) + "&issuer=" + Constants.ApplicationName;

            var disableTwoFactorAuthPassword = Guid.NewGuid().ToString();

            var model = new GoogleAuthenticatorViewModel
            {
                SecretKey = Base32Encoder.Encode(secretKey),
                //BarcodeUrl = HttpUtility.UrlEncode(barcodeUrl)
                BarcodeUrl = barcodeUrl,
                DisableTwoFactorAuthPassword = disableTwoFactorAuthPassword
            };

            return this.View(model); 
        }

        [Authorize, HttpPost, Route("enable-google-authenticator"), ValidateAntiForgeryToken]
        public async Task<ActionResult> EnableGoogleAuthenticator(GoogleAuthenticatorViewModel model)
        {
            if (this.ModelState.IsValid == false)
                return this.View(model);

            long timeStepMatched;
            var totp = new Totp(Base32Encoder.Decode(model.SecretKey));
            var isValid = totp.VerifyTotp(model.Code, out timeStepMatched, new VerificationWindow(2, 2));

            if (isValid == false)
            {
                this.ModelState.AddModelError("Code", "The Code is not valid");
                return this.View(model);
            }

            var user = await this.UserManager.FindByIdAsync(this.UserId);
            if (user == null)
                return this.RedirectToError("Invalid UserId: {0}", this.UserId);

            await this.UserManager.SetTwoFactorEnabledAsync(this.UserId, true);

            user.GoogleAuthenticatorSecretKey = model.SecretKey;
            user.TwoFactorAuthPasswordHash = this.UserManager.PasswordHasher.HashPassword(model.DisableTwoFactorAuthPassword);
            await this.UserManager.UpdateAsync(user);

            await this.SignInManager.SignInAsync(user, false, false);
    
            return this.RedirectToAction("Manage", new ManageViewModel
            {
                Message = "Your two-factor authentication provider has been enabled!"
            });
        }

        #endregion TwoFactorAuthentication

        #region ForgotPassword

        [AllowAnonymous, HttpGet, Route("~/forgot-password")]
        public ActionResult ForgotPassword()
        {
            return this.View();
        }

        [AllowAnonymous, HttpPost, Route("~/forgot-password"), ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (this.ModelState.IsValid == false)
                return this.View(model);

            var infoViewModel = new InformationViewModel { Message = "Please check your email to reset your password!" };

            var data = await this.UserManager.FindInfoByEmailAsync(model.Email);
            if (data.Id < 1 || data.EmailConfirmed == false)
                return this.View("Information", infoViewModel); // Don't reveal that the user does not exist

            var callbackUrl = await this.SendPasswordResetToken(data.Id);
#if DEBUG
            infoViewModel.CallbackUrl = callbackUrl;
#endif
            return this.View("Information", infoViewModel);
        }       

        #endregion ForgotPassword

        #region ResetPassword

        [AllowAnonymous, HttpGet, Route("~/reset-password{code:maxlength(1)}")]
        public ActionResult ResetPassword(string code)
        {
            //TODO: if user is already authenticated, redirect to home page.

            return string.IsNullOrEmpty(code) ? this.RedirectToError("Empty code when resetting password") : this.View();
        }

        [AllowAnonymous, HttpPost, Route("~/reset-password"), ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            //TODO: if user is already authenticated, redirect to home page.

            if (this.ModelState.IsValid == false)
                return this.View(model);

            var loginUrl = this.GetUrl("Login", "Session");
            var infoViewModel = new InformationViewModel
            {
                Message = "Your password has been reset!",
                LoginUrl = loginUrl
            };
            
            var user = await this.UserManager.FindByNameAsync(model.Email);
            if (user == null)
                return this.View("Information", infoViewModel); // Don't reveal that the user does not exist

            var result = await this.UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            return result.Succeeded 
                ? this.View("Information", infoViewModel) 
                : this.RedirectToInvalidModel(model, result.Errors);
        }

        #endregion ResetPassword

        #region Register

        [AllowAnonymous, HttpGet, Route("~/register", Name = "register")]
        public ActionResult Register()
        {
            //TODO: if user is already authenticated, redirect to home page.

            return this.View();
        }

        [AllowAnonymous, HttpPost, Route("~/register"), ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            //TODO: if user is already authenticated, redirect to home page.

            if (this.ModelState.IsValid == false)
                return this.View(model);

            var reCaptchaResponse = this.Request["g-recaptcha-response"];
            if (string.IsNullOrEmpty(reCaptchaResponse) || this.VerifyReCaptchaCode(reCaptchaResponse) == false)
                return this.RedirectToInvalidModel(model, "Invalid reCaptcha code");

            var user = new User
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                UserName = model.Email, 
                Email = model.Email,
            };

            var result = await this.UserManager.CreateAsync(user, model.Password, Constants.DefaultUserRoleName);
            if (result.Succeeded == false)
                return this.RedirectToInvalidModel(model, result.Errors);

            return await this.SendEmailConfirmationTokenAsync(user.Id);
        }

        #endregion Register

        #region ResendConfirmationEmail

        [AllowAnonymous, HttpGet, Route("~/resend-confirmation-email")]
        public ActionResult ResendConfirmationEmail()
        {
            //TODO: if user is already authenticated, redirect to home page.

            return this.View();
        }

        [AllowAnonymous, HttpPost, Route("~/resend-confirmation-email"), ValidateAntiForgeryToken]
        public async Task<ActionResult> ResendConfirmationEmail(ResendConfirmationEmailViewModel model)
        {
            //TODO: if user is already authenticated, redirect to home page.

            if (this.ModelState.IsValid == false)
                return this.View(model);

            var userId = await this.UserManager.FindIdByEmailAsync(model.Email);
            if (userId < 1)
                return this.View("Information", new InformationViewModel { Message = "Please check your email and confirm your account!" }); // Don't reveal that the user does not exist

            return await this.SendEmailConfirmationTokenAsync(userId);
        }

        #endregion ResendConfirmationEmail

        #region ConfirmEmail

        [Authorize, HttpGet, Route("~/confirm-email")]
        public async Task<ActionResult> ConfirmEmail(int? userId, string code)
        {
            //TODO: if user is already authenticated, redirect to home page.

            if (userId.HasValue == false || userId.Value < 1 || string.IsNullOrEmpty(code))
                return this.RedirectToError("Invalid user id or code when confirming email");

            var result = await this.UserManager.ConfirmEmailAsync(userId.Value, code);
            return result.Succeeded ? this.View("ConfirmEmail") : this.RedirectToError("Wrong code for confirming email");
        }

        #endregion ConfirmEmail

        #region DisableTwoFactorAuthentication

        [Authorize, HttpGet, Route("disable-two-factor-auth")]
        public async Task<ActionResult> DisableTwoFactorAuthentication()
        {
            var userId = await this.SignInManager.GetVerifiedUserIdAsync(); //TODO: find out why...
            if (userId < 1)
                this.RedirectToError("The user is supposed to be verified but it's not");

            return this.View();
        }

        [Authorize, HttpPost, Route("disable-two-factor-auth"), ValidateAntiForgeryToken]
        public async Task<ActionResult> DisableTwoFactorAuthentication(DisableTwoFactorAuthenticationViewModel model)
        {
            if (this.ModelState.IsValid == false)
                return this.View(model);

            var userId = await this.SignInManager.GetVerifiedUserIdAsync(); //TODO: find out why...
            if (userId < 1)
                this.RedirectToError("The user is supposed to be verified but it's not");

            await this.UserManager.DisableTwoFactorAuthentication(model.Code, userId);

            return this.View("Information", new InformationViewModel
            {
                Message = "Two-Factor Authentication has been disabled!",
                LoginUrl = this.GetUrl("Login", "Session")
            });
        }

        #endregion DisableTwoFactorAuthentication

        #region Private Members

        private bool VerifyReCaptchaCode(string response)
        {
            var remoteip = this.Request.UserHostAddress;
            var secret = ConfigurationService.GetApplicationSetting("reCAPTCHA:SecretKey");

            var uri = string.Format(CultureInfo.InvariantCulture,
                @"https://www.google.com/recaptcha/api/siteverify?secret={0}&response={1}&remoteip={2}",
                secret, response, remoteip);

            string googleRecaptchaResponseAsString;
            using (var client = new WebClient())
            {
                googleRecaptchaResponseAsString = client.DownloadString(new Uri(uri));
            }

            var result = JsonConvert.DeserializeObject<GoogleReCaptchaResponse>(googleRecaptchaResponseAsString);

            return result.Success;
        }

        private async Task<ActionResult> SendEmailConfirmationTokenAsync(int userId)
        {
            var infoViewModel = new InformationViewModel { Message = "Please check your email and confirm your account!" };

            var code = await this.UserManager.GenerateEmailConfirmationTokenAsync(userId);

            //var callbackUrl = this.GetCallbackUrl("ConfirmEmail", "Session", userId, code);
            var callbackUrl = this.Request.Url == null
                ? this.Url.Action("ConfirmEmail", "Account", new {userId, code})
                : this.Url.Action("ConfirmEmail", "Account", new {userId, code}, this.Request.Url.Scheme);

            var body = string.Format(CultureInfo.InvariantCulture, "In order to use the MyDocShelf application, please verify your account first by clicking <a href=\"{0}\">here</a>.", callbackUrl);

            this.UserManager.SendConfirmationEmail(userId, callbackUrl, Constants.ConfirmationEmailSubject, body);
#if DEBUG
            infoViewModel.CallbackUrl = callbackUrl;
#endif
            return this.View("Information", infoViewModel);        
        }

        private async Task<string> SendPasswordResetToken(int userId)
        {
            var code = await this.UserManager.GeneratePasswordResetTokenAsync(userId);

            //var callbackUrl = this.GetCallbackUrl("ResetPassword", "Session", userId, code);
            var callbackUrl = this.Request.Url == null
                ? this.Url.Action("ResetPassword", "Account", new {userId, code})
                : this.Url.Action("ResetPassword", "Account", new {userId, code}, this.Request.Url.Scheme);

            var body = string.Format(CultureInfo.InvariantCulture, "In order to reset your MyDocShelf account password, please click <a href=\"{0}\">here</a>.", callbackUrl);

            this.UserManager.SendPasswordResetEmail(userId, callbackUrl, Constants.PasswordResetEmailSubject, body);

            return callbackUrl;
        }

        #endregion Private Members
    }
}