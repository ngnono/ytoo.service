using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.intime.fashion.common.Aliyun
{
   public static  class Config
    {
       public static readonly string ACCESS_ID = ConfigurationManager.AppSettings["ALIYUN_ACCESS_ID"];
       public static readonly string ACCESS_KEY = ConfigurationManager.AppSettings["ALIYUN_ACCESS_KEY"];
       public static readonly string RSS_BUCKET_NAME = ConfigurationManager.AppSettings["ALIYUN_RSS_BUCKET_NAME"];
    }
}
