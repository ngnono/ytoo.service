using Intime.OPC.WebApi.Core.MessageHandlers;
using System;
using System.Configuration;

namespace Intime.OPC.WebApi.Core.Security
{
    public class AppSecurityManager : IAppSecurityManager
    {
        public string GetSecretKey(string appKey)
        {
            if (string.IsNullOrEmpty(appKey))
            {
                return String.Empty;
            }

            var configKey = string.Format("AppSecurityManager.{0}:SecretKey", appKey);

            var key = ConfigurationManager.AppSettings[configKey];

            if (string.IsNullOrEmpty(key))
            {
                return string.Empty;
            }

            return key; ;
        }

        public bool Enabled
        {
            get
            {
                var enable = ConfigurationManager.AppSettings["AppSecurityManager:Enabled"];

                if (enable == null)
                {
                    return true;
                }

                bool result = false;

                bool.TryParse(enable, out result);

                return result;
            }
        }
    }
}
