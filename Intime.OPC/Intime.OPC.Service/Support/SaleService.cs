using Intime.OPC.Domain.Models;
using Intime.OPC.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intime.OPC.Service.Support
{
    public class SaleService : ISaleService
    {
        private readonly ISaleRepository _saleRepository;
        public SaleService(ISaleRepository saleRepository)
        {
            _saleRepository = saleRepository;
        }

        public bool UpdateSatus(OPC_Sale sale)
        {
            return _saleRepository.UpdateSatus(sale);
        }
        public System.Collections.Generic.IList<OPC_Sale> Select()
        {
            return _saleRepository.Select();
        }

    }
}
