using Intime.OPC.MessageHandlers.AccessToken;
using Intime.OPC.WebApi.Core.MessageHandlers.AccessToken;
using Intime.OPC.WebApi.Core.Security;

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Intime.OPC.WebApi.Core.MessageHandlers
{
    /// <summary>
    /// 用户身份验证消息处理
    /// </summary>
    public class AccessTokenMessageHandler : DelegatingHandler
    {

        private readonly IList<string> excludesUrls = new List<string>();

        public AccessTokenMessageHandler(IList<string> excludesUrls)
        {
            this.excludesUrls = excludesUrls;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            /**======================================================================
              检查是否支持
             ========================================================================*/
            if (IsPass(request))
            {
                return base.SendAsync(request, cancellationToken);
            }

            /**======================================================================
             AccessToken处理
            ========================================================================*/

            if (!request.Headers.Contains(AccessTokenConst.ACCESSTOKEN))
            {
                return createErrorResponse(request, "用户没有授权");
            }

            var accessToken = request.Headers.GetValues(AccessTokenConst.ACCESSTOKEN).FirstOrDefault();
            if (string.IsNullOrWhiteSpace(accessToken))
            {
                return createErrorResponse(request, "用户没有授权");
            }

            var obj = SecurityUtils.GetAccessToken(accessToken);
            if (obj == null)
            {
                return createErrorResponse(request, "用户授权失败");
            }

            if (obj.Expires.CompareTo(DateTime.Now) < 0)
            {
                return createErrorResponse(request, "AccessToken已经过期");
            }

            // 设置当前用户
            request.Properties.Add(AccessTokenConst.USERID_PROPERTIES_NAME, obj.UserId);

            return base.SendAsync(request, cancellationToken);
        }

        private bool IsPass(HttpRequestMessage request)
        {

            if (!Enabled)
            {
                return true;
            }

            return excludesUrls.Any(c => c == request.RequestUri.AbsolutePath);
        }

        private Task<HttpResponseMessage> createErrorResponse(HttpRequestMessage request, string message)
        {
            var errorResponse = request.CreateErrorResponse(HttpStatusCode.Forbidden, message);
            return Task.FromResult(errorResponse);
        }

        public bool Enabled
        {
            get
            {
                var enable = ConfigurationManager.AppSettings["AccessToken:Enabled"];

                if (enable == null)
                {
                    return true;
                }

                bool result = false;

                bool.TryParse(enable, out result);

                return result;
            }
        }
    }
}
