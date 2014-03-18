using System;

namespace Intime.OPC.Domain.Models
{
    public class OPC_RMA
    {
        public int Id { get; set; }
        public int SaleId { get; set; }
        public string RMANo { get; set; }
        public bool? IsInquirer { get; set; }
        public string SourceDesc { get; set; }
        public int? Count { get; set; }
        public decimal RefundAmount { get; set; }
        public bool? IsShipping { get; set; }
        public bool? IsPackage { get; set; }
        public string OrderNo { get; set; }
        public int RMAType { get; set; }
        public int Status { get; set; }
        public decimal RMAAmount { get; set; }
        public string BankName { get; set; }
        public string BankAccount { get; set; }
        public string BankCard { get; set; }
        public string RejectReason { get; set; }
        public decimal? RebatePostfee { get; set; }
        public decimal? Chargepostfee { get; set; }
        public decimal? ActualAmount { get; set; }
        public string GiftReason { get; set; }
        public string InvoiceReason { get; set; }
        public string RebatePointReason { get; set; }
        public string PostalFeeReason { get; set; }
        public decimal? ChargeGiftFee { get; set; }
        public string ContactPhone { get; set; }
        public int? ShipviaId { get; set; }
        public string ShipNo { get; set; }
        public int? UserId { get; set; }
        public string MailAddress { get; set; }
        public int? RMAReason { get; set; }
        public string ContactPerson { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CreatedUser { get; set; }
        public DateTime UpdatedDate { get; set; }
        public int UpdatedUser { get; set; }
    }
}