using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intime.OPC.Domain.Dto;
using Intime.OPC.Domain.Models;
using Intime.OPC.Repository;

namespace Intime.OPC.Service.Support
{
    public class OrderService:IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        public OrderService(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public IList<OrderDto> GetOrder(string orderNo, string orderSource, DateTime dtStart, DateTime dtEnd, int storeId, int brandId, int status, string paymentType, string outGoodsType, string shippingContactPhone, string expressDeliveryCode, int expressDeliveryCompany, int userId)
        {

            var lstOrder=  _orderRepository.GetOrder(orderNo, orderSource, dtStart, dtEnd, storeId, brandId, status, paymentType,
                outGoodsType, shippingContactPhone, expressDeliveryCode, expressDeliveryCompany);
 
            return Map.Mapper.Map<Order, OrderDto>(lstOrder);
        }

        
    }
}
