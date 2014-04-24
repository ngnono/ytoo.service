using OPCApp.Domain.Models;
using OPCApp.Infrastructure.DataService;

namespace OPCApp.DataService.Interface
{
    public interface IStoreDataService : IBaseDataService<Store>
    {
        bool SetIsStop(int StoreId, bool isStop);
    }
}