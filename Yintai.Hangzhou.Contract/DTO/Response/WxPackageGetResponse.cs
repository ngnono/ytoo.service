using com.intime.fashion.common.Wxpay;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yintai.Hangzhou.Contract.DTO.Response
{
   public class WxPackageGetResponse
    {
       public string AppId { get; set; }
       public string TimeStamp { get; set; }
       public string NonceStr { get; set; }
       public int RetCode { get; set; }
       public string RetErrMsg { get; set; }
       public string AppSignature
       {
           get
           {
               var preSigned = new Dictionary<string, dynamic>();
               preSigned.Add("appid", AppId);
               preSigned.Add("appkey", WxPayConfig.PARTER_SIGN_KEY);
               preSigned.Add("package", Package==null?string.Empty:Package.Encode());
               preSigned.Add("timestamp", TimeStamp);
               preSigned.Add("nonceStr", NonceStr);
               preSigned.Add("retcode", RetCode);
               preSigned.Add("reterrmsg", RetErrMsg);
               return Util.PaySign(preSigned);

           }
       }
       public string SignMethod { get; set; }
       public WxPackage Package { get; set; }
    }
}
