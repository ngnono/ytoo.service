namespace Yintai.Hangzhou.Data.Models
{
    public partial class IMS_GiftCardItem : Yintai.Architecture.Common.Models.BaseEntity
    {
        public int Id { get; set; }
        public int GiftCardId { get; set; }
        public decimal Price { get; set; }
        public decimal UnitPrice { get; set; }
        public int MaxLimit { get; set; }
        public int Status { get; set; }

        public override object EntityId
        {
            get { return this.Id; }
        }
    }
}
