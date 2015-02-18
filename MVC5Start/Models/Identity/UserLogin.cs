using MVC5Start.Models.Definitions;
using MVC5Start.Models.Validation;

namespace MVC5Start.Models.Identity
{
    /// <summary>
    /// Association between an User and an external login provider.
    /// POCO class imitating the Microsoft.AspNet.Identity.EntityFramework.IdentityUserLogin and compatible with Asp.Net Identity 2.0.
    /// Compared to the original IdentityUserLogin, it has two extras, an Int32 id property, and a AddedAtUtc time stamp property.
    /// </summary>
    public class UserLogin : BaseIdAddedAtEntity
    {
        /// <summary>
        /// The Id of the User of this login.
        /// </summary>
        public int UserId { get; set; }        

        /// <summary>
        /// The provider of this login.
        /// </summary>
        public string LoginProvider { get; set; }

        /// <summary>
        /// The key of the provider of this login.
        /// </summary>
        public string ProviderKey { get; set; }

        public override ValidationResult IsValid()
        {
            return ValidationResult.Success;
        }
    }
}