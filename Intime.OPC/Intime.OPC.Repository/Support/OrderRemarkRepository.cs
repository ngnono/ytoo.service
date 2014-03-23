using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intime.OPC.Domain.Models;
using Intime.OPC.Repository.Base;

namespace Intime.OPC.Repository.Support
{
    public class OrderRemarkRepository : BaseRepository<OPC_OrderComment>, IOrderRemarkRepository
    {

        public IList<OPC_OrderComment> GetByOrderNo(string orderNo)
        {
            return Select(t => t.OrderNo == orderNo);
        }
    }
}
