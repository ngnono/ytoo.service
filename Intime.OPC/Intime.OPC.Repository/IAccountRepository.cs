using System;
using System.Linq;
using System.Linq.Expressions;
using Intime.OPC.Domain.Models;
using System.Collections.Generic;

namespace Intime.OPC.Repository
{
    public interface IAccountRepository : IRepository<OPC_AuthUser>
    {
        OPC_AuthUser Get(string userName, string password);
        //bool Create(OPC_AuthUser user);
        //bool Update(OPC_AuthUser user);
        //bool Delete(int userId);
       IEnumerable<OPC_AuthUser> All();
        bool SetEnable(int userId, bool enable);
    }
}