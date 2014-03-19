using Intime.OPC.Domain.Models;
using System.Collections.Generic;

namespace Intime.OPC.Repository
{
    public interface IAccountRepository
    {
        OPC_AuthUser Get(string userName, string password);
        bool Create(OPC_AuthUser user);
        bool Update(OPC_AuthUser user);
        bool Delete(int userId);
        IList<OPC_AuthUser> Select();
        bool IsStop(int userId, bool bValid);
    }
}