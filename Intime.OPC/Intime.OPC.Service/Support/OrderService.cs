using System;
using System.Collections.Generic;
using System.Linq;
using Intime.OPC.Domain;
using Intime.OPC.Domain.Dto;
using Intime.OPC.Domain.Dto.Custom;
using Intime.OPC.Domain.Dto.Financial;
using Intime.OPC.Domain.Enums;
using Intime.OPC.Domain.Exception;
using Intime.OPC.Domain.Models;
using Intime.OPC.Repository;
using Intime.OPC.Service.Map;

namespace Intime.OPC.Service.Support
{
    public class OrderService : BaseService<Order>, IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderRemarkRepository _orderRemarkRepository;
        private readonly IOrderItemRepository _orderItemRepository;
        private readonly IBrandRepository _brandRepository;
        private IAccountService _accountService;
        private ISaleDetailRepository _saleDetailRepository;
        private ISaleRMARepository _saleRmaRepository;

        public OrderService(IOrderRepository orderRepository, IOrderRemarkRepository orderRemarkRepository, IOrderItemRepository orderItemRepository, IBrandRepository brandRepository, IAccountService accountService, ISaleDetailRepository saleDetailRepository, ISaleRMARepository saleRmaRepository)
            : base(orderRepository)
        {
            _orderRepository = _repository as IOrderRepository;
            _orderRemarkRepository = orderRemarkRepository;
            _orderItemRepository = orderItemRepository;
            _brandRepository = brandRepository;
            _accountService = accountService;
            _saleDetailRepository = saleDetailRepository;
            _saleRmaRepository = saleRmaRepository;
        }

        public PageResult<OrderDto> GetOrder(string orderNo, string orderSource, DateTime dtStart, DateTime dtEnd,
            int storeId, int brandId, int status, string paymentType, string outGoodsType, string shippingContactPhone,
            string expressDeliveryCode, int expressDeliveryCompany, int userId, int pageIndex, int pageSize = 20)
        {
            dtStart = dtStart.Date;
            dtEnd = dtEnd.Date.AddDays(1);
            _orderRepository.SetCurrentUser(_accountService.GetByUserID(UserId));
            var pg=_orderRepository.GetOrder(orderNo, orderSource, dtStart, dtEnd, storeId, brandId,
                status, paymentType,
                outGoodsType, shippingContactPhone, expressDeliveryCode, expressDeliveryCompany,pageIndex,pageSize);

            var r= Mapper.Map<Order, OrderDto>(pg.Result);
            return new PageResult<OrderDto>(r, pg.TotalCount);
        }

        public OrderDto GetOrderByOrderNo(string orderNo)
        {
            var e = _orderRepository.GetOrderByOrderNo(orderNo);
            if (e==null)
            {
                throw new OrderNotExistsException(orderNo);
            }
            return  Mapper.Map<Order, OrderDto>(e);
        }

        #region 有关备注
        
       
        public bool AddOrderComment(OPC_OrderComment comment)
        {

            return _orderRemarkRepository.Create(comment);
        }

        public PageResult<OrderDto> GetOrderByOderNoTime(string orderNo, DateTime dtStart, DateTime dtEnd,int pageIndex,int pageSize)
        {
            dtStart = dtStart.Date;
            dtEnd = dtEnd.Date.AddDays(1);
            _orderRepository.SetCurrentUser(_accountService.GetByUserID(UserId));
            var lstOrder = _orderRepository.GetOrderByOderNoTime(orderNo, dtStart, dtEnd,pageIndex,pageSize);
            return Mapper.Map<Order, OrderDto>(lstOrder);
        }

        public PageResult<OrderItemDto> GetOrderItems(string orderNo,int pageIndex,int pageSize)
        {
            _orderItemRepository.SetCurrentUser(_accountService.GetByUserID(UserId));
            var lstOrderItems= _orderItemRepository.GetByOrderNo(orderNo,pageIndex,pageSize);

            return lstOrderItems;
        }

        public PageResult<OrderDto> GetOrderByShippingNo(string shippingNo, int pageIndex, int pageSize)
        {
            _orderRepository.SetCurrentUser(_accountService.GetByUserID(UserId));
            var lst= _orderRepository.GetOrderByShippingNo(shippingNo,pageIndex,pageSize);
            return Mapper.Map<Order, OrderDto>(lst);
        }

