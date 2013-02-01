using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using Yintai.Hangzhou.Model.Enums;

namespace Yintai.Hangzhou.WebSupport.Helper
{
    public static class EnumExtension
    {
        public static SelectList ToSelectList<TEnum>(this TEnum enumObj, int selected)
        {
            try
            {
                var values = from TEnum e in Enum.GetValues(typeof(TEnum))
                             select
                                 new SelectListItem { Value = (Convert.ToInt32(e).ToString(CultureInfo.InvariantCulture)), Text = e.ToString() };
                return new SelectList(values, "Value", "Text", selected);
            }
            catch
            {
                return new SelectList(new List<string> { "转换枚举类型到列表时出现错误" }, selected);
            }
        }//用的时候       @Html.DropDownListFor(model => model.ExpertIsWork, Consts.YesNo.是.ToSelectList(0))


        public static SelectList ToSelectList<TEnum>(this TEnum enumObj)
        {
            try
            {
                var name = Architecture.Framework.Extension.EnumExtension.GetEnumName(typeof(TEnum), enumObj);


                var values = from TEnum e in Enum.GetValues(typeof(TEnum))
                             select
                                 new SelectListItem { Value = (Convert.ToInt32(e).ToString(CultureInfo.InvariantCulture)), Text = e.ToString(), Selected = e.ToString() == name };

                return new SelectList(values, "Value", "Text");
            }
            catch
            {
                return new SelectList(new List<string> { "转换枚举类型到列表时出现错误" });
            }
        }//用的时候       @Html.DropDownListFor(model => model.ExpertIsWork, Consts.YesNo.是.ToSelectList(0))
    }


}
