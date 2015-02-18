using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNet.Identity;
using MVC5Start.Models.Identity;
using MVC5Start.ViewModels.Account;
using MVC5Start.ViewModels.Queries;

namespace MVC5Start.Infrastructure.Identity.Stores
{
    public sealed class UserStore : BaseStore, 
        IUserLoginStore<User, int>, 
        IUserClaimStore<User, int>, 
        IUserRoleStore<User, int>, 
        IUserPasswordStore<User, int>, 
        IUserSecurityStampStore<User, int>, 
        IQueryableUserStore<User, int>, 
        IUserEmailStore<User, int>, 
        IUserPhoneNumberStore<User, int>, 
        IUserTwoFactorStore<User, int>, 
        IUserLockoutStore<User, int>,
        IUserDisabledStore<User, int>,
        IUserLastLoginStore<User, int>
    {
        #region Constructors

        public UserStore(DbConnectionInfo dbConnectionInfo) : base(dbConnectionInfo) { }

        #endregion Constructors

        #region IUserStore Members

        public async Task CreateAsync(User user)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            await this.Connection.ExecuteAsync(Sql.Users.Insert, new
            {
                user.FirstName,
                user.LastName,
                user.UserName,
                user.PasswordHash,
                user.SecurityStamp,
                user.Email,
                user.EmailConfirmed,
                user.PhoneNumber,
                user.PhoneNumberConfirmed,
                user.TwoFactorEnabled,
                user.LockoutEndDateUtc,
                user.LockoutEnabled,
                user.AccessFailedCount,
                user.LastLogInAtUtc,
                user.IsDisabled,
            });
        }

