namespace com.intime.fashion.data.ims
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class IMS_GiftCardUser
    {
        public int Id { get; set; }

        [Required]
        [StringLength(10)]
        public string Name { get; set; }

        [Required]
        [StringLength(50)]
        public string GiftCardAccount { get; set; }

        public DateTime CreateDate { get; set; }

        public int CreateUser { get; set; }
    }
}
