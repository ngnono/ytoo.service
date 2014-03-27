namespace Yintai.Hangzhou.Data.Models
{
    public partial class IMS_AssociateIncomeRule : Yintai.Architecture.Common.Models.BaseEntity
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public System.DateTime FromDate { get; set; }
        public System.DateTime EndDate { get; set; }
        public int RuleType { get; set; }
        public int Status { get; set; }

        public override object EntityId
        {
            get { return this.Id; }
        }
    }
}
