using System;
using System.Linq;
using System.Linq.Expressions;
using Intime.OPC.Domain.Models;
using System.Collections.Generic;

namespace Intime.OPC.Repository
{
    public interface IMenuRepository : IRepository<OPC_AuthMenu>
    {
        IEnumerable<OPC_AuthMenu> GetMenusByUserID(int userID);

        IEnumerable<OPC_AuthMenu> GetMenusByRoleID(int roleID);
    }
}