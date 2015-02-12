using System;
using System.Collections.Generic;
using MVC5Start.Models.Definitions;

namespace MVC5Start.Models.Identity
{
    /// <summary>
    /// Represents an user of the web application.
    /// POCO class imitating the Microsoft.AspNet.Identity.EntityFramework.IdentityUser and compatible with Asp.Net Identity 2.0.
    /// Compared to the original IdentityUser, it has the following extra properties: FirstName, LastName, IsDisabled, LastLogInAtUtc and timestamps for addition (AddedAtUtc) and modifications (ModifiedAtUtc).
    /// </summary>
    public class User : BaseIdAddedAtModifiedAtEntity
    {
        public User()
        {
            this.Claims = new List<UserClaim>();
            this.Roles = new List<UserRole>();
            this.Logins = new List<UserLogin>();
        }  

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string UserName { get; set; }

        public string PasswordHash { get; set; }

        public string SecurityStamp { get; set; }

        public string Email { get; set; }

        public bool EmailConfirmed { get; set; }

        public string PhoneNumber { get; set; }

        public bool PhoneNumberConfirmed { get; set; }

        public bool TwoFactorEnabled { get; set; }

        public DateTime? LockoutEndDateUtc { get; set; }

        public bool LockoutEnabled { get; set; }

        public int AccessFailedCount { get; set; }

        public DateTime? LastLogInAtUtc { get; set; }

        public virtual ICollection<UserRole> Roles { get; private set; }

        public virtual ICollection<UserClaim> Claims { get; private set; }

        public virtual ICollection<UserLogin> Logins { get; private set; }
    }
}