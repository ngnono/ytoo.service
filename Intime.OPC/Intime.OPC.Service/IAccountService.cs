using System.Collections.Generic;
using Intime.OPC.Domain;
using Intime.OPC.Domain.Models;

namespace Intime.OPC.Service
{
    public interface IAccountService : IService
    {
        OPC_AuthUser Get(string userName, string password);
        bool Create(OPC_AuthUser user);
        bool Update(OPC_AuthUser user);
        bool Delete(int userId);
        PageResult<OPC_AuthUser> Select(int pageIndex, int pageSize = 20);
        bool IsStop(int userId, bool bValid);

        PageResult<OPC_AuthUser> GetUsersByRoleID(int roleId, int pageIndex, int pageSize = 20);
    }
}