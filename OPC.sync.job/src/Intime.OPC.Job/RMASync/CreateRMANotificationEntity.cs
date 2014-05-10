using Intime.OPC.Domain.Models;

namespace Intime.OPC.Job.RMASync
{
    public class CreateRMANotificationEntity:AbstractRMANotificationEntity
    {
        public CreateRMANotificationEntity(OPC_SaleRMA saleRMA) : base(saleRMA)
        {
        }

        public override NotificationStatus Status
        {
            get { return NotificationStatus.Create; }
        }

        public override NotificationType Type
        {
            get { return NotificationType.Create; }
        }

        public override string PaymentType
        {
            get { return "C0"; }
        }
    }
}