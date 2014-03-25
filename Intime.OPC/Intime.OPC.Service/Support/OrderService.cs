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
            IList<OrderDto> lstDtos=new List<OrderDto>();

            return Map.Mapper.Map<Order, OrderDto>(lstOrder);

            var o = new OrderDto();
            o.BuyDate = DateTime.Parse("2014-03-26");
            o.CustomerAddress = "北京市海淀区大地科技大厦1001";
            o.CustomerFreight = 15;
            o.CustomerName = "李健";
            o.CustomerPhone = "13112343456";
            o.CustomerRemark = "备注1";
            o.ExpressCompany = "申通";
            o.ExpressNo = "1234567890";
            o.Id = 1;
            o.IfReceipt = "否";
            o.MustPayTotal = 1234.5M;
            o.OrderChannelNo = "W123456";
            o.OrderNo = "DD123456";
            o.OrderSouce = "微信";
            o.OutGoodsDate = DateTime.Parse("2014-03-26");
            o.OutGoodsType = "z";

            var lst= new List<OrderDto>();
            lst.Add(o);
            return lst;
        }

        
    }
}
