using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Yintai.Hangzhou.Cms.WebSiteV1.Util
{
    public static class BooleanExtension
    {
        public static string ToReportString(this bool? yesOrNo)
        {
            if (!yesOrNo.HasValue)
                return "否";
            return yesOrNo.Value.ToReportString();
        }
        public static string ToReportString(this bool yesOrNo)
        {
            if (yesOrNo == true)
                return "是";
            return "否";
        }
    }
}