using Yintai.Hangzhou.Model.Enums;

namespace Yintai.Hangzhou.Data.Models
{
    public partial class IMS_GiftCardRecharge : Yintai.Architecture.Common.Models.BaseEntity
    {
        public int Id { get; set; }
        public int ChargeUserId { get; set; }
        public string PurchaseId { get; set; }
        public string ChargePhone { get; set; }
        public System.DateTime CreateDate { get; set; }
        public int CreateUser { get; set; }

        public override object EntityId
        {
            get { return this.Id; }
        }
    }
}
