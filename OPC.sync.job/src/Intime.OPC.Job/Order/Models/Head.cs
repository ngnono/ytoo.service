using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Intime.OPC.Job.Order.Models
{
    public class Head
    {
        //订单号
        public string id { get; set; }
        //同订单号
        public string mainid { get; set; }
        //标识  1
        public int flag { get;set;}
        public DateTime createtime { get; set; }
        //收银时间
        public DateTime paytime { get; set; }
        //   1
        public int type { get; set; }
        //状态
        public int status { get; set; }
        //数量
        public int quantity { get; set; }

        public decimal discount { get; set; }
        public decimal total { get; set; }
        public string vipno { get; set; }
        public string vipmemo { get; set; }
        public string storeno { get; set; }
        public string oldid { get; set; }
        public string operid { get; set; }
        public string opername { get; set; }
        public string opertime { get; set; }


    }
}
