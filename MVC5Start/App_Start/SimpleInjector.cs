using System.Reflection;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using MVC5Start.Infrastructure.Identity.Managers;
using MVC5Start.Infrastructure.Identity.Stores;
using MVC5Start.Models.Identity;
using SimpleInjector;
using SimpleInjector.Integration.Web.Mvc;

namespace MVC5Start
{
    public static class SimpleInjector
    {
        public static void Configure()
        {
            var container = new Container();

            container.RegisterSingle(() => Startup.DataProtectionProvider);

            container.RegisterPerWebRequest<IUserStore<User, int>, UserStore>();
            container.RegisterPerWebRequest<UserManager<User, int>, ApplicationUserManager>();

            container.RegisterPerWebRequest<IRoleStore<Role, int>, RoleStore>();
            container.RegisterPerWebRequest<RoleManager<Role, int>, ApplicationRoleManager>();

            container.RegisterPerWebRequest<SignInManager<User, int>, ApplicationSignInManager>();

            container.RegisterPerWebRequest(() => HttpContext.Current.GetOwinContext().Authentication);

            container.RegisterMvcControllers(Assembly.GetExecutingAssembly());
            container.RegisterMvcIntegratedFilterProvider();

            DependencyResolver.SetResolver(new SimpleInjectorDependencyResolver(container));
        }
    }
}