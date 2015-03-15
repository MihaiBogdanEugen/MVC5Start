using System;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Threading.Tasks;
using Hangfire;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security.DataProtection;
using MVC5Start.Infrastructure.Identity.Stores;
using MVC5Start.Infrastructure.Services;
using MVC5Start.Models.Identity;
using MVC5Start.ViewModels.Account;
using MVC5Start.ViewModels.Queries;

namespace MVC5Start.Infrastructure.Identity.Managers
{
    public sealed class UserManager : UserManager<User, int>, IUserManager
    {
        #region Constructors

        public UserManager(DbConnectionInfo dbConnectionInfo, IIdentityMessageService emailService, IDataProtectionProvider dataProtectionProvider) 
            : base(new UserStore(dbConnectionInfo))
        {
            #region User Configuration

            this.UserValidator = new UserValidator<User, int>(this)
            {
                AllowOnlyAlphanumericUserNames = Constants.UserAllowOnlyAlphanumericUserNames,
                RequireUniqueEmail = Constants.UserRequireUniqueEmail
            };

            this.UserLockoutEnabledByDefault = Constants.UserLockoutEnabled;
            this.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(Constants.UserAccountLockoutMinutes);
            this.MaxFailedAccessAttemptsBeforeLockout = Constants.UserMaxFailedAccessAttemptsBeforeLockout;

            #endregion User Configuration

            #region Password Configuration

            this.PasswordValidator = new PasswordValidator
            {
                RequiredLength = Constants.PasswordRequiredLength,
                RequireNonLetterOrDigit = Constants.PasswordRequireNonLetterOrDigit,
                RequireDigit = Constants.PasswordRequireDigit,
                RequireLowercase = Constants.PasswordRequireLowercase,
                RequireUppercase = Constants.PasswordRequireUppercase,
            };

            this.PasswordHasher = new AdaptivePasswordHasher();

            #endregion Password Configuration
            
            this.EmailService = emailService;

            this.RegisterTwoFactorProvider("GoogleAuthenticator", new GoogleAuthenticatorTokenProvider());

            if (dataProtectionProvider != null)
            {
                this.UserTokenProvider = new DataProtectorTokenProvider<User, int>(dataProtectionProvider.Create("MyDocShelfId"))
                {
                    TokenLifespan = TimeSpan.FromHours(24),
                };
            }
        }

        #endregion Constructors

        #region Overriden Members

        public async Task<IdentityResult> CreateAsync(User user, string password, params string[] roles)
        {
            var result = await this.CreateAsync(user, password);
            if (result.Succeeded == false)
                return result;

            return await base.AddToRolesAsync(user.Id, roles);
        }

        public new async Task<IdentityResult> CreateAsync(User user, string password)
        {   
            if (user == null)
                throw new ArgumentNullException("user");
            
            if (string.IsNullOrEmpty(password))
                throw new ArgumentNullException("password");

            var result = User.IsValidPassword(password);
            if (result.Succeeded == false)
                return result.IdentityResult;

            user.PasswordHash = this.PasswordHasher.HashPassword(password);
            user.SecurityStamp = Guid.NewGuid().ToString();

            if (this.UserLockoutEnabledByDefault)
                user.LockoutEnabled = true;

            var validationResult = user.IsValid();
            if (validationResult.Succeeded == false)
                return validationResult.IdentityResult;

            await this.Store.CreateAsync(user);
                
            return IdentityResult.Success;   
        }

        public override async Task<IdentityResult> UpdateAsync(User user)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            var result = user.IsValid();
            if (result.Succeeded == false)
                return result.IdentityResult;

            await Store.UpdateAsync(user);

            return IdentityResult.Success;
        }

        #endregion Overriden Members

        #region Notification Members

        public void SendConfirmationEmail(int userId, string callBackUrl, string subject, string body)
        {
            Expression<Action> sendEmail;
            var applicationEmailService = this.EmailService as AppEmailService;
            if (applicationEmailService == null)
            {
                var destination = this.GetEmail(userId);
                var message = new IdentityMessage { Body = body, Destination = destination, Subject = subject };
                sendEmail = () => this.EmailService.Send(message);
            }
            else
                sendEmail = () => applicationEmailService.SendConfirmationEmail(userId, callBackUrl);

            BackgroundJob.Enqueue(sendEmail);
        }

