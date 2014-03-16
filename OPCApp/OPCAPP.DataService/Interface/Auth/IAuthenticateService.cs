using OPCApp.Domain.Models;
namespace OPCApp.DataService.Interface
{
    public interface IAuthenticateService : OPCApp.Infrastructure.DataService.IBaseDataService<OPC_AuthUser>
    {
       string Login(string userName,string password);
       bool SetIsStop(int userId,bool isStop);
    }
}
