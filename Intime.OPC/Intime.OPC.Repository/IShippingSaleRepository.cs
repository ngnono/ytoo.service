using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intime.OPC.Domain;
using Intime.OPC.Domain.Models;

namespace Intime.OPC.Repository
{
    public  interface IShippingSaleRepository:IRepository<OPC_ShippingSale>
    {
        PageResult<OPC_ShippingSale> GetBySaleOrderNo(string saleNo,int pageIndex, int pageSize = 20);

        PageResult<OPC_ShippingSale> GetByShippingCode(string shippingCode, int pageIndex, int pageSize = 20);

        PageResult<OPC_ShippingSale> Get(string shippingCode, DateTime startTime, DateTime endTime, int shippingStatus, int pageIndex, int pageSize = 20);
    }
}
