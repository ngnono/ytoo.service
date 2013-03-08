using System;
using System.Configuration;
using System.Linq;

namespace Yintai.Hangzhou.Service.Manager
{
    public partial class ConfigManager
    {
        private const string _AppleAppId = "appleappid";
        private const string _ResourceDomain = "resourcedomain";
        private const string _Domain = "domain";
        private const string _GroupCradKey = "jtapis_key";
        private const string _GroupCradInfoUrl = "jtapis_cardinfo";
        private const string _GroupCradQueryPointUrl = "jtapis_cardscore";

        private static readonly string _domainPath = AppDomain.CurrentDomain.BaseDirectory;
        private static readonly string _imageUpload = ConfigurationManager.AppSettings["resourceimagedomain"];
        private static readonly string _soundUpload = ConfigurationManager.AppSettings["resourcesounddomain"];
        private static readonly string _videoUpload = ConfigurationManager.AppSettings["resourcevideodomain"];
        private static readonly string _defUpload = ConfigurationManager.AppSettings["resourcedefdomain"];

        private static readonly string _resourcedomain = ConfigurationManager.AppSettings["resourcedomain"];

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

        public static string GetGroupCardKey()
        {
            return GetAppConfigParamsValue(_GroupCradKey);
        }

        public static string GetGroupCardInfoUrl()
        {
            return GetAppConfigParamsValue(_GroupCradInfoUrl);
        }

        public static string GetGroupCardQueryScoreUrl()
        {
            return GetAppConfigParamsValue(_GroupCradQueryPointUrl);
        }
    }
}
