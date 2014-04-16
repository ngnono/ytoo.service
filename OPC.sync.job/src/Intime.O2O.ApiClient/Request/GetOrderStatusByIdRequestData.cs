using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Intime.O2O.ApiClient.Request
{
    /// <summary>
    /// 根据Id获取订单信息
    /// </summary>
    [DataContract]
    public class GetOrderStatusByIdRequestData
    {

        [DataMember(Name = "counterid")]
        public string Id { get; set; }

        [DataMember(Name = "storeno")]
        public string StoreNo { get; set; }

    }
}
