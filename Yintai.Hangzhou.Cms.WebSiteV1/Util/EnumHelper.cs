using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Yintai.Hangzhou.Data.Models;
namespace Yintai.Hangzhou.Cms.WebSiteV1.Util
{
    public static class EnumHelper
    {
        public static string ToFriendlyString(this Enum proStatus)
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

        public static IEnumerable<SelectListItem> DisplayList<T>() where T:struct
        {
            if (!typeof(T).IsEnum) yield break;
            yield return new SelectListItem
            {
                Text="请选择"
                , Value=""
            };
            foreach(var value in Enum.GetValues(typeof(T)))
            {
                DescriptionAttribute DesAttr =typeof(T).GetField(Enum.GetName(typeof(T),value)).GetCustomAttribute(typeof(DescriptionAttribute)) as DescriptionAttribute;
                yield return new SelectListItem{
                    Text = DesAttr.Description
                    , Value = ((int)value).ToString()
                };
            }
           

        }
    }
}
