using System;

namespace Yintai.Hangzhou.Data.Models
{
    public partial class IMS_Associate : Yintai.Architecture.Common.Models.BaseEntity
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public System.DateTime CreateDate { get; set; }
        public int CreateUser { get; set; }
        public int Status { get; set; }
        public Nullable<int> TemplateId { get; set; }

        public override object EntityId
        {
            get { return this.Id; }
        }
    }
}
