namespace com.intime.fashion.data.ims
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class IMS_Associate
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public DateTime CreateDate { get; set; }

        public int CreateUser { get; set; }

        public int Status { get; set; }

        public int? TemplateId { get; set; }
    }
}
