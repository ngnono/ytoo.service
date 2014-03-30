using System;
using System.Collections.Generic;
using Intime.OPC.Domain.Dto;
using Intime.OPC.Domain.Exception;
using Intime.OPC.Domain.Models;
using Intime.OPC.Repository;
using Intime.OPC.Service.Map;

namespace Intime.OPC.Service.Support
{
    public class OrderService : BaseService, IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderRemarkRepository _orderRemarkRepository;
        public OrderService(IOrderRepository orderRepository, IOrderRemarkRepository orderRemarkRepository)
        {
            _orderRepository = orderRepository;
            _orderRemarkRepository = orderRemarkRepository;
        }

        public IList<OrderDto> GetOrder(string orderNo, string orderSource, DateTime dtStart, DateTime dtEnd,
            int storeId, int brandId, int status, string paymentType, string outGoodsType, string shippingContactPhone,
            string expressDeliveryCode, int expressDeliveryCompany, int userId)
        {
            IList<Order> lstOrder = _orderRepository.GetOrder(orderNo, orderSource, dtStart.Date, dtEnd.Date.AddDays(1), storeId, brandId,
                status, paymentType,
                outGoodsType, shippingContactPhone, expressDeliveryCode, expressDeliveryCompany);

            return Mapper.Map<Order, OrderDto>(lstOrder);
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

        public IList<OrderDto> GetOrderByOderNoTime(string orderNo, DateTime starTime, DateTime endTime)
        {
            var lstOrder = _orderRepository.GetOrderByOderNoTime(orderNo, starTime, endTime);
            return Mapper.Map<Order, OrderDto>(lstOrder);
        }

        public IList<OPC_OrderComment> GetCommentByOderNo(string orderNo)
        {
            return _orderRemarkRepository.GetByOrderNo(orderNo);
        }
        #endregion
    
    }
}