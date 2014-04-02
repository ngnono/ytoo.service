using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intime.OPC.Domain;
using Intime.OPC.Domain.Models;
using Intime.OPC.Repository;

namespace Intime.OPC.Service.Support
{
    public class ShipViaService : BaseService<ShipVia>, IShipViaService
    {
        public ShipViaService(IShipViaRepository shipViaRepository):base(shipViaRepository)
        {
        }

        public PageResult<ShipVia> GetAll( int pageIndex, int pageSize = 20)
        {
            return _repository.GetAll(pageIndex,pageSize);
        }
    }
}
