using System.Collections.Generic;
using OPCApp.Domain.Models;
using OPCApp.Domain.Customer;

namespace OPCApp.ReturnGoodsManage.Print
{
    public class PrintModel
    {
        public List<OPC_RMA> RmaDT { get; set; }
        public List<RmaDetail> RMADetailDT { get; set; }
        public List<Order> OrderDT { get; set; }
    }
    public class ReturnGoodsPrintModel
    {
        public List<RMADto> RmaDT { get; set; }
        public List<RmaDetail> RMADetailDT { get; set; }

    }
}