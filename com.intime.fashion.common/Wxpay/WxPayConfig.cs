using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.intime.fashion.common.Wxpay
{
    public static class WxPayConfig
    {
        public static readonly string APP_ID = ConfigurationManager.AppSettings["WX_APP_ID"];
        public static readonly string APP_SECRET = ConfigurationManager.AppSettings["WX_APP_SECRET"];
        public static readonly string PARTER_SIGN_KEY = ConfigurationManager.AppSettings["WX_SIGN_KEY"];
        public static readonly string PARTER_ID = ConfigurationManager.AppSettings["WX_PARTER_ID"];
        public static readonly string PARTER_KEY = ConfigurationManager.AppSettings["WX_PARTER_KEY"];

        public static readonly string NOTIFY_URL = ConfigurationManager.AppSettings["WX_NOTIFY_URL"];
        public static readonly string NOTIFY_ERP_URL = ConfigurationManager.AppSettings["WX_NOTIFY_ERP_URL"];
        public static string PaymentCode
        {
            get
            {
                return ConfigurationManager.AppSettings["WX_paymentcode"];
            }
        }
        public static readonly string PaymentName = "微信支付";
        public static readonly string ORDER_SOURCE = "BarSale";
        public static string WEB_SERVICE_BASE = ConfigurationManager.AppSettings["wx_WEBSERVICE_BASE"];
        public static readonly string MESSAGE_TEMPLATE_ID = ConfigurationManager.AppSettings["WX_MESSAGE_TEMPLATE_ID"];

    }

    public enum WxPayRetCode
    { 
        Success = 0,
        RequestError = 1
    }
    public enum WxPackageType
    { 
        Order = 1,
        Sku = 2,
        Product = 3,
        ErpOrder = 4
    }
    public static class WxPaySignMethod
    {
        public static readonly string SHA1 = "sha1";
    }
}
