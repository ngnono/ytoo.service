using System;
using System.Linq;
using System.Linq.Expressions;
using Intime.OPC.Domain.Models;
using System.Collections.Generic;

namespace Intime.OPC.Repository
{
    public interface IAccountRepository:IRespository<OPC_AuthUser>
    {
        OPC_AuthUser Get(string userName, string password);
        //bool Create(OPC_AuthUser user);
        //bool Update(OPC_AuthUser user);
        //bool Delete(int userId);
        //IQueryable<OPC_AuthUser> Select(Expression<Func<OPC_AuthUser, bool>> filter);
        bool SetEnable(int userId, bool enable);
    }
}