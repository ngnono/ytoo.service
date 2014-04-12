using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intime.OPC.Job.Order.Models
{
    public class OrderStatus
    {
        //订单号
        public string id { get; set; }
       //状态
        public int status { get; set; }
        public IList<Head> head{get;set;}
        public IList<Detail> detail { get; set; }
        public IList<Payment> payment { get; set; }

    }
}
