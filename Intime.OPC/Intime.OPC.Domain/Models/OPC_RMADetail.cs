using System;

namespace Intime.OPC.Domain.Models
{
    public class OPC_RMADetail
    {
        public int Id { get; set; }
        public string OpcRmaId { get; set; }
        public string CashNum { get; set; }
        public int? StockId { get; set; }
        public int Status { get; set; }
        public int BackCount { get; set; }
        public decimal Price { get; set; }
        public decimal Amount { get; set; }
        public string ProdSaleCode { get; set; }
        public bool? SalesPersonConfirm { get; set; }
        public DateTime RefundDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CreatedUser { get; set; }
        public DateTime UpdatedDate { get; set; }
        public int UpdatedUser { get; set; }
    }
}