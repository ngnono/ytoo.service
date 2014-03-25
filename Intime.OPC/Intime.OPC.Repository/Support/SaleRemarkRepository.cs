using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intime.OPC.Domain.Models;
using Intime.OPC.Repository.Base;

namespace Intime.OPC.Repository.Support
{
    public class SaleRemarkRepository : BaseRepository<OPC_SaleComment>, ISaleRemarkRepository
    {
        public IList<OPC_SaleComment> GetBySaleOrderNo(string saleOrderNo)
        {
            return Select(t => t.SaleOrderNo == saleOrderNo);
        }
    }
}
