namespace com.intime.fashion.data.ims
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class IMS_Combo
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Desc { get; set; }

        public decimal Price { get; set; }

        [Required]
        [StringLength(10)]
        public string Private2Name { get; set; }

        public int Status { get; set; }

        public int UserId { get; set; }

        public DateTime CreateDate { get; set; }

        public int CreateUser { get; set; }

        public DateTime OnlineDate { get; set; }

        public DateTime UpdateDate { get; set; }

        public int UpdateUser { get; set; }
    }
}
