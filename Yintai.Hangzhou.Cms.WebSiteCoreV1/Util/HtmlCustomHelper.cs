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
    public static class HtmlHelperFactoryExtensions
    {

        public static HtmlHelper<TModel> HtmlHelperFor<TModel>(this HtmlHelper htmlHelper)
        {
            return HtmlHelperFor(htmlHelper, default(TModel));
        }

        public static HtmlHelper<TModel> HtmlHelperFor<TModel>(this HtmlHelper htmlHelper, TModel model)
        {
            return HtmlHelperFor(htmlHelper, model, null);
        }

        public static HtmlHelper<TModel> HtmlHelperFor<TModel>(this HtmlHelper htmlHelper, TModel model, string htmlFieldPrefix)
        {

            var viewDataContainer = CreateViewDataContainer(htmlHelper.ViewData, model);

            TemplateInfo templateInfo = viewDataContainer.ViewData.TemplateInfo;

            if (!String.IsNullOrEmpty(htmlFieldPrefix))
                templateInfo.HtmlFieldPrefix = templateInfo.GetFullHtmlFieldName(htmlFieldPrefix);

            ViewContext viewContext = htmlHelper.ViewContext;
            ViewContext newViewContext = new ViewContext(viewContext.Controller.ControllerContext, viewContext.View, viewDataContainer.ViewData, viewContext.TempData, viewContext.Writer);

            return new HtmlHelper<TModel>(newViewContext, viewDataContainer, htmlHelper.RouteCollection);
        }

        static IViewDataContainer CreateViewDataContainer(ViewDataDictionary viewData, object model)
        {

            var newViewData = new ViewDataDictionary(viewData)
            {
                Model = model
            };

            newViewData.TemplateInfo = new TemplateInfo
            {
                HtmlFieldPrefix = newViewData.TemplateInfo.HtmlFieldPrefix
            };

            return new ViewDataContainer
            {
                ViewData = newViewData
            };
        }

        class ViewDataContainer : IViewDataContainer
        {

            public ViewDataDictionary ViewData { get; set; }
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
