using System.Collections.Generic;
using Intime.OPC.Domain.Models;

namespace Intime.OPC.Service
{
    public interface IAccountService : IService
    {
        OPC_AuthUser Get(string userName, string password);
        bool Create(OPC_AuthUser user);
        bool Update(OPC_AuthUser user);
        bool Delete(int userId);
        IList<OPC_AuthUser> Select();
        bool IsStop(int userId, bool bValid);

        IList<OPC_AuthUser> GetUsersByRoleID(int roleId);
    }
}