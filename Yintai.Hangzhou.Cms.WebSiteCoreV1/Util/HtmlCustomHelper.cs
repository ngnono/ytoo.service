using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Yintai.Hangzhou.Cms.WebSiteCoreV1.Util
{
    public static class HtmlCustomHelper
    {
        public static string ClientId<TModel>(this HtmlHelper helper, Expression<Func<TModel, TModel>> ex)
        {
            return helper.ViewData.TemplateInfo.GetFullHtmlFieldId(ExpressionHelper.GetExpressionText(ex));
        }
    }
    public static class JsonResultHelper
    {
        public static JsonResult EnsureContentType(this JsonResult helper,HttpRequestBase request)
        {
            try
            {
                if (request["HTTP_ACCEPT"].Contains("application/json"))
                    helper.ContentType = "application/json";
                else
                    helper.ContentType = "text/plain";
            }
            catch
            {
                helper.ContentType = "text/plain";
            }
            return helper;
        }
    }
}
