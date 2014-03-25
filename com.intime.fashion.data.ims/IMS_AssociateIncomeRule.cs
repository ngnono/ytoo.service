namespace com.intime.fashion.data.ims
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class IMS_AssociateIncomeRule
    {
        public int Id { get; set; }

        public int CategoryId { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime EndDate { get; set; }

        public int RuleType { get; set; }

        public int Status { get; set; }
    }
}
