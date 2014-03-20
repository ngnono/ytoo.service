using System;
using System.Linq;
using Intime.OPC.Domain.Models;
using System.Collections.Generic;
using Intime.OPC.Repository.Base;

namespace Intime.OPC.Repository.Support
{
    public class RoleRepository : BaseRespository<OPC_AuthRole>, IRoleRepository
    {
        
        public bool SetEnable(int roleID, bool enable)
        {
            using (var db = new YintaiHZhouContext())
            {
                var ent = db.OPC_AuthRole.FirstOrDefault(t => t.Id == roleID);
                if (ent != null)
                {
                    ent.IsValid = enable;
                    try
                    {
                        db.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        return false;
                    }

                    return true;
                }
                return false;
            }
        }
    }
}