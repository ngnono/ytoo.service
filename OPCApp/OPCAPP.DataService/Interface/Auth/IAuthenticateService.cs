using OPCApp.Domain.Models;
using OPCApp.Infrastructure.DataService;

namespace OPCApp.DataService.Interface
{
    public interface IAuthenticateService : IBaseDataService<OPC_AuthUser>
    {
        string Login(string userName, string password);
        bool SetIsStop(int userId, bool isStop);
    }
}