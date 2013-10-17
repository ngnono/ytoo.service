using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;


namespace com.intime.fashion.common.Wxpay
{
    public static class Util
    {
        public static string Nonce()
        {
            return Guid.NewGuid().ToString().Substring(0, 30);
        }



        public static string PaySign(Dictionary<string, dynamic> kvalues)
        {
            var toSignStr = kvalues.OrderBy(b => b.Key).Aggregate(new StringBuilder(), (s, b) => s.AppendFormat("{0}={1}&", b.Key, b.Value), s => s.ToString());
            return SHA(toSignStr.TrimEnd('&'));
        }


        private static string SHA(string value)
       {
           byte[] hashData = SHA1.Create().ComputeHash((Encoding.UTF8.GetBytes(value))); 
           var hashText = new StringBuilder();
           foreach (byte b in hashData)
           {
              var hexValue = b.ToString("X").ToLower(); 
               hashText.Append(hexValue.Length == 1 ? "0" : "" + hexValue);
           }
           return hashText.ToString();
       }

        public static string MD5_Encode(string value)
        { 
            byte[] hashData = MD5.Create().ComputeHash((Encoding.UTF8.GetBytes(value))); 
           var hashText = new StringBuilder();
           foreach (byte b in hashData)
           {
              var hexValue = b.ToString("X").ToLower(); 
               hashText.Append(hexValue.Length == 1 ? "0" : "" + hexValue);
           }
           return hashText.ToString();
        }

        public static long Feng4Decimal(decimal value)
        {
            return (long)decimal.Multiply(value, 100);
        }

        public static string NotifySign(Dictionary<string, string> sPara)
        {
            var signingStr = sPara.OrderBy(s=>s.Key).Aggregate(new StringBuilder(),(s,b)=>s.AppendFormat("{0}={1}&",b.Key,b.Value),s=>s.ToString().TrimEnd('&'));
            signingStr = string.Format("{0}&key={1}",signingStr,WxPayConfig.PARTER_KEY);
            return MD5_Encode(signingStr).ToUpper();
        }
    }
}
