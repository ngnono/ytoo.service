using OPCApp.Domain.Models;
using OPCApp.Infrastructure.DataService;

namespace OPCApp.DataService.Interface
{
    public interface IRoleDataService : IBaseDataService<OPC_AuthRole>
    {
      bool  SetIsEnable(OPC_AuthRole role);
    }
}