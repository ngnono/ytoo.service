using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Web.Mvc;
using Yintai.Architecture.Common.Web.Mvc.ActionResults;
using Yintai.Hangzhou.Contract.DTO.Request;
using Yintai.Hangzhou.Contract.Response;
using Yintai.Hangzhou.Data.Models;
using Yintai.Hangzhou.Model.Enums;
using Yintai.Hangzhou.Repository.Contract;
using Yintai.Hangzhou.Service.Logic;
using Yintai.Hangzhou.WebApiCore;

namespace Yintai.Hangzhou.WebApiCore.Areas.Api2.Controllers
{
    public class OrderController:Rest2Controller
    {
        private IOrderRepository _orderRepo;
        private IOrderLogRepository _orderLogRepo;
        private IOutboundRepository _obRepo;
        private IOutboundItemRepository _obiRepo;
        public OrderController(IOrderRepository orderRepo,IOrderLogRepository orderLogRepo,IOutboundRepository obRepo,IOutboundItemRepository obiRepo)
        {
            _orderRepo = orderRepo;
            _orderLogRepo = orderLogRepo;
            _obRepo = obRepo;
            _obiRepo = obiRepo;
        }
        [HttpPost]
        public ActionResult PrepareShip(OrderPrepareShipRequest request)
        {
            if (!ModelState.IsValid)
            {
                var error = ModelState.Values.Where(v => v.Errors.Count() > 0).First();
                return this.RenderError(r => r.Message = error.Errors.First().ErrorMessage);
            }
            var orderEntity = Context.Set<Order2ExEntity>().Where(o => o.ExOrderNo == request.OrderNo)
                            .Join(Context.Set<OrderEntity>(), o => o.OrderNo, i => i.OrderNo, (o, i) => i).FirstOrDefault();
            if (orderEntity==null)
                return this.RenderError(r => r.Message = "订单不存在！");
            if (orderEntity.Status >= (int)OrderStatus.PreparePack)
                return this.RenderSuccess<BaseResponse>(null);
            using (var ts = new TransactionScope())
            {
                orderEntity.Status = (int)OrderStatus.PreparePack;
                orderEntity.UpdateDate = DateTime.Now;
                _orderRepo.Update(orderEntity);

                _orderLogRepo.Insert(new OrderLogEntity() { 
                     CreateDate = DateTime.Now,
                      CreateUser =0,
                       CustomerId = orderEntity.CustomerId,
                        Operation = request.Memo??"准备发货",
                         OrderNo = orderEntity.OrderNo,
                          Type = (int)OrderOpera.PrepareShip
                });

                ts.Complete();
                return this.RenderSuccess<BaseResponse>(null);
            }
        }

