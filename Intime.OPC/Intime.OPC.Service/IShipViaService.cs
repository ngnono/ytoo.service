using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intime.OPC.Domain.Models;

namespace Intime.OPC.Service
{
    public interface IShipViaService
    {
        IList<ShipVia> GetAll();
    }
}
