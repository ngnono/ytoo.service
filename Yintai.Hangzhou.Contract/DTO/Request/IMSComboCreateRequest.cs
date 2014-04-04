using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Yintai.Hangzhou.Contract.DTO.Request
{
    public class IMSComboCreateRequest:BaseRequest
    {
        public int Product_Type { get; set; }
        [JsonProperty("image_ids[]")]
        public int[] Image_Ids { get; set; }
        [JsonProperty("productids[]")]
        public int[] ProductIds { get; set; }
        public string Desc { get; set; }
        public string Private_To { get; set; }
    }
}
