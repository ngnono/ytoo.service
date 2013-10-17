using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace com.intime.fashion.common.Wxpay
{
    public class WxPackage
    {
        public string Body { get; set; }
        public string Attach { get; set; }
        public string OutTradeNo { get; set; }
        public long TotalFee { get; set; }
        public int FeeType { get { return 1; } }
        public string NotifyUrl { get { return WxPayConfig.NOTIFY_URL; } }
        public string SPBill_Create_IP { get; set; }
        public string Time_Start { get; set; }
        public string Time_End { get; set; }
        public long TransportFee { get; set; }
        public long ProductFee { get { return TotalFee - TransportFee; } }
        public string GoodsTag { get; set; }
        public string InputCharset { get { return "UTF-8"; } }

        public string Encode() {
            var values = new Dictionary<string, dynamic>();
            values.Add("bank_type", "WX");
            values.Add("body", Body);
            if (!string.IsNullOrEmpty(Attach))
                values.Add("attach", Attach);
            values.Add("parter", WxPayConfig.PARTER_ID);
            values.Add("out_trade_no", OutTradeNo);
            values.Add("total_fee", TotalFee);
            values.Add("fee_type", FeeType);
            values.Add("notify_url", WxPayConfig.NOTIFY_URL);
            values.Add("spbill_create_ip", SPBill_Create_IP);
            if (!string.IsNullOrEmpty(Time_Start))
                values.Add("time_start", Time_Start);
            if (!string.IsNullOrEmpty(Time_End))
                values.Add("time_expire", Time_End);
            values.Add("transport_fee", TransportFee);
            values.Add("product_fee", ProductFee);
            if (!string.IsNullOrEmpty(GoodsTag))
                values.Add("goods_tag", GoodsTag);
            values.Add("input_charset", InputCharset);

            var signingStr = values.OrderBy(b => b.Key).Aggregate(new StringBuilder(), (s, b) => s.AppendFormat("{0}={1}", b.Key, b.Value), s => s.ToString()).TrimEnd('&');
            signingStr = string.Format("{0}&key={1}", signingStr, WxPayConfig.PARTER_KEY);
            var signedStr = Util.MD5_Encode(signingStr).ToUpper();
            var resultStr = values.OrderBy(b => b.Key).Aggregate(new StringBuilder(), (s, b) => s.AppendFormat("{0}={1}", b.Key,HttpUtility.UrlEncode(b.Value)), s => s.ToString()).TrimEnd('&');
            return string.Format("{0}&sign={1}", resultStr, signedStr);
        }
        
    }
}

