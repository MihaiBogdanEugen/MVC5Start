using System.IO;
using System.Net;
using System.Web.Mvc;

namespace MVC5Start.Infrastructure.Identity
{
    public sealed class EnhancedAuthorizeAttribute : AuthorizeAttribute
    {
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            var response = filterContext.HttpContext.Response;
            if (response != null)
            {
                response.Clear();
                response.StatusCode = (int) HttpStatusCode.Unauthorized;

                var server = filterContext.HttpContext.Server;
                if (server != null)
                {
                    var filePath = server.MapPath("~/Pages/401.html");
                    if (File.Exists(filePath))
                    {
                        response.ContentType = "text/html; charset=utf-8";
                        response.WriteFile(filePath);
                    }
                }                

                response.End();
                return;
            }
            
            base.HandleUnauthorizedRequest(filterContext);
        }
    }
}