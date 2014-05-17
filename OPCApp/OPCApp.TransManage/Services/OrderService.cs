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
    [Export(typeof(IService<Order>))]
    public class OrderService : IService<Order>
    {
        public Order Create(Order obj)
        {
            throw new NotImplementedException();
        }

        public Order Update(Order obj)
        {
            throw new NotImplementedException();
        }

        public void Delete(int id)
        {
            throw new NotImplementedException();
        }

        public Order Query(int id)
        {
            throw new NotImplementedException();
        }

        public PagedResult<Order> Query(IQueryCriteria queryCriteria)
        {
            throw new NotImplementedException();
        }
    }
}
