using System.Collections.Generic;
using OPCApp.Domain.Models;

namespace Intime.OPC.Modules.Logistics.Print
{
    public class PrintModel
    {
        public List<OPC_Sale> SaleDT { get; set; }
        public List<OPC_SaleDetail> SaleDetailDT { get; set; }
        public List<Order> OrderDT { get; set; }
    }
}