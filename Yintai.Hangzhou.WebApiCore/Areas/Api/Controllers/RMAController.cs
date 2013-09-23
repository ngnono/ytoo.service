using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Web.Mvc;
using Yintai.Architecture.Common.Models;
using Yintai.Hangzhou.Contract.DTO.Request;
using Yintai.Hangzhou.Contract.DTO.Response;
using Yintai.Hangzhou.Contract.DTO.Response.Resources;
using Yintai.Hangzhou.Data.Models;
using Yintai.Hangzhou.Model;
using Yintai.Hangzhou.Model.Enums;
using Yintai.Hangzhou.Repository.Contract;
using Yintai.Hangzhou.WebSupport.Mvc;

namespace Yintai.Hangzhou.WebApiCore.Areas.Api.Controllers
{
    public class RMAController:RestfulController
    {
        private IRMARepository _rmaRepo;
        private IRMALogRepository _rmalogRepo;
        public RMAController(IRMARepository rmaRepo,IRMALogRepository rmalogRepo)
        {
            _rmaRepo = rmaRepo;
            _rmalogRepo = rmalogRepo;
        }
        [RestfulAuthorize]
        public ActionResult List(PagerInfoRequest request, UserModel authUser)
        {
            var dbContext = Context;
            var linq = Context.Set<OrderEntity>().Where(o => o.CustomerId == authUser.Id);
           
            var linq2 = linq.GroupJoin(dbContext.Set<OrderItemEntity>().GroupJoin(dbContext.Set<ResourceEntity>().Where(r => r.SourceType == (int)SourceType.Product && r.Type == (int)ResourceType.Image),
                                                     o => o.ProductId,
                                                     i => i.SourceId,
                                                     (o, i) => new { OI = o, R = i.OrderByDescending(r => r.SortOrder).FirstOrDefault() }),
                                        o => o.OrderNo,
                                        i => i.OI.OrderNo,
                                        (o, i) => new { O = o, R = i })
                              .GroupJoin(dbContext.Set<RMAEntity>(),o=>o.O.OrderNo,i=>i.OrderNo,(o,i)=>new {O=o.O,R=o.R,RMA=i});

            int totalCount = linq2.Count();
            int skipCount = request.Page > 0 ? (request.Page - 1) * request.Pagesize : 0;
            linq2 = linq2.OrderByDescending(l => l.O.CreateDate).Skip(skipCount).Take(request.Pagesize);
            var result = linq2.ToList().Select(l => new MyOrderDetailResponse().FromEntity<MyOrderDetailResponse>(l.O, o =>
            {
                if (l.R == null)
                    return;
                o.Products = l.R.Select(oi => new MyOrderItemDetailResponse().FromEntity<MyOrderItemDetailResponse>(oi.OI, product =>
                {
                    product.ProductResource = new ResourceInfoResponse().FromEntity<ResourceInfoResponse>(l.R.FirstOrDefault());
                }));
                o.RMAs = l.RMA.Select(rma => new MyRMAResponse().FromEntity<MyRMAResponse>(rma));
                     

            }));
            var response = new PagerInfoResponse<MyOrderDetailResponse>(request.PagerRequest, totalCount)
            {
                Items = result.ToList()
            };
            return this.RenderSuccess<PagerInfoResponse<MyOrderDetailResponse>>(r => r.Data = response);

        }
         [RestfulAuthorize]
        public ActionResult Detail(RMAInfoRequest request, UserModel authUser)
        {
            var rmaEntity = Context.Set<RMAEntity>().Where(r => r.RMANo == request.RMANo).FirstOrDefault();
            if (rmaEntity == null)
                return this.RenderError(r => r.Message = "RMANo 不存在");
            var response = new RMAInfoResponse().FromEntity<RMAInfoResponse>(rmaEntity, r => { 
                var rmaItemsEntity = Context.Set<RMAItemEntity>().Where(ri=>ri.RMANo== request.RMANo)
                                .GroupJoin(Context.Set<ResourceEntity>().Where(resource=>resource.SourceType==(int)SourceType.Product),
                                        o=>o.ProductId,
                                        i=>i.SourceId,
                                        (o,i)=>new {R=o,RR=i.OrderByDescending(resource=>resource.SortOrder).FirstOrDefault()});
                r.Products = rmaItemsEntity.ToList().Select(ri=>new RMAItemInfoResponse().FromEntity<RMAItemInfoResponse>(ri,ritem=>{
                   ritem.ProductResource = new ResourceInfoResponse().FromEntity<ResourceInfoResponse>(ri.RR);
                }));

            });
            return this.RenderSuccess<RMAInfoResponse>(r=>r.Data = response);
        }
         [RestfulAuthorize]
         public ActionResult Update(RMAUpdateRequest request, UserModel authUser)
         {
             if (!ModelState.IsValid)
             {
                 var error = ModelState.Values.Where(v => v.Errors.Count() > 0).First();
                 return this.RenderError(r => r.Message = error.Errors.First().ErrorMessage);
             }
             var rmaEntity = Context.Set<RMAEntity>().Where(r => r.RMANo == request.RMANo).FirstOrDefault();
             if (rmaEntity == null)
             {
                 return this.RenderError(r => r.Message = "RMANo不存在");
             }
             if (rmaEntity.Status!=(int)RMAStatus.Created)
             {
                 return this.RenderError(r => r.Message = "RMA状态不正确");
             }
             using(var ts = new TransactionScope())
             {
                 rmaEntity.BankAccount = request.BankAccount;
                 rmaEntity.BankCard = request.BankCard;
                 rmaEntity.BankName =request.BankName;
                 rmaEntity.ContactPhone = request.ContactPhone;
                 rmaEntity.ShipNo = request.ShipViaNo;
                 rmaEntity.ShipviaId = request.ShipVia;
                 rmaEntity.UpdateDate = DateTime.Now;
                 rmaEntity.UpdateUser = authUser.Id;
                 rmaEntity.Status = (int)RMAStatus.CustomerConfirmed;
                 _rmaRepo.Update(rmaEntity);

                 _rmalogRepo.Insert(new RMALogEntity(){
                     CreateDate = DateTime.Now,
                      CreateUser = authUser.Id,
                       RMANo = request.RMANo,
                        Operation="用户填写银行信息"
                 });
                 ts.Complete();
             }
  
             return this.RenderSuccess<RMAInfoResponse>(null);
         }
    }
}
