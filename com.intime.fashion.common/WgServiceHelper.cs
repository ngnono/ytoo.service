using com.intime.fashion.common.Weigou;
using com.intime.fashion.common.Wxpay;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.intime.fashion.common
{
    public static class WgServiceHelper
    {
        private static AccessToken WxToken;

        public static AccessToken Token
        {
            get {
                if (WxToken == null ||
                    WxToken.IsExpired)
                    RenewToken();
                return WxToken;
            }
        }

        private static void RenewToken()
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
            WxToken = new AccessToken() {
                access_token = resObject.appInfo.accessToken,
                expires_in = resObject.appInfo.expireTime
            };
        }
    }
}
