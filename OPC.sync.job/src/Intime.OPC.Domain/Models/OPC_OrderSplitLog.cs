using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intime.OPC.Domain.Models
{
    public class OPC_OrderSplitLog
    {
        public int Id { get; set; }
        public string OrderNo { get; set; }
        public int Status { get; set; }
        public System.DateTime CreateDate { get; set; }
        public string Reason { get; set; }
    }
}
