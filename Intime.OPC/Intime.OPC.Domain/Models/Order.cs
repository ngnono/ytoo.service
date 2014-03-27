using System;
using Intime.OPC.Domain.Base;

namespace Intime.OPC.Domain.Models
{
    public class Order : IEntity
    {
        public string OrderNo { get; set; }
        public int CustomerId { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal? RecAmount { get; set; }
        public int Status { get; set; }
        public string PaymentMethodCode { get; set; }
        public string PaymentMethodName { get; set; }
        public string ShippingZipCode { get; set; }
        public string ShippingAddress { get; set; }
        public string ShippingContactPerson { get; set; }
        public string ShippingContactPhone { get; set; }
        public bool? NeedInvoice { get; set; }
        public string InvoiceSubject { get; set; }
        public string InvoiceDetail { get; set; }
        public decimal? ShippingFee { get; set; }
        public DateTime CreateDate { get; set; }
        public int CreateUser { get; set; }
        public DateTime UpdateDate { get; set; }
        public int UpdateUser { get; set; }
        public string ShippingNo { get; set; }
        public int? ShippingVia { get; set; }
        public int StoreId { get; set; }
        public int BrandId { get; set; }
        public string Memo { get; set; }
        public decimal? InvoiceAmount { get; set; }
        public int? TotalPoints { get; set; }
        public string OrderSource { get; set; }
        public int? OrderType { get; set; }

        #region IEntity Members

        public int Id { get; set; }

        #endregion
    }
}