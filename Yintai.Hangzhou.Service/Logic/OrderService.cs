using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yintai.Hangzhou.Data.Models;
using Yintai.Hangzhou.Model.Enums;

namespace Yintai.Hangzhou.Service.Logic
{
    public class OrderService:BusinessServiceBase
    {
        public OrderEntity _innerModel = null;
        public OrderService(OrderEntity order)
        {
            _innerModel = order;
        }


        public bool CanChangePro()
        {
            return _innerModel.Status == (int)OrderStatus.AgentConfirmed ||
                    _innerModel.Status == (int)OrderStatus.Paid;
        }

        public bool IsAssociateOrder(int authuid)
        {
            var context = GetContext();
            var associateIncome = context.Set<IMS_AssociateIncomeHistoryEntity>()
                                    .Where(iair => iair.AssociateUserId == authuid
                                            && iair.SourceType == (int)AssociateOrderType.Product
                                            && iair.SourceNo == _innerModel.OrderNo)
                                    .FirstOrDefault();
            return associateIncome != null;
        }
    }
}
