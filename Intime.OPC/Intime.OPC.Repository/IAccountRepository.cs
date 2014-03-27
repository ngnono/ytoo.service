using System.Collections.Generic;
using Intime.OPC.Domain.Models;

namespace Intime.OPC.Repository
{
    public interface IAccountRepository : IRepository<OPC_AuthUser>
    {
        OPC_AuthUser Get(string userName, string password);
        IList<OPC_AuthUser> All();
        bool SetEnable(int userId, bool enable);

        IList<OPC_AuthUser> GetByRoleId(int roleId);
    }
}