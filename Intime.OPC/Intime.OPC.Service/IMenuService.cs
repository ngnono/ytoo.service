using Intime.OPC.Domain.Models;
using System.Collections.Generic;

namespace Intime.OPC.Service
{
    public interface IMenuService
    {
        IEnumerable<OPC_AuthMenu> Select();
      
    }
}