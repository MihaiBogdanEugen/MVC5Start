using System;
using System.Collections.Generic;
using System.Drawing.Design;
using Microsoft.AspNet.Identity;
using MVC5Start.Models.Definitions;

namespace MVC5Start.Models.Identity
{
    /// <summary>
    /// Represents an user of the web application.
    /// POCO class imitating the Microsoft.AspNet.Identity.EntityFramework.IdentityUser and compatible with Asp.Net Identity 2.0.
    /// Compared to the original IdentityUser, it has the following extra properties: FirstName, LastName, IsDisabled, LastLogInAtUtc and timestamps for addition (AddedAtUtc) and modifications (ModifiedAtUtc).
    /// </summary>
    public class User : BaseIdAddedAtModifiedAtEntity, IUser<int>
    {
        public User()
        {
            this.Claims = new List<UserClaim>();
            this.Roles = new List<UserRole>();
            this.Logins = new List<UserLogin>();
        }  

        /// <summary>
        /// The first name of the user.
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// The last name of the user.
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// The username of the user. 
        /// Usually, the same as the email.
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// The password hash of the user.
        /// </summary>
        public string PasswordHash { get; set; }

        /// <summary>
        /// The security timestamp introduced by the Identity engine of the user.
        /// </summary>
        public string SecurityStamp { get; set; }

        /// <summary>
        /// The email of the user.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Flag indicating if the email is confirmed or not.
        /// </summary>
        public bool EmailConfirmed { get; set; }

        /// <summary>
        /// The phone number of the user.
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Flag indicating if the phone number is confirmed or not.
        /// </summary>
        public bool PhoneNumberConfirmed { get; set; }

        /// <summary>
        /// Flag indicating if two factor authentication is enabled or not for the current user.
        /// </summary>
        public bool TwoFactorEnabled { get; set; }

        /// <summary>
        /// The secret key provided by Google Authenticator
        /// </summary>
        public string GoogleAuthenticatorSecretKey { get; set; }

        /// <summary>
        /// The password hash of the Two Factor authentication process
        /// </summary>
        public string TwoFactorAuthPasswordHash { get; set; }

        /// <summary>
        /// The UTC timestamp when the lockout will end.
        /// </summary>
        public DateTime? LockoutEndDateUtc { get; set; }

        /// <summary>
        /// Flag indicating if lockout is enabled or not.
        /// </summary>
        public bool LockoutEnabled { get; set; }

        /// <summary>
        /// Counter of failed login attempts.
        /// </summary>
        public int AccessFailedCount { get; set; }

        /// <summary>
        /// Timestamp of last successfull login.
        /// </summary>
        public DateTime? LastLogInAtUtc { get; set; }

        /// <summary>
        /// Flag indicating if the current user is disabled or not.
        /// </summary>
        public bool IsDisabled { get; set; }

        /// <summary>
        /// The list of UserRoles of the current user.
        /// </summary>
        public virtual ICollection<UserRole> Roles { get; private set; }

        /// <summary>
        /// The list of claims of the current user.
        /// </summary>
        public virtual ICollection<UserClaim> Claims { get; private set; }

        /// <summary>
        /// The list of logins of the current user.
        /// </summary>
        public virtual ICollection<UserLogin> Logins { get; private set; }
    }
}