using Intime.OPC.Domain.Models;
using System.Collections.Generic;

namespace Intime.OPC.Repository
{
    public interface IRoleService
    {
        bool Create(OPC_AuthRole role);
        bool Update(OPC_AuthRole role);
        bool Delete(int roleId);
        IList<OPC_AuthRole> Select();
        bool IsStop(int roleId, bool bValid);
    }
}