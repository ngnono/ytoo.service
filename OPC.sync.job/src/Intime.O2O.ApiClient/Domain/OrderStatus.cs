using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Intime.O2O.ApiClient.Domain
{

       [DataContract]
    public class OrderStatus
    {
        //订单号
        [DataMember(Name = "ID")]
        public string id { get; set; }

       //状态
        [DataMember(Name = "STATUS")]
        public int status { get; set; }

        [DataMember(Name = "HEAD")]

        public IList<Head> head{get;set;}

        [DataMember(Name = "DETAIL")]
        public IList<Detail> detail { get; set; }
        
        [DataMember(Name = "PAYMENT")]
        public IList<Payment> payment { get; set; }

    }
}
