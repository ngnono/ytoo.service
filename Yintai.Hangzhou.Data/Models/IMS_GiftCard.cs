namespace Yintai.Hangzhou.Data.Models
{
    public partial class IMS_GiftCard : Yintai.Architecture.Common.Models.BaseEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Status { get; set; }
        public System.DateTime UpdateDate { get; set; }

        public override object EntityId
        {
            get { return this.Id; }
        }
    }
}
