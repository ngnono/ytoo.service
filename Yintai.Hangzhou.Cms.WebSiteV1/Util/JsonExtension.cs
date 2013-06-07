using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Yintai.Hangzhou.Cms.WebSiteV1.Util
{
    public static class JsonExtension
    {
        public static string ToJson(this object o)
        {
            return JsonConvert.SerializeObject(o);
        }
    }
}