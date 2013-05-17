using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yintai.Hangzhou.Cms.WebSiteCoreV1.Util
{
    public static class DateTimeExtension
    {
        public static string ToReportFormat(this DateTime? date)
        {
            if (!date.HasValue)
                return "不限";
            return date.Value.ToString("yyyy-MM-dd HH:mm");
        }
    }
}
