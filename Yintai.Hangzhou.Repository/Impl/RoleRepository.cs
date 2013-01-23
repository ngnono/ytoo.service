using System.Collections.Generic;
using System.Linq;
using Yintai.Hangzhou.Data.Models;
using Yintai.Hangzhou.Repository.Contract;

namespace Yintai.Hangzhou.Repository.Impl
{
    public class RoleRepository : RepositoryBase<RoleEntity, int>, IRoleRepository
    {
        public override RoleEntity GetItem(int key)
        {
            return base.Find(key);
        }

        public RoleEntity GetItemByRoleName(string roleName)
        {
            return base.Get(v => v.Name == roleName).SingleOrDefault();
        }

        public RoleEntity GetItemByRoleVal(int val)
        {
            return base.Get(v => v.Val == val).SingleOrDefault();
        }

        public List<RoleEntity> GetListByIds(List<int> ids)
        {
            return base.Get(v => ids.Any(s => s == v.Id)).ToList();
        }

        public List<RoleEntity> GetList()
        {
            return base.FindAll();
        }
    }
}
