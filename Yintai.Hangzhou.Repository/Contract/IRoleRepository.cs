using System.Collections.Generic;
using Yintai.Hangzhou.Data.Models;

namespace Yintai.Hangzhou.Repository.Contract
{
    public interface IRoleRepository : IRepository<RoleEntity, int>
    {
        RoleEntity GetItemByRoleName(string roleName);

        RoleEntity GetItemByRoleVal(int val);

        List<RoleEntity> GetListByIds(List<int> ids);

        List<RoleEntity> GetList();
    }
}
