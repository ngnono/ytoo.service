using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.intime.fashion.common.Wxpay
{
   public class AccessToken
    {
       [JsonProperty]
       public string access_token { get; set; }
       [JsonProperty]
       public long expires_in { get; set; }
       [JsonIgnore]
       public bool IsExpired { get {
           return CreateDate.AddSeconds(expires_in) >= DateTime.Now;
       } }
       [JsonIgnore]
       public DateTime CreateDate { get; set; }
    }
}
