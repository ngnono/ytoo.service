using System;
using System.Collections.Generic;
using System.Linq;
using Intime.OPC.Domain.Models;
using Intime.OPC.Repository.Base;

namespace Intime.OPC.Repository.Support
{
    public class RoleMenuRepository : BaseRepository<OPC_AuthRoleMenu>, IRoleMenuRepository
    {
        public bool DeleteByRoleMenu(int roleID)
        {
            using (var db = new YintaiHZhouContext())
            {
                var lst = db.OPC_AuthRoleMenu.Where(t => t.OPC_AuthRoleId == roleID);
                db.OPC_AuthRoleMenu.RemoveRange(lst);
                db.SaveChanges();
                return true;
            }
        }

        public bool AddMenus(int role,int userId, IEnumerable<int> menuIds)
        {
            using (var db = new YintaiHZhouContext())
            {
                foreach (var menuId in menuIds)
                {
                    var t = db.OPC_AuthRoleMenu.Create();
                    t.CreateUserId = userId;
                    t.CreateDate = DateTime.Now;
                    t.UpdateDate = t.CreateDate;
                    t.UpdateUserId = t.CreateUserId;
                    t.OPC_AuthRoleId = role;
                    t.OPC_AuthMenuId = menuId;
                    db.OPC_AuthRoleMenu.Add(t);
                }

                db.SaveChanges();
                return true;
            }
            return false;
        }
    }
}