using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Intime.OPC.Domain;
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
        private readonly IAccountService  _accountService;

        public TransService(ITransRepository transRepository, 
            IOrderRemarkRepository orderRemarkRepository,
            ISaleRepository saleRepository,
            IShippingSaleRepository shippingSaleRepository,
            IAccountService accountService,
            IShippingSaleCommentRepository shippingSaleCommentRepository)
        {
            _transRepository = transRepository;
            _orderRemarkRepository = orderRemarkRepository;
            _shippingSaleRepository = shippingSaleRepository;
            _saleRepository = saleRepository;
            _shippingSaleCommentRepository = shippingSaleCommentRepository;
            _accountService = accountService;
        }

        #region ITransService Members

        public PageResult<OPC_Sale> Select(DateTime startDate, DateTime endDate, string orderNo, string saleOrderNo,int pageIndex,int pageSize=20)
        {
            return _transRepository.Select(startDate, endDate, orderNo, saleOrderNo, pageIndex, pageSize);
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
            var entity = _shippingSaleRepository.GetBySaleOrderNo(saleNo,0,100).Result.FirstOrDefault();
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
                var lst = _shippingSaleRepository.GetBySaleOrderNo(saleID,1,1);

                if (lst.TotalCount > 0)
                {
                    throw new ShippingSaleExistsException(shippingSaleDto.ShippingCode);
                }
                //验证发货单号 是否重复
                var e=  _shippingSaleRepository.GetByShippingCode(shippingSaleDto.ShippingCode,1,1);
                if (e.TotalCount>0)
                {
                    throw new ShippingSaleExistsException(shippingSaleDto.ShippingCode);
                }


                var bl = _shippingSaleRepository.Create(sale);
               // _saleRepository.UpdateSatus(saleID, EnumSaleOrderStatus.PrintExpress, userId);
            }

            return true;
        }

        public PageResult<ShippingSaleDto> GetShippingSale(string shippingCode, DateTime startTime, DateTime endTime, int pageIndex, int pageSize = 20)
        {
            startTime = startTime.Date;
            endTime = endTime.Date.AddDays(1);

            var lst=  _shippingSaleRepository.Get(shippingCode, startTime, endTime,EnumSaleOrderStatus.PrintExpress.AsID(),pageIndex,pageSize);
            return Mapper.Map<OPC_ShippingSale, ShippingSaleDto>(lst);
        }

        public PageResult<SaleDto> GetSaleByShippingSaleNo(string shippingSaleNo, int pageIndex, int pageSize = 20)
        {
            var lst = _shippingSaleRepository.GetByShippingCode(shippingSaleNo,1,1);
            if (lst.TotalCount>0)
            {
                throw new ShippingSaleNotExistsException(shippingSaleNo);
            }
            IList<OPC_Sale> lstSales= lst.Result.Select(opcShippingSale => _saleRepository.GetBySaleNo(opcShippingSale.SaleOrderNo)).ToList();
            

            var lst2= Mapper.Map<OPC_Sale, SaleDto>(lstSales);
            return new PageResult<SaleDto>(lst2,lst.TotalCount);
        }

        public PageResult<SaleDto> GetSaleOrderPickup(string orderNo, string saleOrderNo, DateTime startDate, DateTime endDate,int userid,int pageIndex,int pageSize)
        {
            startDate = startDate.Date;
            endDate = endDate.Date.AddDays(1);
             var user = _accountService.GetByUserID(userid);
            if (user.SectionIDs.Count == 0)
            {
                throw new UnauthorizedAccessException();
            }

            var lst = _saleRepository.GetPickUped(saleOrderNo, orderNo, startDate,endDate,pageIndex,pageSize,user.SectionIDs.ToArray());
            return Mapper.Map<OPC_Sale, SaleDto>(lst);
        }

       
        public IList<ShippingSaleDto> GetShippingSale(string shippingCode, DateTime startTime, DateTime endTime)
        {
            throw new NotImplementedException();
        }


        public IList<SaleDto> GetSaleByShippingSaleNo(string shippingSaleNo)
        {
            throw new NotImplementedException();
        }
    }
}