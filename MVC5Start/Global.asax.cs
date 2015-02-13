using System;
using System.Configuration;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using MVC5Start.Infrastructure;
using MVC5Start.Infrastructure.Hangfire;
using StackExchange.Exceptional;
using StackExchange.Profiling;

namespace MVC5Start
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start(object sender, EventArgs args)
        {
            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(new RazorViewEngine());
            MvcHandler.DisableMvcResponseHeader = true;

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            SimpleInjectorConfig.Configure();
            HangfireBootstrapper.Instance.Start();

            MiniProfiler.Settings.Results_Authorize = IsUserAllowedToSeeMiniProfilerDashboard;
            MiniProfiler.Settings.Results_List_Authorize = IsUserAllowedToSeeMiniProfilerDashboard;

            ErrorStore.GetCustomData = (exception, context, data) =>
            {
                data.Add("UserId", HttpContext.Current.GetUserId());
            };
            ErrorStore.jQueryURL = "/Scripts/jquery-2.1.3.min.js";
            ErrorStore.AddJSInclude("~/Scripts/errors.js");
            ErrorStore.OnBeforeLog += (senderObj, argsObj) => { };
            ErrorStore.OnAfterLog += (senderObj, argsObj) => { };
        }

        protected void Application_End(object sender, EventArgs e)
        {
            HangfireBootstrapper.Instance.Stop();
        }

        protected void Application_BeginRequest(object sender, EventArgs args)
        {
            MiniProfiler.Start();
        }

        protected void Application_EndRequest(object sender, EventArgs args)
        {
            MiniProfiler.Stop();

//            var application = sender as HttpApplication;
//            if (application == null)
//                return;
//
//            application.Response.SuppressFormsAuthenticationRedirect = true;
//            application.Response.End();
//
//            if (application.Request.IsAuthenticated == false || 
//                application.Response.StatusCode != 401) 
//                return;
//
//            var customErrors = (CustomErrorsSection)ConfigurationManager.GetSection("system.web/customErrors");
//
//            var accessDeniedPath = customErrors.Errors["401"] != null ? customErrors.Errors["401"].Redirect : customErrors.DefaultRedirect;
//            if (string.IsNullOrEmpty(accessDeniedPath))
//                return; // Let other code handle it (probably IIS).
//
//            application.Response.ClearContent();
//            application.Server.Execute(accessDeniedPath);
//            HttpContext.Current.Server.ClearError();
        }

        protected void Application_AuthenticateRequest(Object sender, EventArgs e)
        {
            if (IsUserAllowedToSeeMiniProfilerDashboard(this.Request) == false)
            {
                MiniProfiler.Stop(true);
            }
        }

        public static void LogException(Exception exception)
        {
            if (exception == null)
                return;

            ErrorStore.LogException(exception, HttpContext.Current);
        }

        private static bool IsUserAllowedToSeeMiniProfilerDashboard(HttpRequest request)
        {
            if (request == null)
                return false;

            var httpContext = request.RequestContext.HttpContext;
            if (httpContext.User == null || httpContext.User.Identity == null || httpContext.User.Identity.IsAuthenticated == false)
                return false;

            return request.RequestContext.HttpContext.User.IsInRole(Constants.DefaultAdministratorRoleName);   
        }
    }
}
