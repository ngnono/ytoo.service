using Intime.O2O.ApiClient.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Intime.O2O.ApiClient.Request
{
    [DataContract]
    public class GetOrderStatusRequestData
    {
        //订单号
        [DataMember(Name = "ID")]
        public string Id { get; set; }
        //状态
        [DataMember(Name = "STATUS")]
        public int Status { get; set; }
        [DataMember(Name = "HEAD")]
        public List<Head> Head { get; set; }
        [DataMember(Name = "DETAIL")]
        public List<Detail> Detail { get; set; }
        [DataMember(Name = "PAYMENT")]
        public List<PayMent> PayMent { get; set; }

    }
}
