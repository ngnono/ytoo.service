using System.Collections.Generic;
using System.Linq;
using Intime.OPC.Domain.Dto;
using Intime.OPC.Domain.Models;
using Intime.OPC.Repository.Base;

namespace Intime.OPC.Repository.Support
{
    public class OrderItemRepository : BaseRepository<OrderItem>, IOrderItemRepository
    {
        public IList<OrderItem> GetByOrderNo(string orderNo)
        {
            using (var db = new YintaiHZhouContext())
            {
                return db.OrderItems.Where(t => t.OrderNo == orderNo).ToList();
            }
        }

        public IList<OrderItem> GetByIDs(IEnumerable<int> ids)
        {
            return Select(t => ids.Contains(t.Id));
        }
    }
}