namespace com.intime.fashion.data.ims
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class IMS_AssociateIncomeRuleFix
    {
        public int Id { get; set; }

        public int RuleId { get; set; }

        public decimal FixAmount { get; set; }
    }
}
