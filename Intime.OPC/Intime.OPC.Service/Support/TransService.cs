using System;
using System.Collections.Generic;
using System.Linq;
using Intime.OPC.Domain.Models;
using Intime.OPC.Repository;

namespace Intime.OPC.Service.Support
{
    public class TransService : ITransService
    {
        private readonly ITransRepository _transRepository;

        public TransService(ITransRepository transRepository)
        {
            _transRepository = transRepository;
        }

        public System.Collections.Generic.IList<OPC_Sale> Select(string startDate, string endDate, string orderNo, string saleOrderNo)
        {
            return _transRepository.Select( startDate,  endDate,  orderNo, saleOrderNo);
        }

        public bool Finish(Dictionary<string, string> sale)
        {
            return _transRepository.Finish(sale);
        }


        public IList<OPC_SaleDetail> SelectSaleDetail(string saleIDs)
        {
            return _transRepository.SelectSaleDetail(saleIDs.ToInts(','));
        }
    }
}