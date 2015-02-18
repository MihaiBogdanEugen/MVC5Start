using System;
using System.Data.SqlClient;
using MVC5Start.Infrastructure.Services;

namespace MVC5Start.Infrastructure
{
    public class DbConnectionInfo
    {
        public DbConnectionInfo() : this(ConfigurationService.DefaultConnectionString, true)
        {
        
        }

        private DbConnectionInfo(string connectionString, bool enableMars = false)
        {
            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentNullException("connectionString");

            if (enableMars)
            {
                var builder = new SqlConnectionStringBuilder(connectionString)
                {
                    MultipleActiveResultSets = true
                };

                connectionString = builder.ConnectionString;
            }

            this.ConnectionString = connectionString;
        }

        public string ConnectionString { get; private set; }
    }
}