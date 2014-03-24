using System.Collections.Generic;
using System.Linq;
using Intime.OPC.Domain.Models;
using Intime.OPC.Repository.Base;

namespace Intime.OPC.Repository.Support
{
    public class AccountRepository : BaseRepository<OPC_AuthUser>, IAccountRepository
    {
        #region IAccountRepository Members

        public OPC_AuthUser Get(string userName, string password)
        {
            using (var db = new YintaiHZhouContext())
            {
                return db.OPC_AuthUser.FirstOrDefault(t => t.LogonName == userName && t.Password == password);
            }
        }

        public bool SetEnable(int userId, bool enable)
        {
            using (var db = new YintaiHZhouContext())
            {
                OPC_AuthUser user = db.OPC_AuthUser.FirstOrDefault(t => t.Id == userId);
                if (user != null)
                {
                    user.IsValid = enable;
                    db.SaveChanges();
                    return true;
                }
                return false;
            }
        }

        public IList<OPC_AuthUser> All()
        {
            return Select(t => t.IsValid == true);
        }

        #endregion
    }
}