namespace com.intime.fashion.data.ims
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class IMS_GiftCardTransfers
    {
        public int Id { get; set; }

        public int GiftCardId { get; set; }

        public int FromUserId { get; set; }

        public int ToUserId { get; set; }

        [Required]
        [StringLength(11)]
        public string Phone { get; set; }

        [StringLength(50)]
        public string Comment { get; set; }

        public int IsActive { get; set; }

        public int IsDecline { get; set; }

        public int PreTransferId { get; set; }

        public DateTime CreateDate { get; set; }

        public int CreateUser { get; set; }
    }
}
