using Intime.OPC.WebApi.Core.Security;

using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Intime.OPC.WebApi.Core.MessageHandlers
{
    /// <summary>
    /// 签名处理消息类，
    /// 说明 ： 针对发送数据和URL进行统一的签名处理
    /// </summary>
    public class SignatureMessageHandler : DelegatingHandler
    {
        #region fields

        private const string SIGNATURE_HTTP_HEADER_NAME = "X-Sign";
        private const string AppKey_HTTP_HEADER_NAME = "X-AppKey";

        private IAppSecurityManager appSecurityManager = new AppSecurityManager();

        #endregion

        #region Methods

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            /**======================================================================
             *  签名开启检查
            ========================================================================--*/

            if (!appSecurityManager.Enabled)
            {
                return base.SendAsync(request, cancellationToken);
            }

            /**======================================================================
             *  签名规则说明
             *  
             *      正对签名使用{url}+{postdata}+{consumerKey}，统一转化为大写进行MD5计算
             *      
             ========================================================================--*/

            if (!request.Headers.Contains(SIGNATURE_HTTP_HEADER_NAME))
            {
                return createErrorResponse(request, "签名为空");
            }

            if (!request.Headers.Contains(AppKey_HTTP_HEADER_NAME))
            {
                return createErrorResponse(request, "AppKey为空");
            }

            var sign = request.Headers.GetValues(SIGNATURE_HTTP_HEADER_NAME).FirstOrDefault();

            if (string.IsNullOrWhiteSpace(sign))
            {
                return createErrorResponse(request, "签名不正确");
            }

            var appKey = request.Headers.GetValues(AppKey_HTTP_HEADER_NAME).FirstOrDefault();
            if (string.IsNullOrWhiteSpace(appKey))
            {
                return createErrorResponse(request, "AppKey不能为空");
            }

            var secretKey = appSecurityManager.GetSecretKey(appKey);
            if (string.IsNullOrWhiteSpace(secretKey))
            {
                return createErrorResponse(request, string.Format("AppKey找不到相关配置"));
            }

            /**======================================================================
            *  计算签名
            ========================================================================--*/

            var buffer = new StringBuilder();

            // 添加请求地址
            buffer.Append(request.RequestUri.PathAndQuery);

            // 添加PostData
            if (!request.Content.IsMimeMultipartContent())
            {
                var postData = request.Content.ReadAsStringAsync().Result;
                buffer.Append(postData);
            }

            // 添加secretKey
            buffer.Append(secretKey);

            /*======================================================================
            *  比较签名
            ========================================================================--*/

            var signStr = GetMd5Hash(buffer.ToString().ToUpper());
            if (string.Compare(sign, signStr, true) != 0)
            {
                return createErrorResponse(request, "签名不正确");
            }

            /*======================================================================
            *  执行函数
            ========================================================================--*/
            return base.SendAsync(request, cancellationToken);
        }


        #region Helper

        private Task<HttpResponseMessage> createErrorResponse(HttpRequestMessage request, string message)
        {
            var errorResponse = request.CreateErrorResponse(HttpStatusCode.BadRequest, message);
            return Task.FromResult(errorResponse);
        }

        private static string GetMd5Hash(string content)
        {
            MD5 md5Hasher = MD5.Create();
            byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(content));

            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            return sBuilder.ToString();
        }

        #endregion

        public void SetAppSecurityManager(IAppSecurityManager appSecurityManager)
        {
            this.appSecurityManager = appSecurityManager;
        }

        #endregion
    }
}
