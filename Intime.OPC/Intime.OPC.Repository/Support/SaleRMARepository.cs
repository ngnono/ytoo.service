using System.Linq;
using Intime.OPC.Domain.Models;
using Intime.OPC.Repository.Base;

namespace Intime.OPC.Repository.Support
{
    public class SaleRMARepository : BaseRepository<OPC_SaleRMA>, ISaleRMARepository
    {
        public int Count(string orderNo)
        {
            using (var db = new YintaiHZhouContext())
            {
                return db.OPC_SaleRMA.Count(t => t.OrderNo == orderNo);
            }
        }
    }
}