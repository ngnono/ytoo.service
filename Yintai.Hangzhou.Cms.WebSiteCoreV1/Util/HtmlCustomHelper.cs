using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;
using Yintai.Hangzhou.Cms.WebSiteCoreV1.Controllers;

namespace Yintai.Hangzhou.Cms.WebSiteCoreV1.Util
{
    public static class HtmlCustomHelper
    {
        public static string ClientId<TModel>(this HtmlHelper helper, Expression<Func<TModel, TModel>> ex)
        {
            return helper.ViewData.TemplateInfo.GetFullHtmlFieldId(ExpressionHelper.GetExpressionText(ex));
        }

        public static MvcHtmlString ActionLink2(this HtmlHelper htmlHelper, string linkText, string actionName)
        {
            return htmlHelper.ActionLink2(linkText, actionName, null, new RouteValueDictionary(), new RouteValueDictionary());
        }

        public static MvcHtmlString ActionLink2(this HtmlHelper htmlHelper, string linkText, string actionName, object routeValues)
        {
            return htmlHelper.ActionLink2(linkText, actionName, null, new RouteValueDictionary(routeValues), new RouteValueDictionary());
        }

        public static MvcHtmlString ActionLink2(this HtmlHelper htmlHelper, string linkText, string actionName, RouteValueDictionary routeValues)
        {
            return htmlHelper.ActionLink2(linkText, actionName, null, routeValues, new RouteValueDictionary());
        }

        public static MvcHtmlString ActionLink2(this HtmlHelper htmlHelper, string linkText, string actionName, string controllerName)
        {
            return htmlHelper.ActionLink2(linkText, actionName, controllerName, new RouteValueDictionary(), new RouteValueDictionary());
        }

        public static MvcHtmlString ActionLink2(this HtmlHelper htmlHelper, string linkText, string actionName, object routeValues, object htmlAttributes)
        {
            return htmlHelper.ActionLink2(linkText, actionName, null, new RouteValueDictionary(routeValues), HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        public static MvcHtmlString ActionLink2(this HtmlHelper htmlHelper, string linkText, string actionName, RouteValueDictionary routeValues, IDictionary<string, object> htmlAttributes)
        {
            return htmlHelper.ActionLink2(linkText, actionName, null, routeValues, htmlAttributes);
        }

        public static MvcHtmlString ActionLink2(this HtmlHelper htmlHelper, string linkText, string actionName, string controllerName, object routeValues, object htmlAttributes)
        {
            return htmlHelper.ActionLink2(linkText, actionName, controllerName, new RouteValueDictionary(routeValues), HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        public static MvcHtmlString ActionLink2(this HtmlHelper htmlHelper, string linkText, string actionName, string controllerName, RouteValueDictionary routeValues, IDictionary<string, object> htmlAttributes)
        {
            var controller = htmlHelper.ViewContext.Controller as UserController;
            if (controller != null &&
                !controller.HasRightForAction(controllerName, actionName))
            {
                return HtmlCustomHelper.GenerateNonAuthorizeHtml();
            }
            return htmlHelper.ActionLink(linkText, actionName, controllerName, routeValues, htmlAttributes);
        }

        public static MvcHtmlString ActionLink2(this HtmlHelper htmlHelper, string linkText, string actionName, string controllerName, string protocol, string hostName, string fragment, object routeValues, object htmlAttributes)
        {
            return htmlHelper.ActionLink2(linkText, actionName, controllerName, protocol, hostName, fragment, new RouteValueDictionary(routeValues), HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        public static MvcHtmlString ActionLink2(this HtmlHelper htmlHelper, string linkText, string actionName, string controllerName, string protocol, string hostName, string fragment, RouteValueDictionary routeValues, IDictionary<string, object> htmlAttributes)
        { 
            
            var controller = htmlHelper.ViewContext.Controller as UserController;
            if (controller != null &&
                !controller.HasRightForAction(controllerName, actionName))
            {
                return HtmlCustomHelper.GenerateNonAuthorizeHtml();
            }
            return htmlHelper.ActionLink(linkText, actionName, controllerName, protocol, hostName, fragment, routeValues, htmlAttributes);
        }
        private static MvcHtmlString GenerateNonAuthorizeHtml()
        {
            return MvcHtmlString.Create(string.Empty);
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
