namespace Yintai.Hangzhou.Data.Models
{
    public partial class IMS_AssociateIncomeRequest : Yintai.Architecture.Common.Models.BaseEntity
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string BankName { get; set; }
        public string BankNo { get; set; }
        public decimal Amount { get; set; }
        public int Status { get; set; }
        public System.DateTime CreateDate { get; set; }
        public System.DateTime UpdateDate { get; set; }

        public override object EntityId
        {
            get { return this.Id; }
        }
    }
}