        [HttpPost]
        public ActionResult Ship(OrderShipRequest request)
        {
            if (!ModelState.IsValid)
            {
                var error = ModelState.Values.Where(v => v.Errors.Count() > 0).First();
                return this.RenderError(r => r.Message = error.Errors.First().ErrorMessage);
            }
            var ppLinq = Context.Set<ProductPropertyEntity>().Join(Context.Set<ProductPropertyValueEntity>(),o=>o.Id,i=>i.PropertyId,(o,i)=>new {PP=o,PPV=i});
            var orderEntity = Context.Set<Order2ExEntity>().Where(o => o.ExOrderNo == request.OrderNo)
                            .Join(Context.Set<OrderEntity>(), o => o.OrderNo, i => i.OrderNo, (o, i) => i)
                            .Join(Context.Set<OrderItemEntity>(), o => o.OrderNo, i => i.OrderNo, (o, i) => new { O = o, OI = i })
                            .GroupJoin(Context.Set<ProductMapEntity>(),o=>o.OI.ProductId,i=>i.ProductId,(o,i)=>new {O=o.O,OI=o.OI,EPId =i.FirstOrDefault()})
                            .GroupJoin(ppLinq.Where(pp=>pp.PP.IsSize.HasValue && pp.PP.IsSize.Value==true),o=>o.OI.SizeValueId,i=>i.PPV.Id,(o,i)=>new {O=o.O,OI=o.OI,EPId=o.EPId,SId=i.FirstOrDefault()})
                            .GroupJoin(ppLinq.Where(pp=>pp.PP.IsColor.HasValue && pp.PP.IsColor.Value==true),o=>o.OI.ColorValueId,i=>i.PPV.Id,(o,i)=>new {O=o.O,OI=o.OI,EPId=o.EPId,SId=o.SId,CId = i.FirstOrDefault()})
                            ;
                            
            if (orderEntity == null)
                return this.RenderError(r => r.Message = "订单不存在！");
            if (orderEntity.First().O.Status !=(int)OrderStatus.PreparePack)
                return this.RenderError(r=>r.Message="订单状态不是准备发货");
            
            var haveShippedEntity = Context.Set<OutboundEntity>().Where(ob => ob.SourceType == (int)OutboundType.Order && ob.SourceNo == request.OrderNo)
                                    .Join(Context.Set<OutboundItemEntity>(), o => o.OutboundNo, i => i.OutboundNo, (o, i) => new { OB = o, OBI = i })
                                    .GroupJoin(ppLinq.Where(pp => pp.PP.IsColor.HasValue && pp.PP.IsColor.Value == true), o => new { P = o.OBI.ProductId, CId = o.OBI.ColorId??0 }, i => new { P = i.PP.ProductId, CId = i.PPV.Id }, (o, i) => new { OB = o.OB, OBI = o.OBI, CId = i.FirstOrDefault() })
                                    .GroupJoin(ppLinq.Where(pp => pp.PP.IsSize.HasValue && pp.PP.IsSize.Value == true), o => new { P = o.OBI.ProductId, SId = o.OBI.SizeId??0 }, i => new { P = i.PP.ProductId, SId = i.PPV.Id }, (o, i) => new { OB = o.OB, OBI = o.OBI,CId =o.CId, SId = i.FirstOrDefault() });
            bool isAllShipped = true;
            foreach (var item in orderEntity)
            {
                var shippedQuantity = haveShippedEntity.Where(l => l.SId.PPV.Id == item.OI.SizeValueId && l.CId.PPV.Id== item.OI.ColorValueId && l.OBI.ProductId == item.OI.ProductId)
                                .Sum(l => l.OBI.Quantity);
                var shippingQuantity =  request.Products.Where(l=>l.ProductId== item.EPId.ChannelPId && l.Properties.ColorValueId==item.CId.PPV.ChannelValueId && l.Properties.SizeValueId== item.SId.PPV.ChannelValueId)
                                .Sum(l=>l.Quantity);
                if (shippedQuantity + shippingQuantity < item.OI.Quantity)
                {
                    isAllShipped = false;
                    break;
                } 
            }
            using (var ts = new TransactionScope())
            { 
                //step1: insert outbound log
                //step2: if all shipped, update order status
                var obEntity = _obRepo.Insert(new OutboundEntity() {
                     OutboundNo = OrderRule.CreateShippingCode(request.StoreId.ToString()),
                      SourceNo = request.OrderNo,
                       SourceType=(int)OutboundType.Order,
                        CreateDate=DateTime.Now,
                         CreateUser = 0,
                          Status =(int)DataStatus.Normal,
                           ShippingVia = int.Parse(request.ShipVia),
                            ShippingNo = request.ShipNo
                });

                foreach (var item in request.Products)
                {
                    var linq = Context.Set<ProductMapEntity>().Where(l => l.ChannelPId == item.ProductId)
                                .Join(ppLinq, o => o.ProductId, i => i.PP.ProductId, (o, i) => new { P = o, PP = i.PP, PPV = i.PPV });
                    _obiRepo.Insert(new OutboundItemEntity()
                    {
                        CreateDate = DateTime.Now,
                        ColorId = linq.Where(l => l.PP.IsColor.HasValue && l.PPV.ChannelValueId == item.Properties.ColorValueId).FirstOrDefault().PPV.Id,
                        OutboundNo = obEntity.OutboundNo,
                        ProductId = linq.FirstOrDefault().P.ProductId,
                        Quantity = item.Quantity,
                        SizeId = linq.Where(l => l.PP.IsSize.HasValue && l.PPV.ChannelValueId == item.Properties.SizeValueId).FirstOrDefault().PPV.Id,
                        UpdateDate = DateTime.Now
                    });
                }
                if (isAllShipped)
                {
                    var updateOrderEntity = Context.Set<Order2ExEntity>().Where(o => o.ExOrderNo == request.OrderNo)
                                            .Join(Context.Set<OrderEntity>(), o => o.OrderNo, i => i.OrderNo, (o, i) => i).FirstOrDefault();
                    updateOrderEntity.Status = (int)OrderStatus.Shipped;
                    updateOrderEntity.CreateDate = DateTime.Now;
                    _orderRepo.Update(updateOrderEntity);

                    _orderLogRepo.Insert(new OrderLogEntity() { 
                         CreateDate =request.UpdateTime,
                          CustomerId = updateOrderEntity.CustomerId,
                           Operation = "已发货",
                            OrderNo = updateOrderEntity.OrderNo,
                             Type = (int)OrderOpera.Shipping
                    });

                }
                ts.Complete();
            }
            return this.RenderSuccess<BaseResponse>(null);

        }

