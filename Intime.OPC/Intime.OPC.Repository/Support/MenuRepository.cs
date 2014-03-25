using System;
using System.Linq;
using System.Linq.Expressions;
using Intime.OPC.Domain.Models;
using System.Collections.Generic;
using Intime.OPC.Repository.Base;

namespace Intime.OPC.Repository.Support
{
    public class MenuRepository : BaseRepository<OPC_AuthMenu>, IMenuRepository
    {
        public IEnumerable<OPC_AuthMenu> GetMenusByUserID(int userID)
        {
            using (var db = new YintaiHZhouContext())
            {
                var lstRoleUser = db.OPC_AuthRoleUser.Where(t => t.OPC_AuthUserId == userID);
                IList<OPC_AuthMenu> menus = new List<OPC_AuthMenu>();

                foreach (var roleUser in lstRoleUser)
                {
                    var lstMenu = GetMenusByRoleID(roleUser.OPC_AuthRoleId);
                    foreach (var menu in lstMenu)
                    {
                        if (!menus.Contains(menu))
                        {
                            menus.Add(menu);
                        }
                    }
                }
                return menus;
            }
        }


        public IEnumerable<OPC_AuthMenu> GetMenusByRoleID(int roleID)
        {
            using (var db = new YintaiHZhouContext())
            {
                var lstRoleMenu = db.OPC_AuthRoleMenu.Where(t => t.OPC_AuthRoleId == roleID);
                IList<OPC_AuthMenu> lstMenu = new List<OPC_AuthMenu>();

                foreach (var opcAuthRoleMenu in lstRoleMenu)
                {
                    var menu = db.OPC_AuthMenu.FirstOrDefault(t => t.Id == opcAuthRoleMenu.OPC_AuthMenuId);
                    if (menu != null && menu.IsValid)
                    {
                        lstMenu.Add(menu);
                    }
                }
                return lstMenu;
            }
        }
    }
}