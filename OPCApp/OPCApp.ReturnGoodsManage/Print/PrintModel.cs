using System.Collections.Generic;
using OPCApp.Domain.Models;

namespace OPCApp.ReturnGoodsManage.Print
{
    public class PrintModel
    {
        public List<OPC_RMA> RmaDT { get; set; }
        public List<OPC_RMADetail> RMADetailDT { get; set; }
        public List<Order> OrderDT { get; set; }
    }
}