        [HttpPost]
        public ActionResult Void(OrderVoidRequest request)
        {
            if (!ModelState.IsValid)
            {
                var error = ModelState.Values.Where(v => v.Errors.Count() > 0).First();
                return this.RenderError(r => r.Message = error.Errors.First().ErrorMessage);
            }
            var orderEntity = Context.Set<Order2ExEntity>().Where(o => o.ExOrderNo == request.OrderNo)
                             .Join(Context.Set<OrderEntity>(), o => o.OrderNo, i => i.OrderNo, (o, i) => i).FirstOrDefault();
            if (orderEntity == null)
                return this.RenderError(r => r.Message = "订单不存在！");
            if (orderEntity.Status == (int)OrderStatus.Void)
                return this.RenderSuccess<BaseResponse>(null);
            using (var ts = new TransactionScope())
            {
                orderEntity.Status = (int)OrderStatus.Void;
                orderEntity.UpdateDate = request.UpdateTime ?? DateTime.Now;
                _orderRepo.Update(orderEntity);

                _orderLogRepo.Insert(new OrderLogEntity() { 
                     CreateDate = request.UpdateTime??DateTime.Now,
                      CreateUser =0,
                      CustomerId = 0,
                       Operation="取消订单",
                        OrderNo = orderEntity.OrderNo,
                         Type = (int)OrderOpera.SystemVoid
                });
                ts.Complete();
            }
            return this.RenderSuccess<BaseResponse>(null);

        }

        [HttpPost]
        public ActionResult CustomerReject(OrderRejectRequest request)
        {
            if (!ModelState.IsValid)
            {
                var error = ModelState.Values.Where(v => v.Errors.Count() > 0).First();
                return this.RenderError(r => r.Message = error.Errors.First().ErrorMessage);
            }
            var orderEntity = Context.Set<Order2ExEntity>().Where(o => o.ExOrderNo == request.OrderNo)
                             .Join(Context.Set<OrderEntity>(), o => o.OrderNo, i => i.OrderNo, (o, i) => i).FirstOrDefault();
            if (orderEntity == null)
                return this.RenderError(r => r.Message = "订单不存在！");
            if (orderEntity.Status == (int)OrderStatus.CustomerRejected)
                return this.RenderSuccess<BaseResponse>(null);
            using (var ts = new TransactionScope())
            {
                orderEntity.Status = (int)OrderStatus.CustomerRejected;
                orderEntity.UpdateDate = request.UpdateTime ?? DateTime.Now;
                _orderRepo.Update(orderEntity);

                _orderLogRepo.Insert(new OrderLogEntity()
                {
                    CreateDate = request.UpdateTime ?? DateTime.Now,
                    CreateUser = 0,
                    CustomerId = 0,
                    Operation = "订单拒收",
                    OrderNo = orderEntity.OrderNo,
                    Type = (int)OrderOpera.CustomerReject
                });
                ts.Complete();
            }
            return this.RenderSuccess<BaseResponse>(null);

        }
    }
}
