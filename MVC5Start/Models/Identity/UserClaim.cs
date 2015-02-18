using MVC5Start.Models.Definitions;
using MVC5Start.Models.Validation;

namespace MVC5Start.Models.Identity
{
    /// <summary>
    /// Association between an User and a claim
    /// POCO class imitating the Microsoft.AspNet.Identity.EntityFramework.IdentityUserClaim and compatible with Asp.Net Identity 2.0
    /// Compared to the original IdentityUserClaim, it has Int32 id property.
    /// </summary>
    public class UserClaim : BaseIdAddedAtEntity
    {
        /// <summary>
        /// The Id of the User of this claim.
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// The claim type.
        /// </summary>
        public string ClaimType { get; set; }

        /// <summary>
        /// The claim value.
        /// </summary>
        public string ClaimValue { get; set; }

        public override ValidationResult IsValid()
        {
            return ValidationResult.Success;
        }
    }
}