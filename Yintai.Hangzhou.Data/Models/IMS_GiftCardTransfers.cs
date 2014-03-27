namespace Yintai.Hangzhou.Data.Models
{
    public partial class IMS_GiftCardTransfers : Yintai.Architecture.Common.Models.BaseEntity
    {
        public int Id { get; set; }
        public int GiftCardId { get; set; }
        public int FromUserId { get; set; }
        public int ToUserId { get; set; }
        public string Phone { get; set; }
        public string Comment { get; set; }
        public int IsActive { get; set; }
        public int IsDecline { get; set; }
        public int PreTransferId { get; set; }
        public System.DateTime CreateDate { get; set; }
        public int CreateUser { get; set; }

        public override object EntityId
        {
            get { return this.Id; }
        }
    }
}
