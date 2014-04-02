using Intime.OPC.Domain;
using Intime.OPC.Domain.Models;

namespace Intime.OPC.Repository
{
    public interface IAccountRepository : IRepository<OPC_AuthUser>
    {
        OPC_AuthUser Get(string userName, string password);
        PageResult<OPC_AuthUser> All(int pageIndex, int pageSize = 20);
        bool SetEnable(int userId, bool enable);

        PageResult<OPC_AuthUser> GetByRoleId(int roleId, int pageIndex, int pageSize);
        PageResult<OPC_AuthUser> GetByLoginName(string orgID, string loginName, int pageIndex, int pageSize);

        PageResult<OPC_AuthUser> GetByOrgId(string orgID, string name, int pageIndex, int pageSize);
    }
}