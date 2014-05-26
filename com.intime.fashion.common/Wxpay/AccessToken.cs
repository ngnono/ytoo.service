using com.intime.fashion.common.Weigou;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.intime.fashion.common.Wxpay
{
    public class AccessToken
    {
        private AccessTokenType _tokenType;
        private long _expiresTime;

        public AccessToken(AccessTokenType tokenType)
        {
            _tokenType = tokenType;
        }

        public string access_token { get; set; }

        public long expires_in
        {
            get { return _expiresTime; }
            set
            {
                _expiresTime = value;
                ExpireDate = new DateTime(1970, 1, 1).AddSeconds(expires_in);
            }
        }

        public bool IsExpired
        {
            get
            {
                return ExpireDate >= DateTime.UtcNow;
            }
        }
        private DateTime ExpireDate { get; set; }

        public AccessToken Renew()
        {
            switch (_tokenType)
            {
                case AccessTokenType.XihuanYintai:
                    RenewTokenFromWeigou();
                    break;
                case AccessTokenType.MiniYin:
                    RenewTokenFromAws();
                    break;
                default:
                    break;
            }

            return this;
        }
        public AccessToken Token()
        {
            switch (_tokenType)
            {
                case AccessTokenType.XihuanYintai:
                    return Renew();
                case AccessTokenType.MiniYin:
                    AwsHelper.SendHttpMessage(string.Format("{0}ims/weixin/access_token", ConfigManager.AwsHost),
                        null, ConfigManager.AwsHttpPublicKey,
                        ConfigManager.AwsHttpPrivateKey,
                        r =>
                        {
                            access_token = r.token;
                        }, null);
                    return this;
                default:
                    break;
            }

            return this; 
        }

        private void RenewTokenFromWeigou()
        {
            var client = new RestClient(WeigouConfig.BASE_URL);
            var request = new RestRequest("app/getAppInfoByAppId.xhtml", Method.GET);
            request.AddParameter("appID", WxPayConfig.APP_ID);
            request.AddParameter("appKey", WxPayConfig.APP_SECRET);
            var response = client.Execute(request).Content;
            var resObject = JsonConvert.DeserializeObject<dynamic>(response).getAppInfoByAppId;
            if (resObject.errorCode != 0)
            {
                CommonUtil.Log.Info(resObject.errorMessage);
                return;
            }
            access_token = resObject.appInfo.accessToken;
            expires_in = resObject.appInfo.expireTime - 600;
        }

        private void RenewTokenFromAws()
        {
            AwsHelper.SendHttpMessage(string.Format("{0}ims/weixin/renew", ConfigManager.AwsHost),
                         null, ConfigManager.AwsHttpPublicKey,
                         ConfigManager.AwsHttpPrivateKey,
                         r =>
                         {
                             access_token = r.token;
                         }, null);

        }
    }

    public enum AccessTokenType
    {
        XihuanYintai,
        MiniYin
    }
}
