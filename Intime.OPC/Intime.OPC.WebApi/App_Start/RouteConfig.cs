using System.Web.Mvc;
using System.Web.Routing;

namespace Intime.OPC.WebApi
{
    /// <summary>
    /// 路由配置，目前暂时没有使用到
    /// </summary>
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}