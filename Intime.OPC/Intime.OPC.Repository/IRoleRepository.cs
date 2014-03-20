using Intime.OPC.Domain.Models;
using System.Collections.Generic;

namespace Intime.OPC.Repository
{
    public interface IRoleRepository:IRespository<OPC_AuthRole>
    {

        bool SetEnable(int roleID, bool enable);
    }
}