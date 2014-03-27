using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using Intime.OPC.Service;

namespace Intime.OPC.WebApi
{
    public class WebApiApplication : HttpApplication
    {
        protected void Application_Start()
        {
            // MEF配置
            MefConfig.RegisterMefDependencyResolver();

            AreaRegistration.RegisterAllAreas();

            // WebApi配置
            WebApiConfig.Register(GlobalConfiguration.Configuration);

            // 全局过滤器注册
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);

            // 路由注册
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            //初始化映射
            MapConfig.Config();
        }
    }
}