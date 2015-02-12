using MVC5Start.Models.Definitions;

namespace MVC5Start.Models.Identity
{
    /// <summary>
    /// Association between an User and a Role
    /// POCO class imitating the Microsoft.AspNet.Identity.EntityFramework.IdentityUserRole and compatible with Asp.Net Identity 2.0
    /// Compared to the original IdentityUserRole, it has two extras, an Int32 id property, and a AddedAtUtc time stamp property.
    /// </summary>
    public class UserRole : BaseIdAddedAtEntity
    {
        /// <summary>
        /// The Id of the User of this UserRole.
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// The Id of the Role of this UserRole.
        /// </summary>
        public int RoleId { get; set; }
    }
}