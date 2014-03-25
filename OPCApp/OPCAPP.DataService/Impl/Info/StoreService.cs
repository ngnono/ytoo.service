using System.Collections.Generic;
using System.ComponentModel.Composition;
using OPCApp.DataService.Interface;
using OPCApp.Infrastructure;
using OPCApp.Infrastructure.DataService;

namespace OPCApp.DataService.Impl.Info
{
    [Export(typeof(IStoreDataService))]
    public class StoreDataService : IStoreDataService
    {
        public bool SetIsStop(int StoreId, bool isStop)
        {
            throw new System.NotImplementedException();
        }

        public ResultMsg Add(Intime.OPC.Domain.Models.Store model)
        {
            throw new System.NotImplementedException();
        }

        public ResultMsg Edit(Intime.OPC.Domain.Models.Store model)
        {
            throw new System.NotImplementedException();
        }

        public ResultMsg Delete(Intime.OPC.Domain.Models.Store model)
        {
            throw new System.NotImplementedException();
        }

        public PageResult<Intime.OPC.Domain.Models.Store> Search(IDictionary<string, object> iDicFilter)
        {
            throw new System.NotImplementedException();
        }
    }
}
