using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Intime.OPC.Job.Order.Models
{
    
    public class OrderStatusDetail
    {
        //订单号
        public string Id { get; set; }
        //状态
        public int Status { get; set; }
        public HeadDto Head { get; set; }

        public DetailDto Detail { get; set; }

        public PayMentDto PayMent { get; set; }


    }
}
