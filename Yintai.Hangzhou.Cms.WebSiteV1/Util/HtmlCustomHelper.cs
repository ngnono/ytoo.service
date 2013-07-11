using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;
using Yintai.Hangzhou.Cms.WebSiteV1.Controllers;

namespace Yintai.Hangzhou.Cms.WebSiteV1.Util
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
        public static MvcHtmlString LinkToAddNestedForm<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, string linkText, string containerElement, string counterElement, string cssClass = null) where TProperty : IEnumerable<object>
        {
            // a fake index to replace with a real index
            long ticks = DateTime.UtcNow.Ticks;
            // pull the name and type from the passed in expression
            string collectionProperty = ExpressionHelper.GetExpressionText(expression);
            var nestedObject = Activator.CreateInstance(typeof(TProperty).GetGenericArguments()[0]);

            // save the field prefix name so we can reset it when we're doing
            string oldPrefix = htmlHelper.ViewData.TemplateInfo.HtmlFieldPrefix;
            // if the prefix isn't empty, then prepare to append to it by appending another delimiter
            if (!string.IsNullOrEmpty(oldPrefix))
            {
                htmlHelper.ViewData.TemplateInfo.HtmlFieldPrefix += ".";
            }
            // append the collection name and our fake index to the prefix name before rendering
            htmlHelper.ViewData.TemplateInfo.HtmlFieldPrefix += string.Format("{0}[{1}]", collectionProperty, ticks);
            string partial = htmlHelper.EditorFor(x => nestedObject).ToHtmlString();
              // done rendering, reset prefix to old name
            htmlHelper.ViewData.TemplateInfo.HtmlFieldPrefix = oldPrefix;

            // strip out the fake name injected in (our name was all in the prefix)
            partial = Regex.Replace(partial, @"[\._]?nestedObject", "");

            // encode the output for javascript since we're dumping it in a JS string
            partial = HttpUtility.JavaScriptStringEncode(partial);

            // create the link to render
            var js = string.Format("javascript:addNestedForm('{0}','{1}','{2}','{3}');return false;", containerElement, counterElement, ticks, partial);
            TagBuilder a = new TagBuilder("a");
            a.Attributes.Add("href", "javascript:void(0)");
            a.Attributes.Add("onclick", js);
            if (cssClass != null)
            {
                a.AddCssClass(cssClass);
            }
            a.InnerHtml = linkText;

            return MvcHtmlString.Create(a.ToString(TagRenderMode.Normal));
        }

        public static MvcHtmlString Captcha(this HtmlHelper htmlHelper)
        {
          

            var html = Recaptcha.RecaptchaControlMvc.GenerateCaptcha(htmlHelper,null,"clean");
            return MvcHtmlString.Create(html);
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
