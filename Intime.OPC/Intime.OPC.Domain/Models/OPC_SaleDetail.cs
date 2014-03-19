using System;

namespace Intime.OPC.Domain.Models
{
    public class OPC_SaleDetail
    {
        public int Id { get; set; }
        public int SaleId { get; set; }
        public int Status { get; set; }
        public int StockId { get; set; }
        public int SaleCount { get; set; }
        public decimal Price { get; set; }
        public int? BackNumber { get; set; }
        public string ProdSaleCode { get; set; }
        public string Remark { get; set; }
        public DateTime? RemarkDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CreatedUser { get; set; }
        public DateTime UpdatedDate { get; set; }
        public int UpdatedUser { get; set; }
    }
}