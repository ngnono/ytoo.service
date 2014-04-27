using System.Collections.Generic;
using Intime.OPC.Domain;
using Intime.OPC.Domain.Models;

namespace Intime.OPC.Repository
{
    public interface IRoleRepository : IRepository<OPC_AuthRole>
    {
        PageResult<OPC_AuthRole> All(int pageIndex, int pageSize = 20);
        PageResult<OPC_AuthRole> GetByUserID(int userID, int pageIndex, int pageSize = 20);
        bool SetEnable(int roleID, bool enable);
    }
}