using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace Yintai.Hangzhou.Service.Manager
{
    public partial class ConfigManager
    {
        private const string _AppleAppId = "appleappid";
        private const string _ResourceDomain = "resourcedomain";
        private const string _Domain = "domain";


        private static readonly string _domainPath = AppDomain.CurrentDomain.BaseDirectory;
        private static readonly string _imageUpload = System.Configuration.ConfigurationManager.AppSettings["resourceimagedomain"];
        private static readonly string _soundUpload = System.Configuration.ConfigurationManager.AppSettings["resourcesounddomain"];
        private static readonly string _videoUpload = System.Configuration.ConfigurationManager.AppSettings["resourcevideodomain"];
        private static readonly string _defUpload = System.Configuration.ConfigurationManager.AppSettings["resourcedefdomain"];

        private static readonly string _resourcedomain = System.Configuration.ConfigurationManager.AppSettings["resourcedomain"];

        /// <summary>
        /// image
        /// </summary>
        /// <returns></returns>
        public static string GetHttpApiImagePath()
        {
            return _resourcedomain + _imageUpload;
            //return _domain + "api/" + _imageUpload;
        }

        /// <summary>
        /// sound
        /// </summary>
        /// <returns></returns>
        public static string GetHttpApiSoundPath()
        {
            return _resourcedomain + _soundUpload;
            //return _domain + "api/" + _imageUpload;
        }

        /// <summary>
        /// video
        /// </summary>
        /// <returns></returns>
        public static string GetHttpApivideoPath()
        {
            return _resourcedomain + _videoUpload;
            //return _domain + "api/" + _imageUpload;
        }

        /// <summary>
        /// def
        /// </summary>
        /// <returns></returns>
        public static string GetHttpApidefPath()
        {
            return _resourcedomain + _defUpload;
            //return _domain + "api/" + _imageUpload;
        }

        private ConfigManager()
        {
        }

        #region methods

        private static string GetAppConfigParamsValue(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }

        private static object GetParamsObjectValue(string key)
        {
            if (ConfigurationManager.AppSettings.AllKeys.Contains(key))
            {
                return GetAppConfigParamsValue(key);
            }

            return null;
        }

        private static T GetParamsValueOrDefault<T>(string key, T defaultValue)
        {
            var t = GetParamsObjectValue(key);

            if (t == null)
            {
                return defaultValue;
            }

            return (T)t;
        }

        #endregion

        /// <summary>
        /// 获取苹果 appid
        /// </summary>
        /// <returns></returns>
        public static string GetAppleAppId()
        {
            return GetAppConfigParamsValue(_AppleAppId);
        }

        /// <summary>
        /// 获取 域
        /// </summary>
        public static string Domain
        {
            get { return GetAppConfigParamsValue(_Domain); }
        }
    }
}
