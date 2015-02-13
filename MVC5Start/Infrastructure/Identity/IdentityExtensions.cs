using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using MVC5Start.Models.Identity;

namespace MVC5Start.Infrastructure.Identity
{
    public static class IdentityExtensions
    {
        public static void AddUserClaims(this ClaimsIdentity identity, User user)
        {
            identity.AddClaim(new Claim(ClaimTypes.Surname, user.LastName));
            identity.AddClaim(new Claim(ClaimTypes.GivenName, user.FirstName));
            identity.AddClaim(new Claim(ClaimTypes.Email, user.Email));        
        }

        public static string GetFullName(this IPrincipal principal)
        {
            if (principal == null || 
                principal.Identity == null || 
                principal.Identity.IsAuthenticated == false)
                return Constants.Anonymous;

            var claimsIdentity = principal.Identity as ClaimsIdentity;
            if (claimsIdentity == null)
                return Constants.Anonymous;

            var lastNameClaim = claimsIdentity.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Surname);
            var lastName = lastNameClaim == null ? string.Empty : lastNameClaim.Value;

            var firstNameClaim = claimsIdentity.Claims.FirstOrDefault(x => x.Type == ClaimTypes.GivenName);
            var firstName = firstNameClaim == null ? string.Empty : firstNameClaim.Value;

            var fullName = (firstName + " " + lastName).Trim();
            return string.IsNullOrEmpty(fullName) ? Constants.Anonymous : fullName;
        }

        public static string GetRoles(this IPrincipal principal)
        {
            if (principal == null || 
                principal.Identity == null || 
                principal.Identity.IsAuthenticated == false)
                return string.Empty;

            var claimsIdentity = principal.Identity as ClaimsIdentity;
            if (claimsIdentity == null)
                return string.Empty;

            var values = claimsIdentity.Claims.Where(x => x.Type == ClaimTypes.Role).Select(x => x.Value);
            var roles = string.Join(", ", values).Trim();
            return roles;
        }

        public static string GetFullNameAndRoles(this IPrincipal principal)
        {
            var fullName = principal.GetFullName();
            var roles = principal.GetRoles();
            return string.IsNullOrEmpty(roles) ? fullName : string.Format(CultureInfo.InvariantCulture, "{0} ({1})", fullName, roles);
        }

        public static void AllSignOut(this IAuthenticationManager authenticationManager)
        {
            authenticationManager.SignOut(
                DefaultAuthenticationTypes.ApplicationCookie,
                DefaultAuthenticationTypes.ExternalBearer,
                DefaultAuthenticationTypes.ExternalCookie,
                DefaultAuthenticationTypes.TwoFactorCookie,
                DefaultAuthenticationTypes.TwoFactorRememberBrowserCookie);            
        }
    }
}