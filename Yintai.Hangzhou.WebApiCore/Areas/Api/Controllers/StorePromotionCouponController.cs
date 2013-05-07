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
using Yintai.Hangzhou.Contract.DTO.Request.Coupon;
using Yintai.Hangzhou.Contract.DTO.Response;
using Yintai.Hangzhou.Contract.Group;
using Yintai.Hangzhou.Data.Models;
using Yintai.Hangzhou.Model;
using Yintai.Hangzhou.Model.Enums;
using Yintai.Hangzhou.Repository.Contract;
using Yintai.Hangzhou.Service.Contract.Apis;
using Yintai.Hangzhou.Service.Logic;
using Yintai.Hangzhou.Service.Manager;
using Yintai.Hangzhou.WebSupport.Mvc;

namespace Yintai.Hangzhou.WebApiCore.Areas.Api.Controllers
{
    public class StorePromotionCouponController:RestfulController
    {
        private ICardRepository _cardRepo;
        private IGroupCardService _groupData;
        private IStoreCouponsRepository _storecouponRepo;
        private IStorePromotionRepository _storeproRepo;
        private IPointRepository _pointRepo;
        public StorePromotionCouponController(ICardRepository cardRepo,
            IGroupCardService groupData,
            IStoreCouponsRepository storecouponRepo,
            IStorePromotionRepository storeproRepo,
            IPointRepository pointRepo)
        {
            _cardRepo = cardRepo;
            _groupData = groupData;
            _storecouponRepo = storecouponRepo;
            _storeproRepo = storeproRepo;
            _pointRepo = pointRepo;
        }
        [HttpPost]
        [RestfulAuthorize]
        public ActionResult Exchange(ExchangeStoreCouponRequest request, int? authuid, UserModel authUser)
        {
            request.AuthUser = authUser;
            var storepromotion = _storeproRepo.Get(sp => sp.Id == request.StorePromotionId && sp.ActiveStartDate <= DateTime.Now && sp.ActiveEndDate >= DateTime.Now).FirstOrDefault();
            if (storepromotion == null)
                return new RestfulResult
                {
                    Data = new ExecuteResult { StatusCode = StatusCode.InternalServerError, Message = "促销活动无效!" }

                };
            if (storepromotion.MinPoints > request.Points)
            {
                return new RestfulResult
                {
                    Data = new ExecuteResult { StatusCode = StatusCode.InternalServerError, Message = "兑换积分需大于最小积分限制!" }

                }; 
            }
            var cardInfo = _cardRepo.Get(c => c.User_Id == authUser.Id && c.Status != (int)DataStatus.Deleted).FirstOrDefault();
            if (cardInfo == null)
                return new RestfulResult { 
                    Data =   new ExecuteResult { StatusCode = StatusCode.InternalServerError, Message = "没有绑定卡！" }
                };
            var pointResult =_groupData.GetPoint(new GroupCardPointRequest() { CardNo = cardInfo.CardNo });
            if (pointResult == null)
                return new RestfulResult
                {
                    Data = new ExecuteResult {  StatusCode = StatusCode.InternalServerError,Message="会员卡信息错误!"}

                };
            if (pointResult.Point < request.Points)
                return new RestfulResult
                {
                    Data = new ExecuteResult { StatusCode = StatusCode.InternalServerError, Message = "积分不足!" }

                };
             StoreCouponEntity newCoupon = null;
            using (var ts = new TransactionScope())
            { 
                // step1: create coupon code
               newCoupon= _storecouponRepo.Insert(new StoreCouponEntity()
                {
                    Amount = StorePromotionRule.AmountFromPoints(request.StorePromotionId, request.Points),
                    CreateDate = DateTime.Now,
                    CreateUser = authUser.Id,
                    Points = request.Points,
                    Status = (int)CouponStatus.Normal,
                    StorePromotionId = request.StorePromotionId,
                    UpdateDate = DateTime.Now,
                    UpdateUser = authUser.Id,
                    UserId = authUser.Id,
                    ValidStartDate = storepromotion.CouponStartDate,
                    ValidEndDate = storepromotion.CouponEndDate,
                    VipCard = cardInfo.CardNo,
                    Code = StorePromotionRule.CreateCode(request.StorePromotionId)
                });
                // step2: deduce points
               var exchangeResult = _groupData.Exchange(new GroupExchangeRequest(){
                     CardNo = cardInfo.CardNo,
                      IdentityNo = request.IdentiyNo
                });
                // step3: commit
                if (exchangeResult.Success)
                    ts.Complete();
                else
                {
                     return new RestfulResult
                {
                    Data = new ExecuteResult { StatusCode = StatusCode.InternalServerError, Message = "积分扣减异常!" }

                };
                }
            }
            return new RestfulResult { Data = new ExecuteResult<ExchangeStoreCouponResponse>(new ExchangeStoreCouponResponse().FromEntity<ExchangeStoreCouponResponse>(newCoupon)) };
        }
        [RestfulAuthorize]
        public ActionResult List(StoreCouponListRequest request, int? authuid, UserModel authUser)
        {
            request.AuthUid = authuid.Value;
            request.AuthUser = authUser;
            if (request == null)
                return new RestfulResult { Data = new ExecuteResult<StoreCouponListRequest>(null) };
            var linq = _storecouponRepo.Get(c => c.UserId == authUser.Id && c.Status != (int)CouponStatus.Deleted);
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
            linq = linq.OrderByDescending(c => c.CreateDate).Skip(request.Page * request.Pagesize).Take(request.Pagesize);
            var linq2 = linq.Join(_storeproRepo.GetAll(), o => o.StorePromotionId, i => i.Id, (o, i) => new { SC = o, SP = i });
                          
            var responseData = from l in linq2.ToList()
                               select new StoreCouponDetailResponse().FromEntity<StoreCouponDetailResponse>(l.SC,
                                            c =>
                                            {
                                                c.Promotion = new StorePromotionDetailResponse().FromEntity<StorePromotionDetailResponse>(l.SP);
                                              
                                            });
            var response = new PagerInfoResponse<StoreCouponDetailResponse>(request.PagerRequest, totalCount)
            {
                Items = responseData.ToList()
            };
            return new RestfulResult { Data = new ExecuteResult<PagerInfoResponse<StoreCouponDetailResponse>>(response) };
        }
        [RestfulAuthorize]
        public ActionResult Detail(StoreCouponDetailRequest request, int? authuid, UserModel authUser)
        {
            request.AuthUser = authUser;
            if (request == null)
                return new RestfulResult { Data = new ExecuteResult<StoreCouponListRequest>(null) };
            var linq = _storecouponRepo.Get(c => c.Id == request.StoreCouponId && c.UserId== authUser.Id && c.Status != (int)CouponStatus.Deleted);
            var linq2 = linq.Join(_storeproRepo.GetAll(), o => o.StorePromotionId, i => i.Id, (o, i) => new { SC = o, SP = i });
                           
            var responseData = from l in linq2.ToList()
                               select new StoreCouponDetailResponse().FromEntity<StoreCouponDetailResponse>(l.SC,
                                            c =>
                                            {
                                                c.Promotion = new StorePromotionDetailResponse().FromEntity<StorePromotionDetailResponse>(l.SP);
                                              
                                            });
            return new RestfulResult { Data = new ExecuteResult<StoreCouponDetailResponse>(responseData.FirstOrDefault()) };
        }

