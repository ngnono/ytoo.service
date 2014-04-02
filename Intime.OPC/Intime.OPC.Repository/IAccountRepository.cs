using System.Collections.Generic;
using Intime.OPC.Domain;
using Intime.OPC.Domain.Dto;
using Intime.OPC.Domain.Models;

namespace Intime.OPC.Repository
{
    public interface IAccountRepository : IRepository<OPC_AuthUser>
    {
        OPC_AuthUser Get(string userName, string password);
        PageResult<OPC_AuthUser> All(int pageIndex, int pageSize = 20);
        bool SetEnable(int userId, bool enable);

        PageResult<OPC_AuthUser> GetByRoleId(int roleId,int pageIndex,int pageSize=20);

    }
}