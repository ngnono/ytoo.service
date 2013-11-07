using com.intime.fashion.common.Wxpay;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Yintai.Architecture.Common.Logger;
using Yintai.Architecture.Framework.ServiceLocation;

namespace com.intime.fashion.common
{
    public static class WxServiceHelper
    {
        private static AccessToken WxToken;
        public static bool Notify(dynamic requestData, Action<dynamic> successCallback, Action<dynamic> failCallback)
        {
            var client = new RestClient(WxPayConfig.WEB_SERVICE_BASE);
            if (WxToken == null || WxToken.IsExpired)
            {

                var request = new RestRequest("cgi-bin/token", Method.GET);
                request.AddParameter("grant_type", "client_credential");
                request.AddParameter("appid", WxPayConfig.APP_ID);
                request.AddParameter("secret", WxPayConfig.APP_SECRET);
                var response = client.Execute<AccessToken>(request);
                WxToken = response.Data;
                WxToken.CreateDate = DateTime.Now;
            }
            var notifyRequest = new RestRequest("pay/delivernotify", Method.POST);
            notifyRequest.AddUrlSegment("access_token", WxToken.access_token);
            notifyRequest.AddBody(requestData);
            var notifyResponse = client.ExecuteDynamic(notifyRequest);

            if (notifyResponse.Data.errcode == 0)
            {
                if (successCallback != null)
                    successCallback(notifyResponse.Data);
                return true;
            }
            else
            {
                Logger.Debug(notifyResponse.Data);

                if (failCallback != null)
                    failCallback(notifyResponse.Data);
                return false;
            }


        }

        public static bool Query(dynamic requestData, Action<dynamic> successCallback, Action<dynamic> failCallback)
        {
            var client = new RestClient(WxPayConfig.WEB_SERVICE_BASE);
            if (WxToken == null || WxToken.IsExpired)
            {

                var request = new RestRequest("cgi-bin/token", Method.GET);
                request.AddParameter("grant_type", "client_credential");
                request.AddParameter("appid", WxPayConfig.APP_ID);
                request.AddParameter("secret", WxPayConfig.APP_SECRET);
                var response = client.Execute(request);
                WxToken = JsonConvert.DeserializeObject<AccessToken>(response.Content);
                WxToken.CreateDate = DateTime.Now;
            }
            var notifyRequest = new RestRequest("pay/orderquery?access_token={access_token}", Method.POST);
            notifyRequest.RequestFormat = DataFormat.Json;
            notifyRequest.AddUrlSegment("access_token", WxToken.access_token);
            notifyRequest.AddBody(requestData);
            var notifyResponse =JsonConvert.DeserializeObject<dynamic>(client.Execute(notifyRequest).Content);

            if (notifyResponse.errcode == 0)
            {
                if (successCallback != null)
                    successCallback(notifyResponse);
                return true;
            }
            else
            {
                Logger.Debug(notifyResponse);

                if (failCallback != null)
                    failCallback(notifyResponse);
                return false;
            }


        }
        private static ILog Logger
        {
            get
            {
                return ServiceLocator.Current.Resolve<ILog>();
            }
        }

    }
}
