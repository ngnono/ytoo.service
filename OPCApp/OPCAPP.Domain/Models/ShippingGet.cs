using System;

namespace OPCApp.Domain.Models
{
    public class ShippingGet
    {
        public ShippingGet()
        {
            StartGoodsOutDate = DateTime.Now;
            EndGoodsOutDate = DateTime.Now;
        }

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