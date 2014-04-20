using Intime.OPC.Domain.Models;

namespace Intime.OPC.Job.Order.OrderStatusSync
{
    public class CreateOrderNotificationEntity:AbstractOrderNotificationEntity
    {
        public CreateOrderNotificationEntity(OPC_Sale saleOrder) : base(saleOrder)
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