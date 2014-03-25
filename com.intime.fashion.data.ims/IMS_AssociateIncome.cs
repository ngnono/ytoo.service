namespace com.intime.fashion.data.ims
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class IMS_AssociateIncome
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public decimal TotalAmount { get; set; }

        public decimal AvailableAmount { get; set; }

        public decimal RequestAmount { get; set; }

        public decimal ReceivedAmount { get; set; }

        public int Status { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime UpdateDate { get; set; }
    }
}
