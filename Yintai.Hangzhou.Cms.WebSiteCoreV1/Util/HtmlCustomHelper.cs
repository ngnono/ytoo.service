using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
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
}
