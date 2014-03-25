namespace com.intime.fashion.data.ims
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class IMS_AssociateIncomeHistory
    {
        public int Id { get; set; }

        public int SourceType { get; set; }

        [Required]
        [StringLength(20)]
        public string SourceNo { get; set; }

        public int AssociateUserId { get; set; }

        public decimal AssociateIncome { get; set; }

        public int Status { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime UpdateDate { get; set; }
    }
}
