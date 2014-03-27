using System;

namespace Intime.OPC.Domain.Models
{
    public class ShippingGet
    {
        public string OrderNo { get; set; }
        public string ExpressNo { get; set; }
        public DateTime StartGoodsOutDate { get; set; }
        public DateTime EndGoodsOutDate { get; set; }
        public string OutGoodsCode { get; set; }
        public int SectionId { get; set; }
        public int ShippingStatus { get; set; }

        public string CustomerPhone { get; set; }
        public string BrandId { get; set; }
    }
}