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
        private readonly ISaleRemarkRepository _saleRemarkRepository;
        public SaleService(ISaleRepository saleRepository,ISaleRemarkRepository saleRemarkRepository)
        {
            _saleRepository = saleRepository;
            _saleRemarkRepository = saleRemarkRepository;
        }

        public bool UpdateSatus(OPC_Sale sale)
        {
            return _saleRepository.UpdateSatus(sale);
        }
        public System.Collections.Generic.IList<OPC_Sale> Select()
        {
            return _saleRepository.Select();
        }



        public IList<OPC_SaleComment> GetRemarksBySaleNo(string saleNo)
        {
           return  _saleRemarkRepository.GetBySaleOrderNo(saleNo);
        }
    }
}
