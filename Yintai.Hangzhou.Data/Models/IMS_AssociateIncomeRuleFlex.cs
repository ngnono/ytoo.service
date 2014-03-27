using System;

namespace Yintai.Hangzhou.Data.Models
{
    public partial class IMS_AssociateIncomeRuleFlex : Yintai.Architecture.Common.Models.BaseEntity
    {
        public int Id { get; set; }
        public int RuleId { get; set; }
        public decimal BenchFrom { get; set; }
        public decimal BenchTo { get; set; }
        public Nullable<decimal> Percentage { get; set; }

        public override object EntityId
        {
            get { return this.Id; }
        }
    }
}
