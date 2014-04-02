using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Intime.OPC.Domain;
using Intime.OPC.Domain.Exception;
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

       public  PageResult<OPC_AuthUser> GetByRoleId(int roleId, int pageIndex, int pageSize)
        {
            using (var db = new YintaiHZhouContext())
            {
                IQueryable<OPC_AuthUser> lst = db.OPC_AuthRoleUser.Where(t => t.OPC_AuthRoleId == roleId)
                    .Join(db.OPC_AuthUser.Where(t => t.IsValid == true), t => t.OPC_AuthUserId, o => o.Id, (t, o) => o);
                return lst.ToPageResult(pageIndex, pageSize);
            }
        }

       public PageResult<OPC_AuthUser> All(int pageIndex, int pageSize = 20)
        {
            return Select(t => t.IsValid == true,t=>t.Name,true,pageIndex,pageSize);
        }

        #endregion


       public PageResult<OPC_AuthUser> GetByOrgId(string orgID, int pageIndex, int pageSize)
       {
           return Select2<OPC_AuthUser, string>(t => t.IsValid==true && t.OrgId == orgID, o => o.Name, true, pageIndex,
               pageSize);
       }

       public PageResult<OPC_AuthUser> GetByLoginName(string orgID, string loginName, int pageIndex, int pageSize)
       {
           Expression<Func<OPC_AuthUser, bool>> filter = t =>t.IsValid==true && t.LogonName.Contains(loginName) ;
           if (!string.IsNullOrEmpty(orgID))
           {
               filter = filter.And(t=>t.OrgId==orgID);
           }
           return  Select(filter, s => s.LogonName, true, pageIndex, pageSize);
       }

        public PageResult<OPC_AuthUser> GetByOrgId(string orgID, string name, int pageIndex, int pageSize)
        {
            Expression<Func<OPC_AuthUser, bool>> filter = t => t.IsValid == true && t.Name.Contains(name);
            if (!string.IsNullOrEmpty(orgID))
            {
                filter = filter.And(t => t.OrgId == orgID);
            }
            return Select(filter, s => s.Name, true, pageIndex, pageSize);
        }
    }
}