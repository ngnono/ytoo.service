namespace Yintai.Hangzhou.Data.Models
{
    public partial class IMS_SalesCode : Yintai.Architecture.Common.Models.BaseEntity
    {
        public int Id { get; set; }
        public int SectionId { get; set; }
        public string Code { get; set; }

        public override object EntityId
        {
            get { return this.Id; }
        }
    }
}
