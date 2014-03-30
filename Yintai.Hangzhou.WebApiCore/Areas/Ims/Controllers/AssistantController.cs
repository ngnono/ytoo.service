using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Yintai.Architecture.Common.Data.EF;
using Yintai.Architecture.Common.Models;
using Yintai.Hangzhou.Contract.DTO.Request;
using Yintai.Hangzhou.Contract.DTO.Response;
using Yintai.Hangzhou.Data.Models;
using Yintai.Hangzhou.Model.Enums;
using Yintai.Hangzhou.WebSupport.Mvc;

namespace Yintai.Hangzhou.WebApiCore.Areas.Ims.Controllers
{
    public class AssistantController : RestfulController
    {
        private IEFRepository<IMS_AssociateSaleCodeEntity> _salescodeRepo;
        private IEFRepository<IMS_AssociateItemsEntity> _associateitemRepo;
        public AssistantController(IEFRepository<IMS_AssociateSaleCodeEntity> salescodeRepo,
            IEFRepository<IMS_AssociateItemsEntity> associateitemRepo)
        {
            _salescodeRepo = salescodeRepo;
            _associateitemRepo = associateitemRepo;
        }
        [RestfulAuthorize]
        public ActionResult Gift_Cards(PagerInfoRequest request)
        {
            var mockupResponse = new List<dynamic>();
            mockupResponse.Add(new {
                id = 1,
                desc = "mockup 礼品卡",
                image = "",
                is_online = true
            });
            var response = new PagerInfoResponse<dynamic>(request.PagerRequest, mockupResponse.Count)
            {
                Items = mockupResponse
            };
            return this.RenderSuccess<PagerInfoResponse<dynamic>>(c => c.Data = response);
        }

        [RestfulRoleAuthorize(UserLevel.DaoGou)]
        public ActionResult Combos(PagerInfoRequest request,int authuid)
        {
            var linq = Context.Set<IMS_AssociateEntity>().Where(ia=>ia.UserId == authuid)
                       .Join(Context.Set<IMS_AssociateItemsEntity>().Where(ia=>ia.ItemType==(int)ComboType.Product),o=>o.Id,i=>i.AssociateId,(o,i)=>i)
                       .Join(Context.Set<IMS_ComboEntity>(),o=>o.ItemId,i=>i.Id,(o,i)=>i)
                       .GroupJoin(Context.Set<ResourceEntity>().Where(r=>r.SourceType==(int)SourceType.Combo && r.Status==(int)DataStatus.Normal)
                                ,o=>o.Id
                                ,i=>i.SourceId
                                , (o, i) => new { A = o, R = i.OrderByDescending(ir => ir.SortOrder).FirstOrDefault() });

            int totalCount = linq.Count();
            int skipCount = request.Page > 0 ? (request.Page - 1) * request.Pagesize : 0;
            linq = linq.OrderByDescending(l => l.A.CreateDate).Skip(skipCount).Take(request.Pagesize);
            var result = linq.ToList().Select(l => new Yintai.Hangzhou.Contract.DTO.Response.IMSComboDetailResponse().FromEntity<IMSComboDetailResponse>(l.A, o =>
            {
                if (l.R == null)
                    return;
                o.ImageUrl = l.R.Name;

            }));
            var response = new PagerInfoResponse<IMSComboDetailResponse>(request.PagerRequest, totalCount)
            {
                Items = result.ToList()
            };

            return this.RenderSuccess<PagerInfoResponse<IMSComboDetailResponse>>(c => c.Data = response);
        }

