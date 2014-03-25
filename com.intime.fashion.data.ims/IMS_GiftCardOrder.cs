namespace com.intime.fashion.data.ims
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class IMS_GiftCardOrder
    {
        public int Id { get; set; }

        [Required]
        [StringLength(20)]
        public string No { get; set; }

        public int GiftCardId { get; set; }

        public decimal Amount { get; set; }

        public int PurchaseUserId { get; set; }

        public int OwnerUserId { get; set; }

        public int Status { get; set; }

        public DateTime CreateDate { get; set; }

        public int CreateUser { get; set; }
    }
}
