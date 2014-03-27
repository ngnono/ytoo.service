using System.Configuration;
using System.Linq;

namespace OPCApp.Infrastructure.Config
{
    internal class DefaultConfig : IConfig
    {
        public string GetValue(string key, string defaultValue = "")
        {
            if (ConfigurationManager.AppSettings.AllKeys.Contains(key))
            {
                return ConfigurationManager.AppSettings[key];
            }
            return defaultValue;
        }

        public string GetValue(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }
    }
}