        public PageResult<OrderDto> GetByReturnGoodsInfo(ReturnGoodsInfoRequest request)
        {
            request.FormatDate();
            _orderRepository.SetCurrentUser(_accountService.GetByUserID(UserId));
            var lst = _orderRepository.GetByReturnGoodsInfo(request);
               return Mapper.Map<Order, OrderDto>(lst);

            //var lst = _orderRepository.GetOrder(request.OrderNo,"",request.StartDate,request.EndDate,request.StoreID.Value,
            //    -1,-1,request.PayType,"","","",-1,1,1000);
                   
            //return Mapper.Map<Order, OrderDto>(lst.Result);
        }

        public PageResult<OrderDto> GetShippingBackByReturnGoodsInfo(ReturnGoodsInfoRequest request)
        {
            request.FormatDate();

            string returnGoodsStatus = "";
            int status = EnumRMAStatus.ShipVerifyNotPass.AsID();
            _orderRepository.SetCurrentUser(_accountService.GetByUserID(UserId));
            var lst = _orderRepository.GetBySaleRma(request, status, returnGoodsStatus);
            return Mapper.Map<Order, OrderDto>(lst);
        }

        public PageResult<OrderDto> GetSaleRmaByReturnGoodsCompensate(ReturnGoodsInfoRequest request)
        {
            request.FormatDate();
            _orderRepository.SetCurrentUser(_accountService.GetByUserID(UserId));
            string returnGoodsStatus = EnumReturnGoodsStatus.CompensateVerifyFailed.GetDescription();
            var lst = _orderRepository.GetBySaleRma(request, null, returnGoodsStatus);
            return Mapper.Map<Order, OrderDto>(lst);
        }

        public PageResult<OrderDto> GetOrderByOutOfStockNotify(OutOfStockNotifyRequest request)
        {
            request.FormatDate();
            _orderRepository.SetCurrentUser(_accountService.GetByUserID(UserId));
            int orderstatus =EnumOderStatus.StockOut.AsID();
            var lst = _orderRepository.GetByOutOfStockNotify(request,orderstatus);
            return Mapper.Map<Order, OrderDto>(lst);
        }

        public PageResult<OrderDto> GetOrderOfVoid(OutOfStockNotifyRequest request)
        {
            request.FormatDate();
            _orderRepository.SetCurrentUser(_accountService.GetByUserID(UserId));
            int orderstatus = EnumOderStatus.Void.AsID();
            var lst = _orderRepository.GetByOutOfStockNotify(request, orderstatus);
            return Mapper.Map<Order, OrderDto>(lst);
        }

        public SaleDetailStatListDto WebSiteStatSaleDetail(SearchStatRequest request)
        {
            request.FormatDate();
            _orderItemRepository.SetCurrentUser(_accountService.GetByUserID(UserId));
            var lst = _orderItemRepository.WebSiteStatSaleDetail(request);
            lst.Stat();
            return lst;

        }

        public ReturnGoodsStatListDto WebSiteStatReturnDetail(SearchStatRequest request)
        {
            request.FormatDate();
            _orderItemRepository.SetCurrentUser(_accountService.GetByUserID(UserId));
            var lst = _orderItemRepository.WebSiteStatReturnGoods(request);
            lst.Stat();
            return lst;
        }

        public CashierList WebSiteCashier(SearchCashierRequest request)
        {
            request.FormatDate();
            _orderItemRepository.SetCurrentUser(_accountService.GetByUserID(UserId));
            var lst = _orderItemRepository.WebSiteCashier(request);
            lst.Stat();
            return lst;
        }

        public PageResult<OrderItemDto> GetOrderItemsAutoBack(string orderNo, int pageIndex, int pageSize)
        {
            _orderItemRepository.SetCurrentUser(_accountService.GetByUserID(UserId));
            var lstOrderItems = _orderItemRepository.GetOrderItemsAutoBack(orderNo, pageIndex, pageSize);

            return lstOrderItems;
        }

        public void SetSaleOrderVoid(string saleOrderNo)
        {
            _orderItemRepository.SetSaleOrderVoid(saleOrderNo);
            _saleRmaRepository.SetVoidBySaleOrder(saleOrderNo);

        }

        public IList<OPC_OrderComment> GetCommentByOderNo(string orderNo)
        {
            return _orderRemarkRepository.GetByOrderNo(orderNo);
        }
        #endregion
    
    }
}