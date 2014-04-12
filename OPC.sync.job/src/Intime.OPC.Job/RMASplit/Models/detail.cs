using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Intime.OPC.Job.RMASplit.Models
{
    public class Detail
    {
        public string id { get; set; }
        public string productid { get; set; }
        public string productname { get; set; }
        public decimal price { get; set; }
        public decimal discount { get; set; }
        public int vipdiscount { get; set; }
        public int quantity { get; set; }
        public decimal total { get; set; }
        public int rowno { get; set; }
        public string comcode { get; set; }
        public string counter { get; set; }
        public string memo { get; set; }
        public string storeno { get; set; }

    }
}
