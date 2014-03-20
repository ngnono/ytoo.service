using System;
using System.Linq;
using System.Linq.Expressions;
using Intime.OPC.Domain.Models;
using System.Collections.Generic;
using Intime.OPC.Repository.Base;

namespace Intime.OPC.Repository.Support
{
    public class AccountRepository : BaseRespository<OPC_AuthUser> ,IAccountRepository
    {
        public OPC_AuthUser Get(string userName, string password)
        {
            using (var db = new YintaiHZhouContext())
            {
                var query = db.OPC_AuthUser
                    .Where(b => (b.LogonName == userName && b.Password == password))
                    .Select(item => new OPC_AuthUser()
                    {
                        Id = item.Id,
                        LogonName = item.LogonName
                    });

                return query.FirstOrDefault();
            }
        }

        public bool SetEnable(int userId, bool enable)
        {
            using (var db = new YintaiHZhouContext())
            {
                var user = db.OPC_AuthUser.FirstOrDefault(t => t.Id == userId);
                if (user != null)
                {
                    user.IsValid = enable;
                    db.SaveChanges();
                    return true;
                }
                return false;
            }
        }
    }
}