        public void SendPasswordResetEmail(int userId, string callBackUrl, string subject, string body)
        {
            Expression<Action> sendEmail;
            var applicationEmailService = this.EmailService as AppEmailService;
            if (applicationEmailService == null)
            {
                var destination = this.GetEmail(userId);
                var message = new IdentityMessage { Body = body, Destination = destination, Subject = subject };
                sendEmail = () => this.EmailService.Send(message);
            }
            else
                sendEmail = () => applicationEmailService.SendPasswordResetEmail(userId, callBackUrl);

            BackgroundJob.Enqueue(sendEmail);
        }

        #endregion Notification Members

        #region Public Members

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(User user)
        {
            var identity = await this.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
            identity.AddUserClaims(user);
            return identity;
        }

        public async Task RecordLastLoginAtAsync(User user)
        {
            var applicationUserStore = this.Store as UserStore;
            if (applicationUserStore == null)
                return;

            await applicationUserStore.RecordLastLoginAtAsync(user);
        }

        public async Task<bool> IsDisabledAsync(int userId)
        {
            var applicationUserStore = this.Store as UserStore;
            if (applicationUserStore == null)
                return false;

            return await applicationUserStore.IsDisabledAsync(userId);
        }

        public async Task<int> FindIdByEmailAsync(string email)
        {
            if (string.IsNullOrEmpty(email))
                return 0;

            var applicationUserStore = this.Store as UserStore;
            if (applicationUserStore == null)
                return 0;

            return await applicationUserStore.FindIdByEmailAsync(email);
        }

        public async Task<UserEmailInfo> FindInfoByEmailAsync(string email)
        {
            if (string.IsNullOrEmpty(email))
                return null;

            var applicationUserStore = this.Store as UserStore;
            if (applicationUserStore == null)
                return null;

            return await applicationUserStore.FindInfoByEmailAsync(email);
        }

        public async Task<PersonalInfoViewModel> GetUserPersonalInfoAsync(int userId)
        {
            var applicationUserStore = this.Store as UserStore;
            if (applicationUserStore == null)
                return null;

            return await applicationUserStore.GetUserPersonalInfoAsync(userId);            
        }
        
        public async Task<bool> SaveProfileInfoAsync(PersonalInfoViewModel model, int userId)
        {
            var applicationUserStore = this.Store as UserStore;
            if (applicationUserStore == null)
                return false;

            return (await applicationUserStore.SaveProfileInfoAsync(model, userId)) == 1;            
        }

        public async Task<bool> DisableTwoFactorAuthentication(string code, int userId)
        {
            var applicationUserStore = this.Store as UserStore;
            if (applicationUserStore == null)
                return false;

            var passwordForTwoFactorAuthHash = await applicationUserStore.GetTwoFactorAuthPasswordHash(userId);
            var result = this.PasswordHasher.VerifyHashedPassword(passwordForTwoFactorAuthHash, code);
            if (result != PasswordVerificationResult.Success)
                return false;

            await applicationUserStore.DisableTwoFactorAuthenticationAsync(userId);
            return true;
        }

        public async Task<PersonalSettingsViewModel> GetUserPersonalSettingsAsync(int userId)
        {
            var applicationUserStore = this.Store as UserStore;
            if (applicationUserStore == null)
                return null;

            return await applicationUserStore.GetUserPersonalSettingsAsync(userId);
        }

        public async Task<bool> SavePersonalSettingsAsync(PersonalSettingsViewModel model, int userId)
        {
            var applicationUserStore = this.Store as UserStore;
            if (applicationUserStore == null)
                return false;

            return (await applicationUserStore.SavePersonalSettingsAsync(model, userId)) == 1;            
        }

        #endregion Public Members

        #region Private Members

        private IUserPasswordStore<User, int> GetPasswordStore()
        {
            var userPasswordStore = this.Store as IUserPasswordStore<User, int>;
            if (userPasswordStore == null)
                throw new NotSupportedException("Store does not implement IUserPasswordStore");
            return userPasswordStore;
        }

        private IUserSecurityStampStore<User, int> GetSecurityStore()
        {
            var securityStampStore = this.Store as IUserSecurityStampStore<User, int>;
            if (securityStampStore == null)
                throw new NotSupportedException("Store does not implement IUserSecurityStampStore");
            return securityStampStore;
        }
        
        private IUserLockoutStore<User, int> GetUserLockoutStore()
        {
            var userLockoutStore = this.Store as IUserLockoutStore<User, int>;
            if (userLockoutStore == null)
                throw new NotSupportedException("Store does not implement IUserLockoutStore");
            return userLockoutStore;
        }

        #endregion Private Members
    }
}