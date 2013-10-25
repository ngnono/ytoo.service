using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yintai.Hangzhou.Contract.DTO.Request
{
   public class WxNotifyPostRequest
    {
       public string OpenId { get; set; }
       public string AppId { get; set; }
       public int IsSubscribe { get; set; }
       public long TimeStamp { get; set; }
       public string NonceStr { get; set; }
       public string AppSignature { get; set; }
       public string SignMethod { get; set; }
    }
}
