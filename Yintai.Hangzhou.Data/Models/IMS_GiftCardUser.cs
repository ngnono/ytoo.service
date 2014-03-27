namespace Yintai.Hangzhou.Data.Models
{
    public partial class IMS_GiftCardUser : Yintai.Architecture.Common.Models.BaseEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string GiftCardAccount { get; set; }
        public System.DateTime CreateDate { get; set; }
        public int CreateUser { get; set; }

        public override object EntityId
        {
            get { return this.Id; }
        }
    }
}
