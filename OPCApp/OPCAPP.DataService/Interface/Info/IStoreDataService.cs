using Intime.OPC.Domain.Models;
namespace OPCApp.DataService.Interface
{
    public interface IStoreDataService : OPCApp.Infrastructure.DataService.IBaseDataService<Store>
    {
       bool SetIsStop(int StoreId,bool isStop);
    }
}
