namespace Yintai.Hangzhou.Data.Models
{
    public partial class IMS_AssociateIncome : Yintai.Architecture.Common.Models.BaseEntity
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal AvailableAmount { get; set; }
        public decimal RequestAmount { get; set; }
        public decimal ReceivedAmount { get; set; }
        public int Status { get; set; }
        public System.DateTime CreateDate { get; set; }
        public System.DateTime UpdateDate { get; set; }

        public override object EntityId
        {
            get { return this.Id; }
        }
    }
}
