using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Web.Mvc;
using Yintai.Architecture.Common.Models;
using Yintai.Architecture.Common.Web.Mvc.ActionResults;
using Yintai.Architecture.Common.Web.Mvc.Controllers;
using Yintai.Hangzhou.Contract.DTO.Request;
using Yintai.Hangzhou.Contract.DTO.Response;
using Yintai.Hangzhou.Contract.DTO.Response.Resources;
using Yintai.Hangzhou.Data.Models;
using Yintai.Hangzhou.Model;
using Yintai.Hangzhou.Model.Enums;
using Yintai.Hangzhou.Repository.Contract;
using Yintai.Hangzhou.Service.Logic;
using Yintai.Hangzhou.WebSupport.Mvc;

namespace Yintai.Hangzhou.WebApiCore.Areas.Api.Controllers
{
    public class OrderController:RestfulController
    {
        private IOrderLogRepository _orderlogRepo;
        private IOrderRepository _orderRepo;
        private IRMARepository _rmaRepo;
        private IRMAItemRepository _rmaitemRepo;
        private IRMALogRepository _rmalogRepo;
        public OrderController(IOrderRepository orderRepo,IOrderLogRepository orderlogRepo,
            IRMARepository rmaRepo,
            IRMAItemRepository rmaitemRepo,
            IRMALogRepository rmalogRepo)
        {
            _orderlogRepo = orderlogRepo;
            _orderRepo = orderRepo;
            _rmaRepo = rmaRepo;
            _rmaitemRepo = rmaitemRepo;
            _rmalogRepo = rmalogRepo;
        }
        [RestfulAuthorize]
        public RestfulResult My(MyOrderRequest request, UserModel authUser)
        {
            var dbContext = Context;
            var onGoStatus = new int[]{(int)OrderStatus.Create,
                                        (int)OrderStatus.AgentConfirmed,
                                        (int)OrderStatus.CustomerConfirmed,
                                        (int)OrderStatus.CustomerReceived,
                                        (int)OrderStatus.OrderPrinted,
                                        (int)OrderStatus.PreparePack,
                                        (int)OrderStatus.Shipped
                };
            var linq = Context.Set<OrderEntity>().Where(o=>o.CustomerId == authUser.Id);
            switch(request.Type)
            {
                case OrderRequestType.OnGoing:
                    linq = linq.Where(o => onGoStatus.Any(status => status == o.Status));
                    break;
                case OrderRequestType.Complete:
                    linq = linq.Where(o=>o.Status==(int)OrderStatus.Convert2Sales ||
                                        o.Status==(int)OrderStatus.CustomerRejected);
                    break;
                case OrderRequestType.Void:
                    linq = linq.Where(o=>o.Status == (int)OrderStatus.Void);
                    break;
            }
            var linq2 = linq.GroupJoin(dbContext.Set<OrderItemEntity>().GroupJoin(dbContext.Set<ResourceEntity>().Where(r=>r.SourceType==(int)SourceType.Product &&r.Type==(int)ResourceType.Image),
                                                     o=>o.ProductId,
                                                     i=>i.SourceId,
                                                     (o,i)=>new {OI=o,R=i.OrderByDescending(r=>r.SortOrder).FirstOrDefault()}),
                                        o => o.OrderNo,
                                        i => i.OI.OrderNo,
                                        (o, i) => new { O = o, R = i.FirstOrDefault() });

            int totalCount = linq2.Count();
            int skipCount = request.Page > 0 ? (request.Page - 1) * request.Pagesize : 0;
            linq2 = linq2.OrderByDescending(l => l.O.CreateDate).Skip(skipCount).Take(request.Pagesize);
            var result = linq2.ToList().Select(l=>new MyOrderDetailResponse().FromEntity<MyOrderDetailResponse>(l.O,o=>{
                if (l.R == null)
                    return;
                o.ProductResource = new ResourceInfoResponse().FromEntity<ResourceInfoResponse>(l.R.R);
            }));
            var response = new PagerInfoResponse<MyOrderDetailResponse>(request.PagerRequest, totalCount)
            {
                Items = result.ToList()
            };
            return new RestfulResult { Data = new ExecuteResult<PagerInfoResponse<MyOrderDetailResponse>>(response) };
        }
        [RestfulAuthorize]
        public ActionResult Detail(MyOrderDetailRequest request, UserModel authUser)
        {
            var dbContext = Context;
            var linq = dbContext.Set<OrderEntity>().Where(o => o.OrderNo == request.OrderNo && o.CustomerId == authUser.Id)
                        .GroupJoin(dbContext.Set<OrderItemEntity>(),
                            o => o.OrderNo,
                            i => i.OrderNo,
                            (o, i) => new { O = o, OI = i.FirstOrDefault() })
                        .FirstOrDefault();
            if (linq == null)
            {
                return this.RenderError(m => m.Message = "订单号不存在");
            }
            var result = new MyOrderDetailResponse().FromEntity<MyOrderDetailResponse>(linq.O, o =>
            {

                o.Product = new MyOrderItemDetailResponse().FromEntity<MyOrderItemDetailResponse>(linq.OI, op =>
                {
                    op.ProductResource = new ResourceInfoResponse().FromEntity<ResourceInfoResponse>(
                            dbContext.Set<ResourceEntity>()
                                     .Where(resource => resource.SourceType == (int)SourceType.Product && resource.SourceId == linq.OI.ProductId && resource.Type == (int)ResourceType.Image)
                                     .OrderByDescending(resource => resource.SortOrder).FirstOrDefault());


                });
                o.ProductResource = o.Product.ProductResource;
                o.RMAs = dbContext.Set<RMAEntity>().Where(r => r.OrderNo == o.OrderNo).OrderByDescending(r => r.CreateDate)
                        .ToList()
                        .Select(r => new MyRMAResponse().FromEntity<MyRMAResponse>(r));

            });
            return this.RenderSuccess<MyOrderDetailResponse>(r => r.Data=result);
        }

