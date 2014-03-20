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
        //public bool Create(OPC_AuthUser user) {
        //    using (var db = new YintaiHZhouContext()) {
        //        db.OPC_AuthUser.Add(user);
        //        db.SaveChanges();
        //        return true;
        //    }
        //}
        //public bool Update(OPC_AuthUser user) 
        //{
        //    using (var db = new YintaiHZhouContext()) {
        //      OPC_AuthUser userEnt =  db.OPC_AuthUser.Where(e => e.Id == user.Id).FirstOrDefault();
        //      if (userEnt != null) {
        //          userEnt.Name = user.Name;
        //          userEnt.OrgId = user.OrgId;
        //          userEnt.Phone = user.Phone;
        //          userEnt.Password = user.Password;
        //          userEnt.SectionId = user.SectionId;
        //          userEnt.LogonName = user.LogonName;
        //          db.SaveChanges();
        //          return true;
                  
        //      }
        //      return false;
        //    }
        //}
        //public bool Delete(int userId) {
        //    using (var db = new YintaiHZhouContext()) 
        //    {
        //        var user = db.OPC_AuthUser.Where(e => e.Id == userId).SingleOrDefault();
        //        if (user != null) {
        //            db.OPC_AuthUser.Remove(user);
        //            db.SaveChanges();
        //            return true;
        //        }
        //        return false;
        //    }
        //}
        //public IQueryable<OPC_AuthUser> Select(Expression<Func<OPC_AuthUser, bool>> filter) 
        //{
        //    using (var db = new YintaiHZhouContext())
        //    {
        //        return db.OPC_AuthUser.Where(filter);
               
        //    }
        //}


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