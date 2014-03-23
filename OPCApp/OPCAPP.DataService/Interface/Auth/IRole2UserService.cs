using System.Collections.Generic;
using OPCApp.Domain.Models;
using OPCApp.Infrastructure.DataService;

namespace OPCApp.DataService.Interface
{
    public interface IRole2UserService
    {
        ResultMsg SetUserByRole(int roleId,List<int> listUserId);
        List<OPC_AuthUser> GetUserListByRole(int roleId);
    }
}
