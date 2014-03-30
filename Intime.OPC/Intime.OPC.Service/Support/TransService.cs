using System.Collections.Generic;
using Intime.OPC.Domain.Dto;
using Intime.OPC.Domain.Models;
using Intime.OPC.Repository;
using Intime.OPC.Service.Map;

namespace Intime.OPC.Service.Support
{
    public class TransService : BaseService, ITransService
    {
        private readonly IOrderRemarkRepository _orderRemarkRepository;
        private readonly ISaleRepository _saleRepository;
        private readonly ITransRepository _transRepository;
        private readonly IShippingSaleCommentRepository _shippingSaleCommentRepository;

        public TransService(ITransRepository transRepository, IOrderRemarkRepository orderRemarkRepository,ISaleRepository saleRepository,IShippingSaleCommentRepository shippingSaleCommentRepository)
        {
            _transRepository = transRepository;
            _orderRemarkRepository = orderRemarkRepository;
            _saleRepository = saleRepository;
            _shippingSaleCommentRepository = shippingSaleCommentRepository;
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

        public ShippingSaleDto GetShippingSaleBySaleNo(string saleNo)
        {
            var entity=  _saleRepository.GetBySaleNo(saleNo);
            return Mapper.Map<OPC_Sale, ShippingSaleDto>(entity);
        }


        public bool AddShippingSaleComment(OPC_ShippingSaleComment comment)
        {
            return   _shippingSaleCommentRepository.Create(comment);
        }

        public IList<OPC_ShippingSaleComment> GetByShippingCommentCode(string shippingCode)
        {
            return _shippingSaleCommentRepository.GetByShippingCode(shippingCode);
        }
    }
}