using System.Linq;
using Intime.OPC.Domain.Models;
using System.Collections.Generic;

namespace Intime.OPC.Repository.Support
{
    public class AccountRepository : IAccountRepository
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
        public bool Create(OPC_AuthUser user) {
            using (var db = new YintaiHZhouContext()) {
                db.OPC_AuthUser.Add(user);
                db.SaveChanges();
                return true;
            }
        }
        public bool Update(OPC_AuthUser user) 
        {
            using (var db = new YintaiHZhouContext()) {
              OPC_AuthUser userEnt =  db.OPC_AuthUser.Where(e => e.Id == user.Id).FirstOrDefault();
              if (userEnt != null) {
                  userEnt.Name = user.Name;
                  userEnt.OrgId = user.OrgId;
                  userEnt.Phone = user.Phone;
                  userEnt.Password = user.Password;
                  userEnt.SectionId = user.SectionId;
                  userEnt.LogonName = user.LogonName;
                  db.SaveChanges();
                  return true;
                  
              }
              return false;
            }
        }
        public bool Delete(int userId) {
            using (var db = new YintaiHZhouContext()) 
            {
                var user = db.OPC_AuthUser.Where(e => e.Id == userId).SingleOrDefault();
                if (user != null) {
                    db.OPC_AuthUser.Remove(user);
                    db.SaveChanges();
                    return true;
                }
                return false;
            }
        }
        public IList<OPC_AuthUser> Select() 
        {
            using (var db = new YintaiHZhouContext()) 
            {
                var userList = db.OPC_AuthUser.ToList();
                return userList;
            }
        }

        public bool IsStop(int userId,bool bValid) 
        {
            using (var db = new YintaiHZhouContext()) 
            {
                var user = db.OPC_AuthUser.Where(e => e.Id == userId).FirstOrDefault();
                if (user != null) 
                {
                    user.IsValid = bValid;
                    db.SaveChanges();
                    return true;
                }
                return false;
            }
        }


    }
}