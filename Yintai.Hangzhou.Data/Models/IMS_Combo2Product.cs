namespace Yintai.Hangzhou.Data.Models
{
    public partial class IMS_Combo2Product : Yintai.Architecture.Common.Models.BaseEntity
    {
        public int Id { get; set; }
        public int ComboId { get; set; }
        public int ProductId { get; set; }

        public override object EntityId
        {
            get { return this.Id; }
        }
    }
}
