using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intime.OPC.Repository;

namespace Intime.OPC.Service.Support
{
    public class ShipViaService : BaseService, IShipViaService
    {
        private IShipViaRepository _shipViaRepository;
        public ShipViaService(IShipViaRepository shipViaRepository)
        {
            _shipViaRepository = shipViaRepository;
        }

        public IList<Domain.Models.ShipVia> GetAll()
        {
            return _shipViaRepository.GetAll();
        }
    }
}
