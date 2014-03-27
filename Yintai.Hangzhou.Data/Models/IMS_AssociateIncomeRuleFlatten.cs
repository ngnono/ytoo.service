namespace Yintai.Hangzhou.Data.Models
{
    public partial class IMS_AssociateIncomeRuleFlatten : Yintai.Architecture.Common.Models.BaseEntity
    {
        public int Id { get; set; }
        public int RuleId { get; set; }
        public decimal Percentage { get; set; }

        public override object EntityId
        {
            get { return this.Id; }
        }
    }
}
