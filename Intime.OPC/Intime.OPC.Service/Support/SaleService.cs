using System;
using System.Collections.Generic;
using Intime.OPC.Domain.Dto;
using Intime.OPC.Domain.Enums;
using Intime.OPC.Domain.Exception;
using Intime.OPC.Domain.Models;
using Intime.OPC.Repository;
using Intime.OPC.Service.Map;

namespace Intime.OPC.Service.Support
{
    public class SaleService : ISaleService
    {
        private readonly ISaleRemarkRepository _saleRemarkRepository;
        private readonly ISaleRepository _saleRepository;

        public SaleService(ISaleRepository saleRepository, ISaleRemarkRepository saleRemarkRepository)
        {
            _saleRepository = saleRepository;
            _saleRemarkRepository = saleRemarkRepository;
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

        public IList<OPC_SaleDetail> GetSaleOrderDetails(string saleOrderNo, int userId)
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

            //todo 权限校验

            return _saleRepository.GetSaleOrderDetails(saleOrderNo);
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

        public IList<SaleDto> GetPrintSale(string saleId, int userId, string orderNo, DateTime dtStart, DateTime dtEnd)
        {

            var lst = _saleRepository.GetPrintSale(saleId, orderNo, dtStart, dtEnd);
            return  Mapper.Map<OPC_Sale, SaleDto>(lst);
         
        }

        public IList<SaleDto> GetPrintExpress(string saleOrderNo, int userId, string orderNo, DateTime dtStart,
            DateTime dtEnd)
        {
         
            var lst = _saleRepository.GetPrintExpress(saleOrderNo, orderNo, dtStart, dtEnd);
            return Mapper.Map<OPC_Sale, SaleDto>(lst);
        }

        public IList<SaleDto> GetPrintInvoice(string saleOrderNo, int userId, string orderNo, DateTime dtStart,
            DateTime dtEnd)
        {
            var lst = _saleRepository.GetPrintInvoice(saleOrderNo, orderNo, dtStart, dtEnd);
            return Mapper.Map<OPC_Sale, SaleDto>(lst);
        }

        public IList<SaleDto> GetShipInStorage(string saleOrderNo, int userId, string orderNo, DateTime dtStart,
            DateTime dtEnd)
        {
            var lst = _saleRepository.GetShipInStorage(saleOrderNo, orderNo, dtStart, dtEnd);
            return Mapper.Map<OPC_Sale, SaleDto>(lst);
        }

        public bool WriteSaleRemark(OPC_SaleComment comment)
        {
            return _saleRemarkRepository.Create(comment);
        }

        public IList<SaleDto> GetByOrderNo(string orderID)
        {
            if (string.IsNullOrWhiteSpace(orderID))
            {
                throw new OrderNoIsNullException();
            }
      
            var lst = _saleRepository.GetByOrderNo(orderID);
            return Mapper.Map<OPC_Sale, SaleDto>(lst);
        }

        public IList<SaleDto> GetNoPickUp(string saleId, int userId, string orderNo, DateTime dtStart, DateTime dtEnd)
        {
            //todo 权限校验

            IList<OPC_Sale> lst = _saleRepository.GetNoPickUp(saleId, orderNo, dtStart, dtEnd);
            var lstDto = new List<SaleDto>();
            foreach (OPC_Sale opcSale in lst)
            {
                SaleDto t = Mapper.Map<OPC_Sale, SaleDto>(opcSale);
                var saleOrderStatus = (EnumSaleOrderStatus) opcSale.Status;
                t.StatusName = saleOrderStatus.GetDescription();
                lstDto.Add(t);
            }
            return lstDto;
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
            saleOrder.UpdatedDate = DateTime.Now;
            saleOrder.UpdatedUser = userId;
            _saleRepository.Update(saleOrder);
            return true;
        }
    }
}