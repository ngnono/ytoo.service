namespace com.intime.fashion.data.ims
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class IMS_AssociateItems
    {
        public int Id { get; set; }

        public int AssociateId { get; set; }

        public int ItemType { get; set; }

        public int ItemId { get; set; }

        public int Status { get; set; }

        public DateTime CreateDate { get; set; }

        public int CreateUser { get; set; }

        public int UpdateUser { get; set; }

        public DateTime UpdateDate { get; set; }
    }
}
