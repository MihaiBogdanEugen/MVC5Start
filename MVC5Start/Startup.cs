using Microsoft.Owin;
using MVC5Start;
using Owin;

[assembly: OwinStartupAttribute(typeof(Startup))]
namespace MVC5Start
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder application)
        {
            ConfigureAuthentication(application);
        }
    }
}