        [RestfulAuthorize]
        public ActionResult Void(StoreCouponDetailRequest request, int? authuid, UserModel authUser)
        {
            request.AuthUser = authUser;
            var coupon = _storecouponRepo.Get(sp => sp.Id == request.StoreCouponId && sp.Status!=(int)CouponStatus.Used && sp.ValidEndDate >= DateTime.Now).FirstOrDefault();
            if (coupon == null)
                return new RestfulResult
                {
                    Data = new ExecuteResult { StatusCode = StatusCode.InternalServerError, Message = "代金券已失效!" }

                };

            using (var ts = new TransactionScope())
            {
                // step1: void coupon
                coupon.Status = (int)CouponStatus.Deleted;
                _storecouponRepo.Update(coupon);
                //step2: rebate points of app

               var newPoint = _pointRepo.Insert(new PointHistoryEntity()
                {
                    CreatedDate = DateTime.Now,
                    CreatedUser = authUser.Id,
                    Description = "取消代金券退回积分",
                    Name = "取消代金券",
                    PointSourceId = request.StoreCouponId,
                    PointSourceType = (int)PointSourceType.Group,
                    Status = (int)DataStatus.Normal,
                    UpdatedDate = DateTime.Now,
                    UpdatedUser = authUser.Id,
                    User_Id = authUser.Id,
                    Type = (int)PointType.VoidCoupon,
                    Amount = ConfigManager.Point2GroupRatio * coupon.Points.Value

                });
               
                // step3: commit
               if (newPoint != null)
               {
                   ts.Complete();
               }
               else
               {
                   return new RestfulResult
                   {
                       Data = new ExecuteResult { StatusCode = StatusCode.InternalServerError, Message = "取消代金券异常!" }

                   };
               }
            }
            return new RestfulResult { Data = new ExecuteResult<ExchangeStoreCouponResponse>(new ExchangeStoreCouponResponse().FromEntity<ExchangeStoreCouponResponse>(coupon)) };
        }
    }
}

 