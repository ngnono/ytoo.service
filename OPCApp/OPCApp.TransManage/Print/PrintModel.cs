using System.Collections.Generic;
using OPCApp.Domain.Models;

namespace OPCApp.TransManage.Print
{
    public class PrintModel
    {
        public List<OPC_Sale> SaleDT { get; set; }
        public List<OPC_SaleDetail> SaleDetailDT { get; set; }
        public List<Order> OrderDT { get; set; }
    }
}