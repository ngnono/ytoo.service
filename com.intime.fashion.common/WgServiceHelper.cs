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
        private static AccessToken WxToken_Mini;

        public static AccessToken Token
        {
            get {
                if (WxToken == null ||
                    WxToken.IsExpired)
                {
                    WxToken = new AccessToken(AccessTokenType.XihuanYintai).Renew();
                };
                return WxToken;
            }
        }

        public static AccessToken TokenMini
        {
            get
            {
                if (WxToken_Mini == null ||
                    WxToken_Mini.IsExpired)
                {
                    WxToken_Mini = new AccessToken(AccessTokenType.MiniYin).Renew();
                };
                return WxToken_Mini;
            }
        }
       
    }
}
