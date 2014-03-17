using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OPCApp.Infrastructure.Config
{
    class DefaultConfig:IConfig
    {
        public string GetValue(string key)
        {
            return System.Configuration.ConfigurationManager.AppSettings[key];
        }

        public string GetValue(string key, string defaultValue = "")
        {
            if (System.Configuration.ConfigurationManager.AppSettings.AllKeys.Contains(key))
            {
                return System.Configuration.ConfigurationManager.AppSettings[key];
            }
            else
            {
                return defaultValue;
            }
        }
    }
}