        [RestfulRoleAuthorize(UserLevel.DaoGou)]
        public ActionResult Products(PagerInfoRequest request,int authuid)
        {
            var linq = Context.Set<ProductEntity>().Where(ia => ia.CreatedUser == authuid && ia.ProductType == (int)ProductType.FromSelf)
                       .GroupJoin(Context.Set<ResourceEntity>().Where(r => r.SourceType == (int)SourceType.Product && r.Status == (int)DataStatus.Normal),
                                    o => o.Id,
                                    i => i.SourceId,
                                    (o, i) => new { P = o, R = i.OrderByDescending(ir => ir.SortOrder).FirstOrDefault() });
            int totalCount = linq.Count();
            int skipCount = request.Page > 0 ? (request.Page - 1) * request.Pagesize : 0;
            linq = linq.OrderByDescending(l => l.P.CreatedDate).Skip(skipCount).Take(request.Pagesize);
            var result = linq.ToList().Select(l => new IMSProductSelfDetailResponse().FromEntity<IMSProductSelfDetailResponse>(l.P, o =>
            {
                
                if (l.R == null)
                    return;
                o.ImageUrl = l.R.Name;
            }));
            var response = new PagerInfoResponse<IMSProductSelfDetailResponse>(request.PagerRequest, totalCount)
            {
                Items = result.ToList()
            };

            return this.RenderSuccess<PagerInfoResponse<IMSProductSelfDetailResponse>>(c => c.Data = response);
           
        }

        [RestfulRoleAuthorize(UserLevel.DaoGou)]
        public ActionResult SalesCodes(int authuid)
        {
            var linq = Context.Set<IMS_AssociateEntity>().Where(ia => ia.UserId == authuid)
                            .GroupJoin(Context.Set<IMS_AssociateSaleCodeEntity>(),
                                        o => o.Id,
                                        i => i.AssociateId,
                                        (o, i) => i).FirstOrDefault();
            var  salesCodes = linq==null?new string[]{}:linq.Select(l=>l.Code).ToArray();
            var response = new PagerInfoResponse<string>(new PagerRequest(), salesCodes.Count())
            {
                Items = salesCodes.ToList()
            };


            return this.RenderSuccess<PagerInfoResponse<string>>(c => c.Data = response);
        }

        [RestfulRoleAuthorize(UserLevel.DaoGou)]
        public ActionResult SalesCode_Add(string sale_code,int authuid)
        {
            if (string.IsNullOrEmpty(sale_code))
                return this.RenderError(r => r.Message = "销售码为空");
            var linq = Context.Set<IMS_AssociateEntity>().Where(ia => ia.UserId == authuid).FirstOrDefault();
            if (linq == null)
                return this.RenderError(r => r.Message = "无权操作");

            _salescodeRepo.Insert(new IMS_AssociateSaleCodeEntity() { 
                 AssociateId = linq.Id,
                  Code = sale_code,
                   CreateDate = DateTime.Now,
                   CreateUser = authuid,
                    Status = (int)DataStatus.Normal,
                     UserId = authuid
            });
            

            return this.RenderSuccess<dynamic>(null);
        }

        [RestfulRoleAuthorize(UserLevel.DaoGou)]
        public ActionResult Brands(int authuid)
        {
            var linq = Context.Set<IMS_AssociateEntity>().Where(ia => ia.UserId == authuid)
                            .GroupJoin(Context.Set<IMS_AssociateBrandEntity>().Join(Context.Set<BrandEntity>().Where(b=>b.Status==(int)DataStatus.Normal),
                                                                    o => o.BrandId, 
                                                                    i => i.Id, 
                                                                    (o, i) => new { AB=o,B=i}),
                                        o => o.Id,
                                        i => i.AB.AssociateId,
                                        (o, i) => i).FirstOrDefault();
            List<dynamic> brands = null;
            if (linq != null)
                brands = linq.Select(l => new
                {
                    id = l.AB.BrandId,
                    name = l.B.Name
                }).ToList<dynamic>();
            else
                brands = new List<dynamic>();
            var response = new PagerInfoResponse<dynamic>(new PagerRequest(), brands.Count())
            {
                Items = brands
            };


            return this.RenderSuccess<PagerInfoResponse<dynamic>>(c => c.Data = response);
           
        }

