namespace Yintai.Hangzhou.Data.Models
{
    public partial class IMS_AssociateIncomeHistory : Yintai.Architecture.Common.Models.BaseEntity
    {
        public int Id { get; set; }
        public int SourceType { get; set; }
        public string SourceNo { get; set; }
        public int AssociateUserId { get; set; }
        public decimal AssociateIncome { get; set; }
        public int Status { get; set; }
        public System.DateTime CreateDate { get; set; }
        public System.DateTime UpdateDate { get; set; }

        public override object EntityId
        {
            get { return this.Id; }
        }
    }
}
