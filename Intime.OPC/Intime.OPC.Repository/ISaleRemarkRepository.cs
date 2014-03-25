using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intime.OPC.Domain.Models;

namespace Intime.OPC.Repository
{
    public interface ISaleRemarkRepository:IRepository<OPC_SaleComment>
    {
        IList<OPC_SaleComment> GetBySaleOrderNo(string saleOrderNo);
    }
}
