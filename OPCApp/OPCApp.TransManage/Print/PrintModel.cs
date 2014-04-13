using OPCApp.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OPCApp.TransManage.Print
{
    public class PrintModel
    {
        public List<OPC_Sale> SaleDT { get; set; }
        public List<OPC_SaleDetail> SaleDetailDT { get; set; }
        public List<Order> OrderDT { get; set; }

    }
    
}
