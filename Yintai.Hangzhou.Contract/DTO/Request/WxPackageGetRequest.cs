using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yintai.Hangzhou.Contract.DTO.Request
{
    public class WxPackageGetRequest
    {
        public string AppId { get; set; }
        public string OpenId { get; set; }
        public int IsSubscribe { get; set; }
        public string ProductId { get; set; }
        public string TimeStamp { get; set; }
        public string NonceStr { get; set; }
        public string AppSignature { get; set; }
        public string SignMethod { get; set; }
    }
}
