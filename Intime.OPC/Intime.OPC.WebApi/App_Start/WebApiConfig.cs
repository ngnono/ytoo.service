using Newtonsoft.Json.Serialization;
using System.Linq;
using System.Web.Http;

using Intime.OPC.WebApi.Controllers;
using Intime.OPC.WebApi.Core.MessageHandlers;
using Intime.OPC.WebApi.Core.MessageHandlers.AccessToken;
using System.Collections.Generic;
using System.Web.Http.Dispatcher;
using System.Reflection;

namespace Intime.OPC.WebApi
{
    /// <summary>
    /// WebApi 配置
    /// </summary>
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // 注册Api默认的路由
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { controller = "home",  action = "index", id = RouteParameter.Optional }
            );

            // WebApi2.1支持的RouteAttribute进行Map
            config.MapHttpAttributeRoutes();

            // 添加签名验证
            config.MessageHandlers.Add(new SignatureMessageHandler());
            config.MessageHandlers.Add(new AccessTokenMessageHandler(new List<string>()
            {
                "/api/account/token"
            }));

            config.EnsureInitialized();
        }
    }
}
