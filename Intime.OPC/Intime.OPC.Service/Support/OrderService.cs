using System;
using System.Collections.Generic;
using System.Linq;
using Intime.OPC.Domain;
using Intime.OPC.Domain.Dto;
using Intime.OPC.Domain.Dto.Custom;
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

        public OrderService(IOrderRepository orderRepository, IOrderRemarkRepository orderRemarkRepository, IOrderItemRepository orderItemRepository, IBrandRepository brandRepository)
            : base(orderRepository)
        {
            _orderRepository = _repository as IOrderRepository;
            _orderRemarkRepository = orderRemarkRepository;
            _orderItemRepository = orderItemRepository;
            _brandRepository = brandRepository;
        }

        public PageResult<OrderDto> GetOrder(string orderNo, string orderSource, DateTime dtStart, DateTime dtEnd,
            int storeId, int brandId, int status, string paymentType, string outGoodsType, string shippingContactPhone,
            string expressDeliveryCode, int expressDeliveryCompany, int userId, int pageIndex, int pageSize = 20)
        {
            dtStart = dtStart.Date;
            dtEnd = dtEnd.Date.AddDays(1);
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
            var lstOrder = _orderRepository.GetOrderByOderNoTime(orderNo, dtStart, dtEnd,pageIndex,pageSize);
            return Mapper.Map<Order, OrderDto>(lstOrder);
        }

        public PageResult<OrderItemDto> GetOrderItems(string orderNo,int pageIndex,int pageSize)
        {
            var lstOrderItems= _orderItemRepository.GetByOrderNo(orderNo,pageIndex,pageSize);

            return lstOrderItems;
        }

        public PageResult<OrderDto> GetOrderByShippingNo(string shippingNo, int pageIndex, int pageSize)
        {
            var lst= _orderRepository.GetOrderByShippingNo(shippingNo,pageIndex,pageSize);
            return Mapper.Map<Order, OrderDto>(lst);
        }

        public PageResult<OrderDto> GetByReturnGoodsInfo(ReturnGoodsInfoRequest request)
        {

            var lst = _orderRepository.GetByReturnGoodsInfo(request);
               return Mapper.Map<Order, OrderDto>(lst);

            //var lst = _orderRepository.GetOrder(request.OrderNo,"",request.StartDate,request.EndDate,request.StoreID.Value,
            //    -1,-1,request.PayType,"","","",-1,1,1000);
                   
            //return Mapper.Map<Order, OrderDto>(lst.Result);
        }

        //public IList<OrderDto> GetByReturnGoodsInfReturnGoodsInfoRequestet request)
        //{
        //    var lst = _orderRepository.GetByReturnGoodsInfo(request);
        //    return Mapper.Map<Order, OrderDto>(lst);
        //}

        public IList<OPC_OrderComment> GetCommentByOderNo(string orderNo)
        {
            return _orderRemarkRepository.GetByOrderNo(orderNo);
        }
        #endregion
    
    }
}