using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Yintai.Hangzhou.Cms.WebSiteCoreV1.Models;

namespace Yintai.Hangzhou.Cms.WebSiteCoreV1.Util
{
    public static class EnumHelper
    {
        public static string ToFriendlyString(this ProUploadStatus proStatus)
        {
            FieldInfo EnumInfo = proStatus.GetType().GetField(proStatus.ToString());
            DescriptionAttribute[] EnumAttributes = (DescriptionAttribute[])EnumInfo.
                GetCustomAttributes(typeof(DescriptionAttribute), false);
            if (EnumAttributes.Length > 0)
            {
                return EnumAttributes[0].Description;
            }
            return proStatus.ToString();
        }
    }
}
