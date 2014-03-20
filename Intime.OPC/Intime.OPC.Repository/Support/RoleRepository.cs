using System;
using System.Linq;
using Intime.OPC.Domain.Models;
using System.Collections.Generic;

namespace Intime.OPC.Repository.Support
{
    public class RoleRepository : IRoleRepository
    {
        
     
        public bool Create(OPC_AuthRole role) {
            using (var db = new YintaiHZhouContext()) {
                db.OPC_AuthRole.Add(role);
                db.SaveChanges();
                return true;
            }
        }
        public bool Update(OPC_AuthRole role) 
        {
            using (var db = new YintaiHZhouContext()) {
               var ent = db.OPC_AuthRole.Where(e => e.Id == role.Id).FirstOrDefault();
               if (ent != null)
               {
                   ent.Name = ent.Name;
                   ent.IsValid =false;
                   ent.Description =ent.Description;
                   db.SaveChanges();
                  return true;
                  
              }
              return false;
            }
        }
        public bool Delete(int roleId) {
            using (var db = new YintaiHZhouContext()) 
            {
                var role = db.OPC_AuthRole.Where(e => e.Id == roleId).SingleOrDefault();
                if (role != null)
                {
                    db.OPC_AuthRole.Remove(role);
                    db.SaveChanges();
                    return true;
                }
                return false;
            }
        }
        public IList<OPC_AuthRole> Select() 
        {
            using (var db = new YintaiHZhouContext()) 
            {
                var list = db.OPC_AuthRole.ToList();
                return list;
            }
        }

        public bool IsStop(int roleId,bool bValid) 
        {
            using (var db = new YintaiHZhouContext()) 
            {
                var ent = db.OPC_AuthUser.Where(e => e.Id == roleId).FirstOrDefault();
                if (ent != null) 
                {
                    ent.IsValid = bValid;
                    db.SaveChanges();
                    return true;
                }
                return false;
            }
        }


    }
}