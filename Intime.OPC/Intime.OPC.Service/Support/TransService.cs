using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Intime.OPC.Domain.Dto;
using Intime.OPC.Domain.Enums;
using Intime.OPC.Domain.Exception;
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
        private readonly IShippingSaleRepository _shippingSaleRepository;

        public TransService(ITransRepository transRepository, 
            IOrderRemarkRepository orderRemarkRepository,
            ISaleRepository saleRepository,
            IShippingSaleRepository shippingSaleRepository,
            IShippingSaleCommentRepository shippingSaleCommentRepository)
        {
            _transRepository = transRepository;
            _orderRemarkRepository = orderRemarkRepository;
            _shippingSaleRepository = shippingSaleRepository;
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
            var entity = _shippingSaleRepository.GetBySaleOrderNo(saleNo).FirstOrDefault();
            if (entity==null)
            {
                throw new ShippingSaleNotExistsException(saleNo);
            }
            return Mapper.Map<OPC_ShippingSale, ShippingSaleDto>(entity);
        }


        public bool AddShippingSaleComment(OPC_ShippingSaleComment comment)
        {
            return   _shippingSaleCommentRepository.Create(comment);
        }

        public IList<OPC_ShippingSaleComment> GetByShippingCommentCode(string shippingCode)
        {
            return _shippingSaleCommentRepository.GetByShippingCode(shippingCode);
        }

        public bool CreateShippingSale(int userId, ShippingSaleCreateDto shippingSaleDto)
        {
            var dt = DateTime.Now;
            foreach (var saleID in shippingSaleDto.SaleOrderIDs)
            {
                var sale = new OPC_ShippingSale();
                sale.CreateDate = dt;
                sale.CreateUser = userId;
                sale.UpdateDate = dt;
                sale.UpdateUser = userId;
                sale.SaleOrderNo = saleID;
                sale.ShipViaId = shippingSaleDto.ShipViaID;
                sale.ShippingCode = shippingSaleDto.ShippingCode;
                sale.ShippingFee = (decimal) (shippingSaleDto.ShippingFee);
                sale.ShippingStatus = EnumSaleOrderStatus.PrintInvoice.AsID();
                

                //验证是否已经生成过发货单
                var lst = _shippingSaleRepository.GetBySaleOrderNo(saleID);

                if (lst.Count > 0)
                {
                    throw new ShippingSaleExistsException(shippingSaleDto.ShippingCode);
                }
                //验证发货单号 是否重复
                var e=  _shippingSaleRepository.GetByShippingCode(shippingSaleDto.ShippingCode);
                if (e.Count>0)
                {
                    throw new ShippingSaleExistsException(shippingSaleDto.ShippingCode);
                }


                var bl = _shippingSaleRepository.Create(sale);
               // _saleRepository.UpdateSatus(saleID, EnumSaleOrderStatus.PrintExpress, userId);
            }

            return true;
        }

        public IList<ShippingSaleDto> GetShippingSale(string shippingCode, DateTime startTime, DateTime endTime)
        {
            var lst=  _shippingSaleRepository.Get(shippingCode, startTime.Date, endTime.AddDays(1),EnumSaleOrderStatus.PrintExpress.AsID());
            return Mapper.Map<OPC_ShippingSale, ShippingSaleDto>(lst);
        }

        public IList<SaleDto> GetSaleByShippingSaleNo(string shippingSaleNo)
        {
            var lst = _shippingSaleRepository.GetByShippingCode(shippingSaleNo);
            if (lst==null || lst.Count==0)
            {
                throw new ShippingSaleNotExistsException(shippingSaleNo);
            }
            IList<OPC_Sale> lstSales= lst.Select(opcShippingSale => _saleRepository.GetBySaleNo(opcShippingSale.SaleOrderNo)).ToList();

            return Mapper.Map<OPC_Sale, SaleDto>(lstSales);
        }

        public IList<SaleDto> GetSaleOrderPickup(string orderNo, string saleOrderNo, DateTime startDate, DateTime endDate)
        {
            var lst = _saleRepository.GetPickUped(saleOrderNo, orderNo, startDate.Date,endDate.Date.AddDays(1));
            return Mapper.Map<OPC_Sale, SaleDto>(lst);
        }
    }
}