using System.Collections.Generic;
using System.Web.Http;
using System.Web.Routing;
using Intime.OPC.WebApi.Core.MessageHandlers.AccessToken;
using Intime.OPC.WebApi.Core.MessageHandlers.Signature;

namespace Intime.OPC.WebApi
{
    /// <summary>
    ///     WebApi 配置
    /// </summary>
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // WebApi2.1支持的RouteAttribute进行Map
            config.MapHttpAttributeRoutes();

            // 注册Api默认的路由
            config.Routes.MapHttpRoute("DefaultApi",
                "api/{controller}/{action}/{id}",
                new { controller = "home", action = "index", id = RouteParameter.Optional }
                );

            config.Routes.MapHttpRoute("DefaultApi2",
                "api/{controller}",
                new { id = System.Web.Http.RouteParameter.Optional }
                );

            // 添加签名验证
            config.MessageHandlers.Add(new SignatureMessageHandler());
            config.MessageHandlers.Add(new AccessTokenMessageHandler(new List<string>
            {
                "/api/account/token"
            }));

            config.EnsureInitialized();
        }
    }
}