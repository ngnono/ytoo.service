using Intime.OPC.Infrastructure.Service;
using OPCApp.Domain.Models;
using OPCApp.Infrastructure.REST;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intime.OPC.Modules.Logistics.Services
{
    [Export(typeof(IService<OPC_Sale>))]
    public class SalesOrderService : IService<OPC_Sale>
    {
        public OPC_Sale Create(OPC_Sale obj)
        {
            throw new NotImplementedException();
        }

        public OPC_Sale Update(OPC_Sale obj)
        {
            throw new NotImplementedException();
        }

        public void Delete(int id)
        {
            throw new NotImplementedException();
        }

        public OPC_Sale Query(int id)
        {
            throw new NotImplementedException();
        }

        public PagedResult<OPC_Sale> Query(IQueryCriteria queryCriteria)
        {
            throw new NotImplementedException();
        }
    }
}
