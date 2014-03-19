using Intime.OPC.Domain.Models;
using System.Collections.Generic;

namespace Intime.OPC.Service
{
    public interface IAccountService
    {
        OPC_AuthUser Get(string userName, string password);
        bool Create(OPC_AuthUser user);
        bool Update(OPC_AuthUser user);
        bool Delete(int userId);
        IList<OPC_AuthUser> Select();
        bool IsStop(int userId, bool bValid);
    }
}