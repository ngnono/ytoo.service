namespace com.intime.fashion.data.ims
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class IMS_SalesCode
    {
        public int Id { get; set; }

        public int SectionId { get; set; }

        [Required]
        [StringLength(50)]
        public string Code { get; set; }
    }
}
