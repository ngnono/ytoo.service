using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intime.OPC.Domain;
using Intime.OPC.Domain.Dto;
using Intime.OPC.Domain.Enums;
using Intime.OPC.Domain.Exception;
using Intime.OPC.Domain.Models;
using Intime.OPC.Repository;
using Intime.OPC.Service.Map;

namespace Intime.OPC.Service.Support
{
    public class ShippingSaleService :BaseService<OPC_ShippingSale>, IShippingSaleService
    {
        private readonly IShippingSaleRepository _shippingSaleRepository;
        private readonly IOrderRepository _orderRepository;
        private ISaleRMARepository _saleRmaRepository;
        public ShippingSaleService(IShippingSaleRepository repository, IOrderRepository orderRepository, ISaleRMARepository saleRmaRepository):base(repository)
        {
            _shippingSaleRepository = repository;
            _orderRepository = orderRepository;
            _saleRmaRepository = saleRmaRepository;
        }

        public PageResult<OPC_ShippingSale> GetByShippingCode(string shippingCode,int pageIndex,int pageSize=20)
        {
            return _shippingSaleRepository.GetByShippingCode(shippingCode,pageIndex,pageSize);
        }

        public void Shipped(string saleOrderNo,int userID)
        {
           var lst=  _shippingSaleRepository.GetBySaleOrderNo(saleOrderNo);

           if (lst == null)
           {
               throw new ShippingSaleNotExistsException(saleOrderNo);
           }
           lst.ShippingStatus = EnumSaleOrderStatus.Shipped.AsID();
           lst.UpdateDate = DateTime.Now;
           lst.UpdateUser = userID;

           _shippingSaleRepository.Update(lst);
           
            
        }

        public void PrintExpress(string orderNo, int userId)
        {
            //todo  确定是销售单还是订单
            var lst = _shippingSaleRepository.GetBySaleOrderNo(orderNo);
            if (lst==null)
            {
                throw new ShippingSaleNotExistsException(orderNo);
            }
                lst.ShippingStatus = EnumSaleOrderStatus.PrintExpress.AsID();
                lst.UpdateDate = DateTime.Now;
                lst.UpdateUser = userId;

                _shippingSaleRepository.Update(lst);
            
        }

        public void CreateRmaShipping(string rmaNo,int userId)
        {
            var dt = DateTime.Now;
            var saleRma = _saleRmaRepository.GetByRmaNo(rmaNo);
            var order = _orderRepository.GetOrderByOrderNo(saleRma.OrderNo);
       
            var sale = new OPC_ShippingSale();
            sale.RmaNo = rmaNo;
            sale.CreateDate = dt;
            sale.CreateUser = userId;
            sale.UpdateDate = dt;
            sale.UpdateUser = userId;
            sale.OrderNo = saleRma.OrderNo;

            sale.ShippingStatus = EnumSaleOrderStatus.PrintInvoice.AsID();
            sale.ShipViaName = "";
            sale.BrandId = order.BrandId;
            sale.ShippingAddress = order.ShippingAddress;
            sale.ShippingContactPerson = order.ShippingContactPerson;
            sale.ShippingContactPhone = order.ShippingContactPhone;
            sale.StoreId = order.StoreId;

            var bl = _shippingSaleRepository.Create(sale);
            
        }

        public void UpdateRmaShipping(RmaExpressSaveDto request)
        {
            var shipping =_shippingSaleRepository.GetByRmaNo(request.RmaNo);
            if (shipping==null)
            {
                throw new Exception(string.Format("快递单不存在,快递单号:{0}",request.ShippingCode));
            }

            shipping.ShipViaId = request.ShipViaID;
            shipping.ShipViaName = request.ShipViaName;
            shipping.ShippingFee =(decimal) (request.ShippingFee);
            shipping.ShippingCode = request.ShippingCode;
            _shippingSaleRepository.Update(shipping);

        }

