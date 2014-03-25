using System;

namespace Intime.OPC.Domain.Models
{
    public class OPC_SaleDetail
    {
        public string SectionNumber { get; set; }
        public string Standard { get; set; }
        public string ColorCode { get; set; }
        public decimal SalePrice { get; set; }
        public int SaleCount { get; set; }
        public int RMACount { get; set; }
        public string Brand { get; set; }
        public decimal OriginalPrice { get; set; }
        public decimal PresentPrice { get; set; }
        public decimal SalesPrice { get; set; }
        public string GoodsCode { get; set; }
    }
}