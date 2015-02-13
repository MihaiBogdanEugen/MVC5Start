using System.Web.Hosting;
using MVC5Start.Infrastructure.Hangfire;

namespace MVC5Start
{
    public sealed class ApplicationPreload : IProcessHostPreloadClient
    {
        public void Preload(string[] parameters)
        {
            HangfireBootstrapper.Instance.Start();
        }
    }
}