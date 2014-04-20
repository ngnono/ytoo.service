using System;
using System.Collections.Generic;
using System.Linq;
using Intime.OPC.Domain;
using Intime.OPC.Domain.Dto;
using Intime.OPC.Domain.Enums;
using Intime.OPC.Domain.Exception;
using Intime.OPC.Domain.Models;
using Intime.OPC.Repository;
using Intime.OPC.Service.Map;

namespace Intime.OPC.Service.Support
{
    public class SaleService : BaseService<OPC_Sale>, ISaleService
    {
        private readonly ISaleRemarkRepository _saleRemarkRepository;
        private readonly ISaleRepository _saleRepository;
        private readonly IAccountService _accountService;

        public SaleService(ISaleRepository saleRepository, ISaleRemarkRepository saleRemarkRepository,IAccountService accountService):base(saleRepository)
        {
            _saleRepository = saleRepository;
            _saleRemarkRepository = saleRemarkRepository;
            _accountService = accountService;
        }

        #region ISaleService Members

        public IList<OPC_Sale> Select()
        {
            return _saleRepository.Select();
        }

        public IList<OPC_SaleComment> GetRemarksBySaleNo(string saleNo)
        {
            return _saleRemarkRepository.GetBySaleOrderNo(saleNo);
        }

        public bool PrintSale(string saleNo, int userId)
        {
            OPC_Sale saleOrder = _saleRepository.GetBySaleNo(saleNo);
            if (saleOrder == null)
            {
                throw new SaleOrderNotExistsException(saleNo);
            }

            if (saleOrder.Status > (int) EnumSaleOrderStatus.PrintSale)
            {
                return true;
            }

            saleOrder.PrintTimes += 1;
            saleOrder.UpdatedDate = DateTime.Now;
            saleOrder.UpdatedUser = userId;
            _saleRepository.Update(saleOrder);
            return true;
        }

        public PageResult<SaleDetailDto> GetSaleOrderDetails(string saleOrderNo, int userId,int pageIndex,int pageSize)
        {
            if (string.IsNullOrEmpty(saleOrderNo))
            {
                throw new ArgumentNullException("saleOrderNo");
            }
            OPC_Sale saleOrder = _saleRepository.GetBySaleNo(saleOrderNo);
            if (saleOrder == null)
            {
                throw new SaleOrderNotExistsException(saleOrderNo);
            }

            return _saleRepository.GetSaleOrderDetails(saleOrderNo,pageIndex,pageSize);
            
        }

        public bool SetSaleOrderPickUp(string saleOrderNo, int userId)
        {
            return UpdateSatus(saleOrderNo, userId, EnumSaleOrderStatus.PickUp);
        }

        public bool SetShipInStorage(string saleOrderNo, int userId)
        {
            return UpdateSatus(saleOrderNo, userId, EnumSaleOrderStatus.ShipInStorage);
        }

        public bool PrintInvoice(string orderNo, int userId)
        {
            return UpdateSatus(orderNo, userId, EnumSaleOrderStatus.PrintInvoice);
        }

        public bool FinishPrintSale(string orderNo, int userId)
        {
            return UpdateSatus(orderNo, userId, EnumSaleOrderStatus.PrintSale);
        }

        public bool PrintExpress(string orderNo, int userId)
        {
            return UpdateSatus(orderNo, userId, EnumSaleOrderStatus.PrintExpress);
        }

        public bool Shipped(string orderNo, int userId)
        {
            return UpdateSatus(orderNo, userId, EnumSaleOrderStatus.Shipped);
        }

        public bool StockOut(string orderNo, int userId)
        {
            return UpdateSatus(orderNo, userId, EnumSaleOrderStatus.StockOut);
        }

        public PageResult<SaleDto> GetPickUp(string saleOrderNo, string orderNo, DateTime dtStart, DateTime dtEnd,int userID, int pageIndex, int pageSize)
        {
            dtStart = dtStart.Date;
            dtEnd = dtEnd.Date.AddDays(1);
           // var user = _accountService.GetByUserID(userID);
            //if (user.SectionIDs.Count == 0)
            //{
            //    throw new UnauthorizedAccessException();
            //}

            var lst = _saleRepository.GetPickUped(saleOrderNo, orderNo, dtStart, dtEnd, pageIndex, pageSize,null);
            return Mapper.Map<OPC_Sale, SaleDto>(lst);
        }


        public PageResult<SaleDto> GetPrintSale(string saleId, int userId, string orderNo, DateTime dtStart, DateTime dtEnd, int pageIndex, int pageSize)
        {
            dtStart = dtStart.Date;
            dtEnd = dtEnd.Date.AddDays(1);
            //var user = _accountService.GetByUserID(userId);
            //if (user.SectionIDs.Count == 0)
            //{
            //    throw new UnauthorizedAccessException();
            //}
            var lst = _saleRepository.GetPrintSale(saleId, orderNo, dtStart, dtEnd, pageIndex, pageSize,null);
            return Mapper.Map<OPC_Sale, SaleDto>(lst);
        }

