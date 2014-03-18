using System;

namespace Intime.OPC.Domain.Models
{
    public class OPC_Stock
    {
        public int Id { get; set; }
        public int SkuId { get; set; }
        public int SourceStockId { get; set; }
        public int SectionId { get; set; }
        public int? Count { get; set; }
        public decimal Price { get; set; }
        public int Status { get; set; }
        public bool? IsDel { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CreatedUser { get; set; }
        public DateTime UpdatedDate { get; set; }
        public int UpdatedUser { get; set; }
    }
}