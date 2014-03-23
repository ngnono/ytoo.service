using Intime.OPC.Domain.Models;
using System.Collections.Generic;

namespace Intime.OPC.Repository
{
    public interface IRoleRepository:IRepository<OPC_AuthRole>
    {
        IList<OPC_AuthRole> All();
        IList<OPC_AuthRole> GetByUserID(int userID);
        bool SetEnable(int roleID, bool enable);
    }
}