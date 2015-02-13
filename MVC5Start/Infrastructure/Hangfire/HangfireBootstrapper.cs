using System.Web.Hosting;
using Hangfire;
using Hangfire.SqlServer;

namespace MVC5Start.Infrastructure.Hangfire
{
    public sealed class HangfireBootstrapper : IRegisteredObject
    {
        public static readonly HangfireBootstrapper Instance = new HangfireBootstrapper();

        private readonly object _lockObject = new object();
        private bool _started;
        private BackgroundJobServer _backgroundJobServer;

        public void Start()
        {
            lock (this._lockObject)
            {
                if (this._started) 
                    return;

                this._started = true;

                HostingEnvironment.RegisterObject(this);

                JobStorage.Current = new SqlServerStorage(Constants.DefaultConnection);

                GlobalJobFilters.Filters.Add(new LogHangfireFailureAttribute());

                this._backgroundJobServer = new BackgroundJobServer();
                this._backgroundJobServer.Start();
            }
        }

        public void Stop()
        {
            lock (this._lockObject)
            {
                if (this._backgroundJobServer != null)
                    this._backgroundJobServer.Dispose();

                HostingEnvironment.UnregisterObject(this);
            }
        }

        void IRegisteredObject.Stop(bool immediate)
        {
            this.Stop();
        }
    }
}