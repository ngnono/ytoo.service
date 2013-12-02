using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.intime.fashion.common.Erp2
{
    public static class Erp2Config
    {
        public static string PUBLIC_KEY = ConfigurationManager.AppSettings["Erp2_Public_Key"];
        public static string Private_KEY = ConfigurationManager.AppSettings["Erp2_Private_Key"];
        public static string BASE_URL = ConfigurationManager.AppSettings["Erp2_Base_Url"];
    }
}
