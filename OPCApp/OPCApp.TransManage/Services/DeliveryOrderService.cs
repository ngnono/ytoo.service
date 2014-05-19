using Intime.OPC.Infrastructure.Service;
using OPCApp.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intime.OPC.Modules.Logistics.Services
{
    public class DeliveryOrderService : IService<OPC_ShippingSale>
    {
        public OPC_ShippingSale Create(OPC_ShippingSale obj)
        {
            throw new NotImplementedException();
        }

        public OPC_ShippingSale Update(OPC_ShippingSale obj)
        {
            throw new NotImplementedException();
        }

        public void Delete(int id)
        {
            throw new NotImplementedException();
        }

        public OPC_ShippingSale Query(int id)
        {
            throw new NotImplementedException();
        }

        public OPCApp.Infrastructure.REST.PagedResult<OPC_ShippingSale> Query(IQueryCriteria queryCriteria)
        {
            throw new NotImplementedException();
        }
    }
}
