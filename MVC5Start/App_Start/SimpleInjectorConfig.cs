using System;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using Hangfire;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using MVC5Start.Infrastructure;
using MVC5Start.Infrastructure.Identity.Managers;
using MVC5Start.Infrastructure.Identity.Stores;
using MVC5Start.Infrastructure.Services;
using MVC5Start.Models.Identity;
using SimpleInjector;
using SimpleInjector.Integration.Web.Mvc;

namespace MVC5Start
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
            container.RegisterPerWebRequest<UserManager<User, int>, UserManager>();
            container.RegisterPerWebRequest<IRoleStore<Role, int>, RoleStore>();
            container.RegisterPerWebRequest<RoleManager<Role, int>, RoleManager>();
            container.RegisterPerWebRequest<SignInManager<User, int>, SignInManager>();

            container.RegisterPerWebRequest(() => HttpContext.Current.GetOwinContext().Authentication);

            container.RegisterMvcControllers(Assembly.GetExecutingAssembly());
            container.RegisterMvcIntegratedFilterProvider();

            DependencyResolver.SetResolver(new SimpleInjectorDependencyResolver(container));

            JobActivator.Current = new SimpleInjectorJobActivator(container);
        }

        public class SimpleInjectorJobActivator : JobActivator
        {
            private readonly Container _container;

            public SimpleInjectorJobActivator(Container container)
            {
                if (container == null)
                    throw new ArgumentNullException("container");

                this._container = container;
            }

            public override object ActivateJob(Type jobType)
            {
                return _container.GetInstance(jobType);
            }
        }
    }
}