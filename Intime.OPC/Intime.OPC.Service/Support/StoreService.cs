using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intime.OPC.Domain.Models;
using Intime.OPC.Repository;

namespace Intime.OPC.Service.Support
{
    public class StoreService : BaseService<Store>, IStoreService
    {
        public StoreService(IStoreRepository repository):base(repository)
        {
        }

        public IList<Store> GetAll()
        {
            return _repository.GetAll(0,1000).Result;
        }
    }
}
