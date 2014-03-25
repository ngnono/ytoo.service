namespace com.intime.fashion.data.ims
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class IMS_GiftCardRecharge
    {
        public int Id { get; set; }

        public int ChargeUserId { get; set; }

        [Required]
        [StringLength(20)]
        public string PurchaseId { get; set; }

        [Required]
        [StringLength(11)]
        public string ChargePhone { get; set; }

        public DateTime CreateDate { get; set; }

        public int CreateUser { get; set; }
    }
}
