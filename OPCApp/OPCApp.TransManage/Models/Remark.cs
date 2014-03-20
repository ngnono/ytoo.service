using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OPCApp.TransManage.Models
{
    public class Remark
    {
        public string OrderID { get; set; }
        public string InvoiceID { get; set; }
        public string SalesID { get; set; }
        public string Content { get; set; }
    }
}
