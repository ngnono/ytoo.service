using System.Web.Mvc;
using System.Linq;
using Yintai.Architecture.Common.Models;
using Yintai.Architecture.Common.Web.Mvc.ActionResults;
using Yintai.Architecture.Common.Web.Mvc.Controllers;
using Yintai.Hangzhou.Contract.Coupon;
using Yintai.Hangzhou.Contract.DTO.Request.Coupon;
using Yintai.Hangzhou.Contract.DTO.Response.Coupon;
using Yintai.Hangzhou.Model;
using Yintai.Hangzhou.Model.Enums;
using Yintai.Hangzhou.Repository.Contract;
using Yintai.Hangzhou.WebSupport.Mvc;
using System;
using Yintai.Hangzhou.Contract.DTO.Response.Product;
using Yintai.Hangzhou.Contract.DTO.Response.Promotion;

namespace Yintai.Hangzhou.WebApiCore.Areas.Api.Controllers
{
    [RestfulAuthorize]
    public class CouponController : RestfulController
    {
        private readonly ICouponDataService _couponDataService;
        private ICouponRepository _couponRepo;
        private IProductRepository _productRepo;
        private IPromotionRepository _promotionRepo;

        public CouponController(ICouponDataService couponDataService,
            ICouponRepository couponRepo,
            IPromotionRepository promotionRepo,
            IProductRepository productRepo)
        {
            this._couponDataService = couponDataService;
            _couponRepo = couponRepo;
            _productRepo = productRepo;
            _promotionRepo = promotionRepo;
        }

        public ActionResult List(CouponInfoGetListRequest request, int? authuid, UserModel authUser)
        {
            request.AuthUid = authuid.Value;
            request.AuthUser = authUser;
            if (request == null)
                return new RestfulResult { Data = new ExecuteResult<CouponInfoResponse>(null)};
            var linq = _couponRepo.Get(c => c.User_Id == authUser.Id && c.Status != (int)CouponStatus.Deleted);
            if (request.Type.HasValue)
            {
                switch (request.Type.Value)
                { 
                    case CouponRequestType.Used:
                        linq = linq.Where(c => c.Status == (int)CouponStatus.Used);
                        break;
                    case CouponRequestType.Expired:
                        linq = linq.Where(c => c.Status != (int)CouponStatus.Used && c.ValidEndDate < DateTime.Now);
                        break;
                    case CouponRequestType.UnUsed:
                        linq = linq.Where(c => c.Status != (int)CouponStatus.Used && c.ValidEndDate >= DateTime.Now);
                        break;
                }
            }
            int totalCount = linq.Count();
            linq = linq.OrderByDescending(c => c.CreatedDate).Skip(request.Page * request.Pagesize).Take(request.Pagesize);
            var linq2 = linq.GroupJoin(_productRepo.GetAll(), o => o.FromProduct, i => i.Id, (o, i) => new { C=o,Pd =i })
                           .GroupJoin(_promotionRepo.GetAll(),o=> o.C.FromPromotion,i=>i.Id,(o,i)=>new {C =o.C,Pd = o.Pd,Pr = i});
            var responseData = from l in linq2.ToList()
                               select new CouponInfoResponse().FromEntity<CouponInfoResponse>(l.C,
                                            c => {
                                                var prod = l.Pd.FirstOrDefault();
                                                if (prod != null)
                                                {
                                                    c.ProductInfoResponse = new ProductInfoResponse().FromEntity<ProductInfoResponse>(prod);
                                                    c.ProductId = c.ProductInfoResponse.Id;
                                                    c.ProductName = c.ProductInfoResponse.Name;
                                                    c.ProductDescription = c.ProductInfoResponse.Description;
                                                }
                                                var pro = l.Pr.FirstOrDefault();
                                                if (pro != null)
                                                {
                                                    c.PromotionInfoResponse = new PromotionInfoResponse().FromEntity<PromotionInfoResponse>(pro);
                                      
                                                }
                                            });
            var response = new CouponInfoCollectionResponse(request.PagerRequest,totalCount){
                                 CouponInfoResponses = responseData.ToList()
                         };
            return new RestfulResult { Data = new ExecuteResult<CouponInfoCollectionResponse>(response) };
        }

        public ActionResult Detail(CouponInfoGetRequest request, int? authuid, UserModel authUser)
        {
            request.AuthUser = authUser;
            if (request == null)
                return new RestfulResult { Data = new ExecuteResult<CouponInfoResponse>(null) };
            var linq = _couponRepo.Get(c => c.User_Id == authUser.Id && c.Status != (int)CouponStatus.Deleted);
            var linq2 = linq.GroupJoin(_productRepo.GetAll(), o => o.FromProduct, i => i.Id, (o, i) => new { C = o, Pd = i })
                           .GroupJoin(_promotionRepo.GetAll(), o => o.C.FromPromotion, i => i.Id, (o, i) => new { C = o.C, Pd = o.Pd, Pr = i });
            var responseData = from l in linq2.ToList()
                               select new CouponInfoResponse().FromEntity<CouponInfoResponse>(l.C,
                                            c =>
                                            {
                                                var prod = l.Pd.FirstOrDefault();
                                                if (prod != null)
                                                {
                                                    c.ProductInfoResponse = new ProductInfoResponse().FromEntity<ProductInfoResponse>(prod);
                                                    c.ProductId = c.ProductInfoResponse.Id;
                                                    c.ProductName = c.ProductInfoResponse.Name;
                                                    c.ProductDescription = c.ProductInfoResponse.Description;
                                                }
                                                var pro = l.Pr.FirstOrDefault();
                                                if (pro != null)
                                                {
                                                    c.PromotionInfoResponse = new PromotionInfoResponse().FromEntity<PromotionInfoResponse>(pro);

                                                }
                                            });
            return new RestfulResult { Data = new ExecuteResult<CouponInfoResponse>(responseData.FirstOrDefault()) };
        }
    }
}
