using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Intime.OPC.MessageHandlers.AccessToken;
using Intime.OPC.WebApi.Core.Security;

namespace Intime.OPC.WebApi.Core.MessageHandlers.AccessToken
{
    /// <summary>
    ///     用户身份验证消息处理
    /// </summary>
    public class AccessTokenMessageHandler : DelegatingHandler
    {
        private readonly IList<string> _excludesUrls = new List<string>();

        public AccessTokenMessageHandler(IList<string> excludesUrls)
        {
            _excludesUrls = excludesUrls;
        }

        private bool Enabled
        {
            get
            {
                string enable = ConfigurationManager.AppSettings["AccessToken:Enabled"];

                if (enable == null)
                {
                    return true;
                }

                bool result;

                bool.TryParse(enable, out result);

                return result;
            }
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
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

            if (!request.Headers.Contains(HeadConfig.Toekn))
            {
                return CreateErrorResponse(request, "用户没有授权");
            }

            string accessToken = request.Headers.GetValues(HeadConfig.Toekn).FirstOrDefault();
            if (string.IsNullOrWhiteSpace(accessToken))
            {
                return CreateErrorResponse(request, "用户没有授权");
            }

            AccessTokenIdentity obj = SecurityUtils.GetAccessToken(accessToken);
            if (obj == null)
            {
                return CreateErrorResponse(request, "用户授权失败");
            }

            if (obj.Expires.CompareTo(DateTime.Now) < 0)
            {
                return CreateErrorResponse(request, "AccessToken已经过期");
            }

            // 设置当前用户
            request.Properties.Add(AccessTokenConst.UseridPropertiesName, obj.UserId);

            return base.SendAsync(request, cancellationToken);
        }

        private bool IsPass(HttpRequestMessage request)
        {
            return !Enabled || _excludesUrls.Any(c => c == request.RequestUri.AbsolutePath);
        }

        private static Task<HttpResponseMessage> CreateErrorResponse(HttpRequestMessage request, string message)
        {
            HttpResponseMessage errorResponse = request.CreateErrorResponse(HttpStatusCode.Forbidden, message);
            return Task.FromResult(errorResponse);
        }
    }
}