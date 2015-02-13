using System.Globalization;
using System.Web;
using Microsoft.AspNet.Identity;

namespace MVC5Start.Infrastructure
{
    public static class ExtensionMethods
    {
        public static string GetUserId(this HttpContext context)
        {
            return context == null
                ? "unknown" : HttpContext.Current.User == null
                ? "unknown" : HttpContext.Current.User.Identity == null
                ? "unknown" : HttpContext.Current.User.Identity.IsAuthenticated
                ? HttpContext.Current.User.Identity.GetUserId<int>().ToString(CultureInfo.InvariantCulture) : "anonymous";
        }
    }
}