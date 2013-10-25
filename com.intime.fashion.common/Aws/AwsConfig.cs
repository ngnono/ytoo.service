using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.intime.fashion.common.Aws
{
   public static class AwsConfig
    {
       public static readonly string PUBLIC_KEY = ConfigurationManager.AppSettings["aws_public_key"];
       public static readonly string PRIVATE_KEY = ConfigurationManager.AppSettings["aws_private_key"];
       public static readonly string BASE_URL = ConfigurationManager.AppSettings["aws_base_url"];
    }
}
