namespace Yintai.Hangzhou.Data.Models
{
    public partial class IMS_Combo : Yintai.Architecture.Common.Models.BaseEntity
    {
        public int Id { get; set; }
        public string Desc { get; set; }
        public decimal Price { get; set; }
        public string Private2Name { get; set; }
        public int Status { get; set; }
        public int UserId { get; set; }
        public System.DateTime CreateDate { get; set; }
        public int CreateUser { get; set; }
        public System.DateTime OnlineDate { get; set; }
        public System.DateTime UpdateDate { get; set; }
        public int UpdateUser { get; set; }

        public override object EntityId
        {
            get { return this.Id; }
        }
    }
}
