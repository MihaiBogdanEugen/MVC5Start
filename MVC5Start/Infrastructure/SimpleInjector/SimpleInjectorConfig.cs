using System.Reflection;
using System.Web;
using System.Web.Mvc;
using Hangfire;
using MVC5Start.Infrastructure.Identity.Managers;
using MVC5Start.Infrastructure.Identity.Stores;
using MVC5Start.Infrastructure.Services;
using MVC5Start.Models.Identity;
using Microsoft.AspNet.Identity;
using SimpleInjector;
using SimpleInjector.Integration.Web.Mvc;

namespace MVC5Start.Infrastructure.SimpleInjector
{
    public static class SimpleInjectorConfig
    {
        public static void Configure()
        {
           var container = new Container();

            container.RegisterSingle(() => Startup.DataProtectionProvider);

            container.RegisterPerWebRequest<DbConnectionInfo, DbConnectionInfo>();
            container.RegisterPerWebRequest<IIdentityMessageService, AppEmailService>();
            container.RegisterPerWebRequest<IUserStore<User, int>, UserStore>();
            container.RegisterPerWebRequest<IUserManager, UserManager>();
            container.RegisterPerWebRequest<IRoleStore<Role, int>, RoleStore>();
            container.RegisterPerWebRequest<IRoleManager, RoleManager>();
            container.RegisterPerWebRequest<ISignInManager, SignInManager>();

            container.RegisterPerWebRequest(() => HttpContext.Current.GetOwinContext().Authentication);

            container.RegisterMvcControllers(Assembly.GetExecutingAssembly());
            container.RegisterMvcIntegratedFilterProvider();

            DependencyResolver.SetResolver(new SimpleInjectorDependencyResolver(container));

            JobActivator.Current = new SimpleInjectorJobActivator(container);
        }
    }
}