        public PageResult<SaleDto> GetShipped(string saleOrderNo, int userId, string orderNo, DateTime dtStart, DateTime dtEnd, int pageIndex, int pageSize)
        {
            dtStart = dtStart.Date;
            dtEnd = dtEnd.Date.AddDays(1);
            
            var lst = _saleRepository.GetShipped(saleOrderNo, orderNo, dtStart, dtEnd, pageIndex, pageSize, null);
            return Mapper.Map<OPC_Sale, SaleDto>(lst);
        }

        public PageResult<SaleDto> GetPrintExpress(string saleOrderNo, int userId, string orderNo, DateTime dtStart,
            DateTime dtEnd, int pageIndex, int pageSize)
        {
            dtStart = dtStart.Date;
            dtEnd = dtEnd.Date.AddDays(1);
            
            var lst = _saleRepository.GetPrintExpress(saleOrderNo, orderNo, dtStart, dtEnd, pageIndex, pageSize, null);
            return Mapper.Map<OPC_Sale, SaleDto>(lst);
        }

        public PageResult<SaleDto> GetPrintInvoice(string saleOrderNo, int userId, string orderNo, DateTime dtStart,
            DateTime dtEnd, int pageIndex, int pageSize)
        {
            dtStart = dtStart.Date;
            dtEnd = dtEnd.Date.AddDays(1);
            
            var lst = _saleRepository.GetPrintInvoice(saleOrderNo, orderNo, dtStart, dtEnd, pageIndex, pageSize, null);
            return Mapper.Map<OPC_Sale, SaleDto>(lst);
        }

        public PageResult<SaleDto> GetShipInStorage(string saleOrderNo, int userId, string orderNo, DateTime dtStart,
            DateTime dtEnd, int pageIndex, int pageSize)
        {
            dtStart = dtStart.Date;
            dtEnd = dtEnd.Date.AddDays(1);
            
            var lst = _saleRepository.GetShipInStorage(saleOrderNo, orderNo, dtStart, dtEnd, pageIndex, pageSize, null);
            return Mapper.Map<OPC_Sale, SaleDto>(lst);
        }

        public bool WriteSaleRemark(OPC_SaleComment comment)
        {
            return _saleRemarkRepository.Create(comment);
        }

        public PageResult<SaleDto> GetByOrderNo(string orderID, int userid, int pageIndex, int pageSize)
        {
            if (string.IsNullOrWhiteSpace(orderID))
            {
                throw new OrderNoIsNullException(orderID);
            }
           
            //IList<SaleDto> lstDtos=new List<SaleDto>();
            //foreach (var sectionID in user.SectionIDs)
            //{
            //    var lst = _saleRepository.GetByOrderNo(orderID,sectionID);
            //    if (lst.Count>0)
            //    {
            //        lstDtos.AddRange(Mapper.Map<OPC_Sale, SaleDto>(lst));  
            //    }
            //}

            var lst = _saleRepository.GetByOrderNo(orderID, -1);
            var lstDtos = Mapper.Map<OPC_Sale, SaleDto>(lst);
            var lst2= lstDtos.Page(pageIndex, pageSize);
            return new PageResult<SaleDto>(lst2.ToList(),lstDtos.Count);
        }

        public IList<SaleDto> GetByShippingCode(string shippingCode)
        {
            var sale= _saleRepository.GetByShippingCode(shippingCode);
            return Mapper.Map<OPC_Sale, SaleDto>(sale);
        }

        public PageResult<SaleDto> GetNoPickUp(string saleId, int userId, string orderNo, DateTime dtStart, DateTime dtEnd, int pageIndex, int pageSize)
        {
            dtStart = dtStart.Date;
            dtEnd = dtEnd.Date.AddDays(1);
           
            var lst = _saleRepository.GetNoPickUp(saleId, orderNo, dtStart, dtEnd, pageIndex, pageSize,null);
            return Mapper.Map<OPC_Sale, SaleDto>(lst);
        }

        #endregion

        private bool UpdateSatus(string saleNo, int userId, EnumSaleOrderStatus saleOrderStatus)
        {
            OPC_Sale saleOrder = _saleRepository.GetBySaleNo(saleNo);
            if (saleOrder == null)
            {
                throw new SaleOrderNotExistsException(saleNo);
            }

            if (saleOrder.Status >= (int) saleOrderStatus)
            {
                return true;
            }
            saleOrder.Status = (int) saleOrderStatus;
            saleOrder.ShippingStatus = saleOrder.Status;
            saleOrder.UpdatedDate = DateTime.Now;
            saleOrder.UpdatedUser = userId;
            _saleRepository.Update(saleOrder);
            return true;
        }
    }
}