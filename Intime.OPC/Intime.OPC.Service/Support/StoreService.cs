using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intime.OPC.Domain.Models;
using Intime.OPC.Repository;

namespace Intime.OPC.Service.Support
{
   public  class StoreService:IStoreService
    {
        private IStoreRepository _storeRepository;
        public StoreService(IStoreRepository repository)
        {
            _storeRepository = repository;
        }

        public IList<Store> GetAll()
        {
            return _storeRepository.GetAll();
        }
    }
}
