namespace com.intime.fashion.data.ims
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Order")]
    public partial class Order
    {
        public int Id { get; set; }

        [Required]
        [StringLength(20)]
        public string OrderNo { get; set; }

        public int CustomerId { get; set; }

        public decimal TotalAmount { get; set; }

        public decimal? RecAmount { get; set; }

        public int Status { get; set; }

        [Required]
        [StringLength(10)]
        public string PaymentMethodCode { get; set; }

        [StringLength(20)]
        public string PaymentMethodName { get; set; }

        [StringLength(20)]
        public string ShippingZipCode { get; set; }

        [StringLength(500)]
        public string ShippingAddress { get; set; }

        [StringLength(10)]
        public string ShippingContactPerson { get; set; }

        [StringLength(20)]
        public string ShippingContactPhone { get; set; }

        public bool? NeedInvoice { get; set; }

        [StringLength(200)]
        public string InvoiceSubject { get; set; }

        [StringLength(200)]
        public string InvoiceDetail { get; set; }

        public decimal? ShippingFee { get; set; }

        public DateTime CreateDate { get; set; }

        public int CreateUser { get; set; }

        public DateTime UpdateDate { get; set; }

        public int UpdateUser { get; set; }

        [StringLength(50)]
        public string ShippingNo { get; set; }

        public int? ShippingVia { get; set; }

        public int StoreId { get; set; }

        public int BrandId { get; set; }

        [StringLength(200)]
        public string Memo { get; set; }

        public decimal? InvoiceAmount { get; set; }

        public int? TotalPoints { get; set; }

        [StringLength(10)]
        public string OrderSource { get; set; }

        public int? OrderType { get; set; }
    }
}
