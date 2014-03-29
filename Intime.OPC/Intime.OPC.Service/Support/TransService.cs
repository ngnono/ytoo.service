using System.Collections.Generic;
using Intime.OPC.Domain.Models;
using Intime.OPC.Repository;

namespace Intime.OPC.Service.Support
{
    public class TransService : ITransService
    {
        private readonly IOrderRemarkRepository _orderRemarkRepository;
        private readonly ITransRepository _transRepository;

        public TransService(ITransRepository transRepository, IOrderRemarkRepository orderRemarkRepository)
        {
            _transRepository = transRepository;
            _orderRemarkRepository = orderRemarkRepository;
        }

        #region ITransService Members

        public IList<OPC_Sale> Select(string startDate, string endDate, string orderNo, string saleOrderNo)
        {
            return _transRepository.Select(startDate, endDate, orderNo, saleOrderNo);
        }

        public bool Finish(Dictionary<string, string> sale)
        {
            return _transRepository.Finish(sale);
        }

        public IList<OPC_SaleDetail> SelectSaleDetail(IEnumerable<string> saleNos)
        {
            return _transRepository.SelectSaleDetail(saleNos);
        }

        public IList<OPC_OrderComment> GetRemarksByOrderNo(string orderNo)
        {
            return _orderRemarkRepository.GetByOrderNo(orderNo);
        }

        #endregion


        public bool AddOrderComment(OPC_OrderComment comment)
        {
           
           return   _orderRemarkRepository.Create(comment);
        }
    }
}