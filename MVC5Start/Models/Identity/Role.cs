using System.Collections.Generic;
using Microsoft.AspNet.Identity;
using MVC5Start.Models.Definitions;

namespace MVC5Start.Models.Identity
{
    /// <summary>
    /// Represents a role, defined by a Name and a Description, with an Int32 as primary key.
    /// POCO class imitating the Microsoft.AspNet.Identity.EntityFramework.IdentityRole and compatible with Asp.Net Identity 2.0.
    /// Compared to the original IdentityRole, it has an extra property called Description, and timestamps for addition (AddedAtUtc) and modifications (ModifiedAtUtc).
    /// </summary>
    public class Role : BaseIdAddedAtModifiedAtEntity, IRole<int>
    {
        public Role()
        {
            this.Users = new List<UserRole>();
        }

        /// <summary>
        /// The name of the Role.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The name of the Role.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The UserRoles associated with this Role.
        /// </summary>
        public virtual ICollection<UserRole> Users { get; private set; }
    }
}