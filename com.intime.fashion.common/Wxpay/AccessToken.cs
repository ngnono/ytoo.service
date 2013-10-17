using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.intime.fashion.common.Wxpay
{
   public class AccessToken
    {
       public string access_token { get; set; }
       public long expires_in { get; set; }

       public bool IsExpired { get {
           return CreateDate.AddSeconds(expires_in) >= DateTime.Now;
       } }

       public DateTime CreateDate { get; set; }
    }
}