        [RestfulRoleAuthorize(UserLevel.DaoGou)]
        public ActionResult Combo_Status_Update(IMSComboStatusUpdateRequest request,int authuid)
        {
            var comboItemEntity = Context.Set<IMS_AssociateEntity>().Where(ia => ia.UserId == authuid)
                                .Join(Context.Set<IMS_AssociateItemsEntity>().Where(iai=>iai.ItemType==request.Item_Type && iai.ItemId == request.Item_Id), o => o.Id, i => i.AssociateId, (o, i) => i)
                                .FirstOrDefault();
            if (comboItemEntity == null)
                return this.RenderError(r => r.Message = "无权操作该搭配");
            comboItemEntity.Status = (int)DataStatus.Default;
            comboItemEntity.UpdateDate = DateTime.Now;
            _associateitemRepo.Update(comboItemEntity);


            return this.RenderSuccess<dynamic>(null);

        }
        [RestfulAuthorize]
        public ActionResult Income(int authuid)
        {
            return this.RenderSuccess<dynamic>(c => c.Data = new { 
                received_amount = 1000,
                frozen_amount = 100,
                request_amount = 200,
                avail_amount = 300

            });
        }

        [RestfulAuthorize]
        public ActionResult Income_Received(PagerInfoRequest request, int authuid)
        {
            var mockupResponse = new List<dynamic>();
            mockupResponse.Add(new
            {
                id = 1,
                create_date = "2014-3-22",
                bankname = "建设银行",
                cardno = "622222222xxxxxx2222",
                amount = 100.1
            });
            mockupResponse.Add(new
            {
                id = 1,
                create_date = "2014-3-22",
                bankname = "建设银行",
                cardno = "622222222xxxxxx2223",
                amount = 101.1
            });

            var response = new PagerInfoResponse<dynamic>(request.PagerRequest, mockupResponse.Count)
            {
                Items = mockupResponse
            };
            return this.RenderSuccess<PagerInfoResponse<dynamic>>(c => c.Data = response);
        }

        [RestfulAuthorize]
        public ActionResult Order_GiftCards(PagerInfoRequest request, int authuid)
        {
            var mockupResponse = new List<dynamic>();
            mockupResponse.Add(new
            {
               
                create_date = "2014-3-22",
                amount = 100.1
            });
            mockupResponse.Add(new
            {
                create_date = "2014-3-22",
                amount = 101.1
            });

            var response = new PagerInfoResponse<dynamic>(request.PagerRequest, mockupResponse.Count)
            {
                Items = mockupResponse
            };
            return this.RenderSuccess<PagerInfoResponse<dynamic>>(c => c.Data = response);
        }

        [RestfulAuthorize]
        public ActionResult Orders(PagerInfoRequest request, int authuid)
        {
            var mockupResponse = new List<dynamic>();
            mockupResponse.Add(new
            {

                create_date = "2014-3-22",
                amount = 100.1,
                order_no="20100000001",
                status = "已支付"
            });
            mockupResponse.Add(new
            {
                create_date = "2014-3-23",
                amount = 100.1,
                order_no = "20100000002",
                status = "已支付"
            });

            var response = new PagerInfoResponse<dynamic>(request.PagerRequest, mockupResponse.Count)
            {
                Items = mockupResponse
            };
            return this.RenderSuccess<PagerInfoResponse<dynamic>>(c => c.Data = response);
        }

        [RestfulAuthorize]
        public ActionResult Update(Yintai.Hangzhou.Contract.DTO.Request.IMSStoreDetailUpdateRequest request)
        {

            return this.RenderSuccess<dynamic>(c => c.Data = new { 
                name = "mockup update",
                phone = "130000000"
            });
        }

        [RestfulAuthorize]
        public ActionResult Update_Logo(FormCollection form)
        {

            return this.RenderSuccess<dynamic>(c => c.Data = new
            {
                logo = ""
            });
        }

        [RestfulAuthorize]
        public ActionResult Update_Template(int templateId, int authuid)
        {

            return this.RenderSuccess<dynamic>(null);
        }

        [RestfulAuthorize]
        public ActionResult Feedback(string comment, int authuid)
        {

            return this.RenderSuccess<dynamic>(null);
        }

    }
}