        public void PintRmaShippingOver(string shippingCode)
        {
            var shipping = _shippingSaleRepository.GetByShippingCode(shippingCode, 1, 100).Result.FirstOrDefault();
            if (shipping == null)
            {
                throw new Exception(string.Format("快递单不存在,快递单号:{0}", shippingCode));
            }
           
            if (shipping.ShippingStatus == EnumRmaShippingStatus.NoPrint.AsID())
            {
                 shipping.ShippingStatus = EnumRmaShippingStatus.Printed.AsID();
                shipping.UpdateDate = DateTime.Now;
                shipping.UpdateUser = UserId;
                _shippingSaleRepository.Update(shipping);
            }
           
        }

        public void PintRmaShipping(string shippingCode)
        {
            //var shipping = _shippingSaleRepository.GetByShippingCode(shippingCode,1,100).Result.FirstOrDefault();
            //if (shipping == null)
            //{
            //    throw new Exception(string.Format("快递单不存在,快递单号:{0}", shippingCode));
            //}
            //if (shipping.ShippingStatus == EnumRmaShippingStatus.NoPrint.AsID() || shipping.ShippingStatus == EnumRmaShippingStatus.Printed.AsID() 
            //    )
            //{
            //    shipping.ShippingStatus = EnumRmaShippingStatus.Printed.AsID();
            //    shipping.UpdateDate = DateTime.Now;
            //    shipping.UpdateUser = UserId;
            //    _shippingSaleRepository.Update(shipping);
            //}

            
        }

        public PageResult<ShippingSaleDto> GetRmaByPackPrintPress(RmaExpressRequest request)
        {
          var lst=   _shippingSaleRepository.GetByOrderNo(request.OrderNo, request.StartDate, request.EndDate, request.pageIndex,
                request.pageSize,EnumRmaShippingStatus.NoPrint.AsID());

            IList<ShippingSaleDto> lstDtos=new List<ShippingSaleDto>();
            foreach (var shippingSale in lst.Result)
            {
                var o = AutoMapper.Mapper.Map<OPC_ShippingSale, ShippingSaleDto>(shippingSale);
                EnumRmaShippingStatus rmaShippingStatus = (EnumRmaShippingStatus) shippingSale.ShippingStatus;
                o.PrintStatus = rmaShippingStatus.GetDescription();
                lstDtos.Add(o);
            }
         return new PageResult<ShippingSaleDto>(lstDtos,lst.TotalCount);


        }

        public PageResult<ShippingSaleDto> GetRmaShippingPrintedByPack(RmaExpressRequest request)
        {
            var lst = _shippingSaleRepository.GetByOrderNo(request.OrderNo, request.StartDate, request.EndDate, request.pageIndex,
                 request.pageSize, EnumRmaShippingStatus.Printed.AsID());

            IList<ShippingSaleDto> lstDtos = new List<ShippingSaleDto>();
            foreach (var shippingSale in lst.Result)
            {
                var o = AutoMapper.Mapper.Map<OPC_ShippingSale, ShippingSaleDto>(shippingSale);
                EnumRmaShippingStatus rmaShippingStatus = (EnumRmaShippingStatus)shippingSale.ShippingStatus;
                o.PrintStatus = rmaShippingStatus.GetDescription();
                lstDtos.Add(o);
            }
            return new PageResult<ShippingSaleDto>(lstDtos, lst.TotalCount);
        }

        public void PintRmaShippingOverConnect(string shippingCode)
        {
            var shipping = _shippingSaleRepository.GetByShippingCode(shippingCode, 1, 100).Result.FirstOrDefault();
            if (shipping == null)
            {
                throw new Exception(string.Format("快递单不存在,快递单号:{0}", shippingCode));
            }
            if (shipping.ShippingStatus ==EnumRmaShippingStatus.Printed.AsID())
            {
                shipping.ShippingStatus = EnumRmaShippingStatus.PrintOver.AsID();
                shipping.UpdateDate = DateTime.Now;
                shipping.UpdateUser = UserId;
                _shippingSaleRepository.Update(shipping);
            }
        }
    }
}
