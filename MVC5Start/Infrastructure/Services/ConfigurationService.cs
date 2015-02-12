using System;
using System.Collections.Generic;
using System.Configuration;

namespace MVC5Start.Infrastructure.Services
{
    public static class ConfigurationService
    {
        private static readonly Dictionary<string, string> ConfigurationSettings = new Dictionary<string, string>();
        private static readonly object Lock = new object();

        public static string DefaultConnectionString
        {
            get { return GetConnectionString("DefaultConnection"); }
        }

        public static string GetConnectionString(string connectionStringName)
        {
            if(string.IsNullOrEmpty(connectionStringName))
                throw new ArgumentNullException("connectionStringName");

            string connectionString;

            lock(Lock)
            {
                if (ConfigurationSettings.TryGetValue(connectionStringName, out connectionString))
                    return connectionString;

                connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
                if (ConfigurationSettings.ContainsKey(connectionStringName) == false)
                    ConfigurationSettings.Add(connectionStringName, connectionString);             
            }

            return connectionString;
        }

        public static string GetApplicationSetting(string key)
        {
            if(string.IsNullOrEmpty(key))
                throw new ArgumentNullException("key");

            string setting;

            lock(Lock)
            {
                if (ConfigurationSettings.TryGetValue(key, out setting))
                    return setting;

                setting = ConfigurationManager.AppSettings[key];
                if (ConfigurationSettings.ContainsKey(key) == false)
                    ConfigurationSettings.Add(key, setting);             
            }

            return setting;
        }
    }   
}