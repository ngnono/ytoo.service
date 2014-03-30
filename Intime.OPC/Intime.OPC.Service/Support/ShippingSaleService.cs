using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intime.OPC.Domain.Models;
using Intime.OPC.Repository;

namespace Intime.OPC.Service.Support
{
    public class ShippingSaleService :BaseService, IShippingSaleService
    {
        private readonly IShippingSaleRepository _shippingSaleRepository;
        public ShippingSaleService(IShippingSaleRepository repository)
        {
            _shippingSaleRepository = repository;
        }

        public IList<OPC_ShippingSale> GetByShippingCode(string shippingCode)
        {
            return _shippingSaleRepository.GetByShippingCode(shippingCode);
        }
    }
}
