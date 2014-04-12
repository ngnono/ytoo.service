using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Intime.OPC.Job.RMASplit.Models
{
    public class Payment
    {
        public string id { get; set; }
        public string type { get; set; }
        public int typeid { get; set; }
        public string typename { get; set; }
        public string no { get; set; }
        public decimal amount { get; set; }
        public int rowno { get; set; }
        public string memo { get; set; }
        public string storeno { get; set; }

    }
}
