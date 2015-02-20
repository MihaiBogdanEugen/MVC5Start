using Hangfire;
using Hangfire.Dashboard;
using Hangfire.SqlServer;
using Microsoft.Owin;
using MVC5Start;
using MVC5Start.Infrastructure;
using MVC5Start.Infrastructure.Hangfire;
using Owin;

[assembly: OwinStartupAttribute(typeof(Startup))]
namespace MVC5Start
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder application)
        {
            ConfigureAuthentication(application);

            application.UseHangfire(configuration =>
            {
                configuration.UseAuthorizationFilters(new AuthorizationFilter { Roles = Constants.DefaultAdministratorRoleName });
                configuration.UseSqlServerStorage(Constants.DefaultConnection);
                configuration.UseServer();

                GlobalJobFilters.Filters.Add(new LogHangfireFailureAttribute());
            });
        }
    }
}
