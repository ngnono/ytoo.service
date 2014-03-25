namespace com.intime.fashion.data.ims
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class IMS_InviteCode
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Code { get; set; }

        public int SectionOperatorId { get; set; }

        public int? AuthRight { get; set; }

        public int IsBinded { get; set; }

        public int UserId { get; set; }

        public int SourceType { get; set; }

        public DateTime CreateDate { get; set; }

        public int CreateUser { get; set; }

        public DateTime UpdateDate { get; set; }

        public int UpdateUser { get; set; }

        public int Status { get; set; }
    }
}
