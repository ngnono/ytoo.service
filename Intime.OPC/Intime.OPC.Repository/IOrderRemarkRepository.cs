using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intime.OPC.Domain.Models;

namespace Intime.OPC.Repository
{
    public interface IOrderRemarkRepository:IRepository<OPC_OrderComment>
    {
        IList<OPC_OrderComment> GetByOrderNo(string orderNo);
    }
}
