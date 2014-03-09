using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OPCApp.Order.Model
{
    public class OrderInfo : ModelBase
    {
        public string OrderID { get; set; }

        public string ChannelOrderID { get; set; }
        public string Source { get; set; }
        public string PayType { get; set; }
        public double Freight { get; set; }
        public string BackDate { get; set; }
        public double Sum { get; set; }
        public double BackGoodsSum { get; set; }
        public double Pay { get; set; }
        public double CompanyPayFreight { get; set; }
        public double BackSum { get; set; }
    }
}
