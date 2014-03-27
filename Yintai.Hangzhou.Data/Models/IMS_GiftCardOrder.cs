namespace Yintai.Hangzhou.Data.Models
{
    public partial class IMS_GiftCardOrder : Yintai.Architecture.Common.Models.BaseEntity
    {
        public int Id { get; set; }
        public string No { get; set; }
        public int GiftCardId { get; set; }
        public decimal Amount { get; set; }
        public int PurchaseUserId { get; set; }
        public int OwnerUserId { get; set; }
        public int Status { get; set; }
        public System.DateTime CreateDate { get; set; }
        public int CreateUser { get; set; }

        public override object EntityId
        {
            get { return this.Id; }
        }
    }
}
