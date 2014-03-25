using System.Collections.Generic;
using System.Linq;
using Intime.OPC.Domain.Models;
using Intime.OPC.Repository.Base;

namespace Intime.OPC.Repository.Support
{
    public class MenuRepository : BaseRepository<OPC_AuthMenu>, IMenuRepository
    {
        #region IMenuRepository Members

        public IList<OPC_AuthMenu> GetMenusByUserID(int userId)
        {
            using (var db = new YintaiHZhouContext())
            {
                return
                    db.OPC_AuthMenu.Join(
                        db.OPC_AuthRoleMenu.Join(db.OPC_AuthRoleUser.Where(u => u.OPC_AuthUserId == userId),
                            arm => arm.OPC_AuthRoleId, aru => aru.OPC_AuthRoleId, (arm, aru) => arm), m => m.Id,
                        arm => arm.OPC_AuthMenuId, (m, arm) => m).ToList();
            }
        }

        public IList<OPC_AuthMenu> GetMenusByRoleID(int roleID)
        {
            using (var db = new YintaiHZhouContext())
            {
              return   db.OPC_AuthRoleMenu.Where(t => t.OPC_AuthRoleId == roleID).Join(
                    db.OPC_AuthMenu,
                    t => t.OPC_AuthMenuId,
                    o => o.Id, (arm, aru) => aru).ToList();
            }
        }

        #endregion
    }
}