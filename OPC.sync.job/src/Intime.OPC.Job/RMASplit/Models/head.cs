using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Intime.OPC.Job.RMASplit.Models
{
    public class Head
    {
        public string id { get; set; }
        public string mainid { get; set; }
        public int flag { get;set;}
        public DateTime createtime { get; set; }
        public DateTime paytime { get; set; }
        public int type { get; set; }
        public int status { get; set; }
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
