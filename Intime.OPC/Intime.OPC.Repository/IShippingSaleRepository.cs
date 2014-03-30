using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Intime.OPC.Domain.Models;

namespace Intime.OPC.Repository
{
    public  interface IShippingSaleRepository:IRepository<OPC_ShippingSale>
    {
        IList<OPC_ShippingSale> GetBySaleOrderNo(string saleNo);

        IList<OPC_ShippingSale> GetByShippingCode(string shippingCode);

        IList<OPC_ShippingSale> Get(string shippingCode, DateTime startTime, DateTime endTime);
    }
}
