using System;

namespace Intime.OPC.Domain.Models
{
    public class OPC_ShippingSale
    {
        public int Id { get; set; }
        public string SaleOrderNo { get; set; }

        public int ShipCompanyName { get; set; }
        public int ShipManName { get; set; }

        public string GoodsOutCode { get; set; }//发货单号

        public string ShippingCode { get; set; }
        public decimal ShippingFee { get; set; }
        public int ShippingStatus { get; set; }
        public DateTime CreateDate { get; set; }
        public int CreatedUser { get; set; }
        public DateTime UpdatedDate { get; set; }
        public int UpdatedUser { get; set; }

        public string CustomerName { get; set; }
        public string CustomerAddress { get; set; }
        public string CustomerPhone { get; set; }

        public DateTime GoodsOutDate { get; set; }//发货时间
        public string GoodsOutType { get; set; }//发货方式
    }
}