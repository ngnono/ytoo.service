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
       
        public static bool Notify(dynamic requestData, Action<dynamic> successCallback, Action<dynamic> failCallback)
        {
            var client = new RestClient(WxPayConfig.WEB_SERVICE_BASE);
            var token = WgServiceHelper.Token;
            if (token == null)
            {
                CommonUtil.Log.Info("token is empty");
                return false;
            }
            var notifyRequest = new RestRequest("pay/delivernotify?access_token={access_token}", Method.POST);
            notifyRequest.RequestFormat = DataFormat.Json;
            notifyRequest.AddUrlSegment("access_token", token.access_token);
            notifyRequest.AddBody(requestData);
            var response = client.Execute(notifyRequest).Content;
            var notifyResponse = JsonConvert.DeserializeObject<dynamic>(response);

            if (notifyResponse.errcode == 0)
            {
                if (successCallback != null)
                    successCallback(notifyResponse);
                return true;
            }
            else
            {
                Logger.Debug(response);

                if (failCallback != null)
                    failCallback(notifyResponse);
                return false;
            }


        }

        public static bool Query(dynamic requestData, Action<dynamic> successCallback, Action<dynamic> failCallback)
        {
            var client = new RestClient(WxPayConfig.WEB_SERVICE_BASE);
            var token = WgServiceHelper.Token;
            if (token == null)
            {
                CommonUtil.Log.Info("token is empty");
                return false;
            }
            var notifyRequest = new RestRequest("pay/orderquery?access_token={access_token}", Method.POST);
            notifyRequest.RequestFormat = DataFormat.Json;
            notifyRequest.AddUrlSegment("access_token", token.access_token);
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

        public static bool SendMessage(dynamic requestData, Action<dynamic> successCallback, Action<dynamic> failCallback)
        {
            var client = new RestClient(WxPayConfig.WEB_SERVICE_BASE);
            var token = WgServiceHelper.Token;
            if (token == null)
            {
                CommonUtil.Log.Info("token is empty");
                return false;
            }
            var notifyRequest = new RestRequest("cgi-bin/message/template/send?access_token={access_token}", Method.POST);
            notifyRequest.RequestFormat = DataFormat.Json;
            notifyRequest.AddUrlSegment("access_token", token.access_token);
            notifyRequest.AddBody(requestData);
            var notifyResponse = JsonConvert.DeserializeObject<dynamic>(client.Execute(notifyRequest).Content);

            if (notifyResponse.errcode == 0)
            {
                if (successCallback != null)
                    successCallback(notifyResponse);
                return true;
            }
            else
            {
                WgServiceHelper.RenewToken();
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
