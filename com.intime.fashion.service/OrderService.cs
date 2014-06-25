using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yintai.Architecture.Common.Logger;
using Yintai.Hangzhou.Data.Models;
using Yintai.Hangzhou.Model.Enums;

namespace com.intime.fashion.service
{
    public class OrderService:BusinessServiceBase
    {
        public OrderService(ILog log, DbContext db):base(log,db)
        {
        }

        public bool CanChangePro(OrderEntity order)
        {
            return order.Status == (int)OrderStatus.AgentConfirmed ||
                    order.Status == (int)OrderStatus.Paid;
        }

        public bool IsAssociateOrder(int authuid,OrderEntity order)
        {
            var context = GetContext();
            var associateIncome = context.Set<IMS_AssociateIncomeHistoryEntity>()
                                    .Where(iair => iair.AssociateUserId == authuid
                                            && iair.SourceType == (int)AssociateOrderType.Product
                                            && iair.SourceNo == order.OrderNo)
                                    .FirstOrDefault();
            return associateIncome != null;
        }
    }
}