        public async Task UpdateAsync(User user)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            await this.Connection.ExecuteAsync(Sql.Users.Update, new
            {
                user.FirstName,
                user.LastName,
                user.UserName,
                user.PasswordHash,
                user.SecurityStamp,
                user.Email,
                user.EmailConfirmed,
                user.PhoneNumber,
                user.PhoneNumberConfirmed,
                user.TwoFactorEnabled,
                user.LockoutEndDateUtc,
                user.LockoutEnabled,
                user.AccessFailedCount,
                user.LastLogInAtUtc,
                user.IsDisabled,
                user.Id
            });
        }

        public async Task DeleteAsync(User user)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            using(var transaction = this.Connection.BeginTransaction())
            {
                var success = true;
                try
                {
                    await this.Connection.ExecuteAsync(Sql.UserClaims.DeleteByUserId, new {UserId = user.Id});

                    await this.Connection.ExecuteAsync(Sql.UserLogins.DeleteByUserId, new {UserId = user.Id});

                    await this.Connection.ExecuteAsync(Sql.UserRoles.DeleteByUserId, new {UserId = user.Id});

                    await this.Connection.ExecuteAsync(Sql.Users.Delete, new {user.Id}, transaction);
                }
                catch
                {
                    success = false;
                }

                if (success)
                    transaction.Commit();
                else
                    transaction.Rollback();
            }
        }

        public async Task<User> FindByIdAsync(int userId)
        {
            return (await this.Connection.QueryAsync<User>(Sql.Users.FindById, new { Id = userId })).FirstOrDefault();
        }

        public async Task<User> FindByNameAsync(string userName)
        {
            if (string.IsNullOrEmpty(userName))
                throw new ArgumentNullException("userName");

            return (await this.Connection.QueryAsync<User>(Sql.Users.FindByName, new { UserName = userName})).FirstOrDefault();
        }

        #endregion IUserStore Members

        #region IUserLoginStore Members

        public async Task AddLoginAsync(User user, UserLoginInfo login)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            if (login == null)
                throw new ArgumentNullException("login");

            await this.Connection.QueryAsync<int?>(Sql.UserLogins.Insert, new { UserId = user.Id, login.LoginProvider, login.ProviderKey });
        }

        public async Task RemoveLoginAsync(User user, UserLoginInfo login)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            if (login == null)
                throw new ArgumentNullException("login");

            await this.Connection.ExecuteAsync(Sql.UserLogins.Delete, new { UserId = user.Id, login.LoginProvider, login.ProviderKey });
        }

        public async Task<IList<UserLoginInfo>> GetLoginsAsync(User user)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            return (await this.Connection.QueryAsync<UserLoginInfo>(Sql.UserLogins.SelectByUserId, new { UserId = user.Id })).ToList();
        }

        public async Task<User> FindAsync(UserLoginInfo login)
        {
            if (login == null)
                throw new ArgumentNullException("login");

            return (await this.Connection.QueryAsync<User>(Sql.Users.SelectByLogin, new { login.LoginProvider, login.ProviderKey })).FirstOrDefault();
        }

        #endregion IUserLoginStore Members

        #region IUserClaimStore Members

        public async Task<IList<Claim>> GetClaimsAsync(User user)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            return (await this.Connection.QueryAsync<UserClaim>(Sql.UserClaims.SelectByUserId, new {UserId = user.Id}))
                .Select(x => new Claim(x.ClaimType, x.ClaimValue)).ToList();
        }

        public async Task AddClaimAsync(User user, Claim claim)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            if (claim == null)
                throw new ArgumentNullException("claim");

            await this.Connection.QueryAsync<int?>(Sql.UserClaims.Insert, new { UserId = user.Id, ClaimType = claim.Type, ClaimValue = claim.Value });
        }

        public async Task RemoveClaimAsync(User user, Claim claim)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            if (claim == null)
                throw new ArgumentNullException("claim");

            await this.Connection.ExecuteAsync(Sql.UserClaims.Delete, new { UserId = user.Id, ClaimType = claim.Type, ClaimValue = claim.Value });
        }

        #endregion IUserClaimStore Members

        #region IUserRoleStore Members

        public async Task AddToRoleAsync(User user, string roleName)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            if (string.IsNullOrEmpty(roleName))
                throw new ArgumentNullException("roleName");

            await this.Connection.ExecuteAsync(Sql.UserRoles.AddToRole, new {UserId = user.Id, RoleName = roleName});
        }

        public async Task RemoveFromRoleAsync(User user, string roleName)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            if (string.IsNullOrEmpty(roleName))
                throw new ArgumentNullException("roleName");

            await this.Connection.ExecuteAsync(Sql.UserRoles.RemoveFromRole, new {UserId = user.Id, RoleName = roleName});
        }

        public async Task<IList<string>> GetRolesAsync(User user)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            return (await this.Connection.QueryAsync<string>(Sql.UserRoles.GetRoles, new { UserId = user.Id })).ToList();
        }

        public async Task<bool> IsInRoleAsync(User user, string roleName)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            if (string.IsNullOrEmpty(roleName))
                throw new ArgumentNullException("roleName");

            var record = (await this.Connection.QueryAsync<UserRole>(Sql.UserRoles.IsInRole, new {UserId = user.Id, RoleName = roleName})).FirstOrDefault();

            return record != null && record.Id > 0;
        }

        #endregion IUserRoleStore Members

        #region IUserPasswordStore Members

        public async Task SetPasswordHashAsync(User user, string passwordHash)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            if (string.IsNullOrEmpty(passwordHash))
                throw new ArgumentNullException("passwordHash");

            await this.Connection.ExecuteAsync(Sql.Users.SetPasswordHash, new { UserId = user.Id, PasswordHash = passwordHash });
        }

        public async Task<string> GetPasswordHashAsync(User user)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            return (await this.Connection.ExecuteScalarAsync<string>(Sql.Users.GetPasswordHash, new { UserId = user.Id }));
        }

        public async Task<bool> HasPasswordAsync(User user)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            var record = (await this.Connection.ExecuteScalarAsync<string>(Sql.Users.GetPasswordHash, new { UserId = user.Id }));

            return string.IsNullOrEmpty(record) == false;
        }

        #endregion IUserPasswordStore Members

        #region IUserSecurityStampStore Members

        public async Task SetSecurityStampAsync(User user, string stamp)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            if (string.IsNullOrEmpty(stamp))
                throw new ArgumentNullException("stamp");

            await this.Connection.ExecuteAsync(Sql.Users.SetSecurityStamp, new { UserId = user.Id, SecurityStamp = stamp });
        }

        public async Task<string> GetSecurityStampAsync(User user)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            return (await this.Connection.ExecuteScalarAsync<string>(Sql.Users.GetSecurityStamp, new { UserId = user.Id }));
        }

        #endregion IUserSecurityStampStore Members

        #region IQueryableUserStore Members

        public IQueryable<User> Users
        {
            get
            {
                return this.Connection.Query<User>(Sql.Users.Select).AsQueryable();
            }
        }

        #endregion IQueryableUserStore Members

        #region IUserEmailStore Members

        public async Task SetEmailAsync(User user, string email)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            if (string.IsNullOrEmpty(email))
                throw new ArgumentNullException("email");

            await this.Connection.ExecuteAsync(Sql.Users.SetEmail, new { UserId = user.Id, Email = email });
        }

        public async Task<string> GetEmailAsync(User user)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            return (await this.Connection.ExecuteScalarAsync<string>(Sql.Users.GetEmail, new { UserId = user.Id }));
        }

        public async Task<bool> GetEmailConfirmedAsync(User user)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            return (await this.Connection.ExecuteScalarAsync<bool>(Sql.Users.GetEmailConfirmed, new { UserId = user.Id }));
        }

        public async Task SetEmailConfirmedAsync(User user, bool confirmed)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            await this.Connection.ExecuteAsync(Sql.Users.SetEmailConfirmed, new { UserId = user.Id, EmailConfirmed = confirmed });
        }

        public async Task<User> FindByEmailAsync(string email)
        {
            if (string.IsNullOrEmpty(email))
                throw new ArgumentNullException("email");

            return (await this.Connection.QueryAsync<User>(Sql.Users.FindByEmail, new { Email = email})).FirstOrDefault();
        }

        #endregion IUserEmailStore Members

        #region IUserPhoneNumberStore Members

        public async Task SetPhoneNumberAsync(User user, string phoneNumber)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            if (string.IsNullOrEmpty(phoneNumber))
                throw new ArgumentNullException("phoneNumber");

            await this.Connection.ExecuteAsync(Sql.Users.SetPhoneNumber, new { UserId = user.Id, PhoneNumber = phoneNumber });
        }

        public async Task<string> GetPhoneNumberAsync(User user)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            return (await this.Connection.ExecuteScalarAsync<string>(Sql.Users.GetPhoneNumber, new { UserId = user.Id }));
        }

        public async Task<bool> GetPhoneNumberConfirmedAsync(User user)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            return (await this.Connection.ExecuteScalarAsync<bool>(Sql.Users.GetPhoneNumberConfirmed, new { UserId = user.Id }));
        }

        public async Task SetPhoneNumberConfirmedAsync(User user, bool confirmed)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            await this.Connection.ExecuteAsync(Sql.Users.SetPhoneNumberConfirmed, new { UserId = user.Id, PhoneNumberConfirmed = confirmed });
        }

        #endregion IUserPhoneNumberStore Members

        #region IUserTwoFactorStore Members

        public async Task SetTwoFactorEnabledAsync(User user, bool enabled)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            await this.Connection.ExecuteAsync(Sql.Users.SetTwoFactorEnabled, new { UserId = user.Id, TwoFactorEnabled = enabled });
        }

        public async Task<bool> GetTwoFactorEnabledAsync(User user)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            return (await this.Connection.ExecuteScalarAsync<bool>(Sql.Users.GetTwoFactorEnabled, new { UserId = user.Id }));
        }

        #endregion IUserTwoFactorStore Members

        #region IUserLockoutStore Members

        public async Task<DateTimeOffset> GetLockoutEndDateAsync(User user)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            var record = (await this.Connection.ExecuteScalarAsync<DateTime?>(Sql.Users.GetLockoutEndDateUtc, new { UserId = user.Id }));

            return record.HasValue 
                ? new DateTimeOffset(DateTime.SpecifyKind(record.Value, DateTimeKind.Utc)) 
                : new DateTimeOffset();
        }

        public async Task SetLockoutEndDateAsync(User user, DateTimeOffset lockoutEnd)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            await this.Connection.ExecuteAsync(Sql.Users.SetLockoutEndDateUtc, new
            {
                UserId = user.Id, 
                LockoutEndDateUtc = lockoutEnd == DateTimeOffset.MinValue 
                ? new DateTime?() 
                : lockoutEnd.UtcDateTime
            });
        }

        public async Task<int> IncrementAccessFailedCountAsync(User user)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            var result = 0;
            using (var transaction = this.Connection.BeginTransaction())
            {
                var success = true;
                try
                {
                    await this.Connection.ExecuteAsync(Sql.Users.IncrementAccessFailedCount, new { user.Id }, transaction);

                    result = await this.Connection.ExecuteScalarAsync<int>(Sql.Users.GetAccessFailedCount, new { UserId = user.Id }, transaction);
                }
                catch
                {
                    success = false;
                }

                if (success)
                    transaction.Commit();
                else
                    transaction.Rollback();
            }

            return result;
        }

        public async Task ResetAccessFailedCountAsync(User user)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            await this.Connection.ExecuteAsync(Sql.Users.ResetAccessFailedCount, new { user.Id });
        }

        public async Task<int> GetAccessFailedCountAsync(User user)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            return await this.Connection.ExecuteScalarAsync<int>(Sql.Users.GetAccessFailedCount, new { UserId = user.Id });
        }

        public async Task<bool> GetLockoutEnabledAsync(User user)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            return (await this.Connection.ExecuteScalarAsync<bool>(Sql.Users.GetLockoutEnabled, new { UserId = user.Id }));
        }

        public async Task SetLockoutEnabledAsync(User user, bool enabled)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            await this.Connection.ExecuteAsync(Sql.Users.SetLockoutEnabled, new { UserId = user.Id, LockoutEnabled = enabled });
        }

        #endregion IUserLockoutStore Members

        #region IUserDisabledStore Members

        public async Task<bool> IsDisabledAsync(User user)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            return await this.Connection.ExecuteScalarAsync<bool>(Sql.Users.IsDisabled, new {UserId = user.Id});
        }

        public async Task DisableAsync(User user)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            await this.Connection.ExecuteAsync(Sql.Users.Disable, new {UserId = user.Id});
        }

        public async Task EnableAsync(User user)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            await this.Connection.ExecuteAsync(Sql.Users.Enable, new {UserId = user.Id});
        }

        #endregion IUserDisabledStore Members

        #region IUserLastLoginStore Members

        public async Task RecordLastLoginAtAsync(User user)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            await this.Connection.ExecuteAsync(Sql.Users.RecordLastLoginAt, new {UserId = user.Id});
        }

        #endregion IUserLastLoginStore Members

        #region Public Members

        public async Task<bool> IsDisabledAsync(int userId)
        {
            return await this.Connection.ExecuteScalarAsync<bool>(Sql.Users.IsDisabled, new {UserId = userId});
        }
        
        public async Task<int> FindIdByEmailAsync(string email)
        {
            return (await this.Connection.ExecuteScalarAsync<int>(Sql.Users.FindIdByEmail, new { Email = email }));
        }

        public async Task<UserEmailInfo> FindInfoByEmailAsync(string email)
        {
            return (await this.Connection.QueryAsync<UserEmailInfo>(Sql.Users.GetEmailInfoByEmail, new { Email = email })).FirstOrDefault();
        }

        public async Task<UserEmailInfo> FindInfoByIdAsync(int userId)
        {
            return (await this.Connection.QueryAsync<UserEmailInfo>(Sql.Users.GetEmailInfoById, new { Id = userId })).FirstOrDefault();
        }

        public async Task<PersonalInfoViewModel> GetUserPersonalInfoAsync(int userId)
        {
            var model = (await this.Connection.QueryAsync<PersonalInfoViewModel>(Sql.Users.GetPersonalInfoById, new { Id = userId })).FirstOrDefault();
            if (model == null) 
                return null;

            var values = await this.Connection.QueryAsync<string>(Sql.UserRoles.GetRoles, new {UserId = userId});
            model.Roles = string.Join(", ", values);

            return model;
        }

        public async Task<int> SaveProfileInfoAsync(PersonalInfoViewModel model, int userId)
        {
            return await this.Connection.ExecuteAsync(Sql.Users.SavePersonalInfo, new {model.FirstName, model.LastName, model.PhoneNumber, Id = userId});
        }

        public async Task<int> DisableTwoFactorAuthenticationAsync(int userId)
        {
            return await this.Connection.ExecuteAsync(Sql.Users.DisableTwoFactorAuthSettings, new {Id = userId});
        }

        public async Task<string> GetTwoFactorAuthPasswordHash(int userId)
        {
            return (await this.Connection.QueryAsync<string>(Sql.Users.GetTwoFactorAuthPasswordHash, new { Id = userId })).FirstOrDefault();
        }

        public async Task<PersonalSettingsViewModel> GetUserPersonalSettingsAsync(int userId)
        {
            return (await this.Connection.QueryAsync<PersonalSettingsViewModel>(Sql.Users.GetPersonalSettings, new { Id = userId })).FirstOrDefault();
        }

        public async Task<int> SavePersonalSettingsAsync(PersonalSettingsViewModel model, int userId)
        {
            return await this.Connection.ExecuteAsync(Sql.Users.SavePersonalSettings, new {model.DateFormat, model.LanguageCode, model.TimeFormat, model.TimeZoneCode, Id = userId});
        }

        #endregion Public Members
    }
}