        [RestfulAuthorize]
        public ActionResult Void(MyOrderDetailRequest request, UserModel authUser)
        {
            var dbContext = Context;
            var linq = dbContext.Set<OrderEntity>().Where(o => o.OrderNo == request.OrderNo && o.CustomerId == authUser.Id).FirstOrDefault();
                      
            if (linq == null)
            {
                return this.RenderError(m => m.Message = "订单号不存在");
            }
           var voidStatus = new int[]{(int)OrderStatus.Create,
                                        (int)OrderStatus.AgentConfirmed,
                                        (int)OrderStatus.CustomerConfirmed,
                                        (int)OrderStatus.CustomerReceived,
                                        (int)OrderStatus.OrderPrinted};
           if (!voidStatus.Any(s=>s==linq.Status))
           {
               return this.RenderError(m => m.Message = "订单状态现在不能取消");
           }
           using (var ts = new TransactionScope())
           {
               linq.Status = (int)OrderStatus.Void;
               linq.UpdateDate = DateTime.Now;
               linq.UpdateUser = authUser.Id;
               _orderRepo.Update(linq);

               _orderlogRepo.Insert(new OrderLogEntity() {
                 CreateDate = DateTime.Now,
                  CreateUser = authUser.Id,
                   CustomerId = authUser.Id,
                     OrderNo = linq.OrderNo,
                      Type = (int)OrderOpera.CustomerVoid,
                       Operation="用户取消订单。"
               });
               ts.Complete();
           }
           return Detail(request,authUser);
        }

        [RestfulAuthorize]
        public ActionResult RMA(RMARequest request, UserModel authUser)
        {
            var dbContext = Context;
            var orderEntity = dbContext.Set<OrderEntity>().Where(o => o.OrderNo == request.OrderNo && o.CustomerId == authUser.Id).FirstOrDefault();
            if (orderEntity == null)
            {
                return this.RenderError(m => m.Message = "订单号不存在");
            }
           
            if (orderEntity.Status!=(int)OrderStatus.Convert2Sales) 
            {
                return this.RenderError(m => m.Message = "订单状态现在不能申请退货");
            }
            var rmaEntity = dbContext.Set<RMAEntity>().Where(r => r.OrderNo == request.OrderNo
                               && r.Status != (int)RMAStatus.Reject2Customer && r.Status != (int)RMAStatus.Void).FirstOrDefault();
            if (rmaEntity != null)
            {
                return this.RenderError(m => m.Message = "已经申请了退货单，不能再次申请！");
            }
            var orderItemEntity = dbContext.Set<OrderItemEntity>().Where(o => o.OrderNo == request.OrderNo).FirstOrDefault();
            using (var ts = new TransactionScope())
            {
                var newRma =_rmaRepo.Insert(new RMAEntity() {
                     RMANo = OrderRule.CreateRMACode(),
                      Reason = request.Reason,
                       Status = (int)RMAStatus.Created,
                        OrderNo = request.OrderNo,
                         BankAccount = request.BankAccount,
                          BankCard = request.BankCard,
                           BankName = request.BankName,
                            CreateDate = DateTime.Now,
                             CreateUser = authUser.Id,
                              RMAAmount = orderEntity.TotalAmount,
                               RMAType = (int)RMAType.FromOnline,
                               UpdateDate = DateTime.Now,
                                UpdateUser = authUser.Id,
                                ContactPhone = request.ContactPhone
                });
                _rmaitemRepo.Insert(new RMAItemEntity()
                {
                    CreateDate = DateTime.Now,
                    ItemPrice = orderItemEntity.ItemPrice,
                    ExtendPrice = orderItemEntity.ExtendPrice,
                    ProductDesc = orderItemEntity.ProductDesc,
                    ProductId = orderItemEntity.ProductId,
                    Quantity = orderItemEntity.Quantity,
                    RMANo = newRma.RMANo,
                    Status = (int)DataStatus.Normal,
                    UnitPrice = orderItemEntity.UnitPrice,
                    UpdateDate = DateTime.Now,
                    StoreItem = orderItemEntity.StoreItemNo,
                    StoreDesc = orderItemEntity.StoreItemDesc

                });
                _rmalogRepo.Insert(new RMALogEntity
                {
                    CreateDate = DateTime.Now,
                    CreateUser = authUser.Id,
                    RMANo =newRma.RMANo,
                     Operation="申请线上退货"
                });
                ts.Complete();
                return this.RenderSuccess<MyRMAResponse>(r => r.Data = new MyRMAResponse().FromEntity<MyRMAResponse>(newRma));
                
            }
            
        }
    }
}
