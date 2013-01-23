using System;
using System.Linq;
using Yintai.Architecture.Common.Models;
using Yintai.Hangzhou.Contract.Coupon;
using Yintai.Hangzhou.Contract.DTO.Request.Coupon;
using Yintai.Hangzhou.Contract.DTO.Response.Coupon;
using Yintai.Hangzhou.Data.Models;
using Yintai.Hangzhou.Model.Enums;
using Yintai.Hangzhou.Repository.Contract;
using Yintai.Hangzhou.Service.Contract;
using Yintai.Hangzhou.Service.Manager;

namespace Yintai.Hangzhou.Service
{
    /// <summary>
    /// CLR Version: 4.0.30319.296
    /// NameSpace: Yintai.Hangzhou.Service
    /// FileName: CouponDataService
    ///
    /// Created at 11/27/2012 11:03:30 AM
    /// Description: 
    /// </summary>
    public class CouponDataService : BaseService, ICouponDataService
    {
        #region fields

        private readonly ITimeSeedRepository _timeSeedRepository;
        private readonly ICouponRepository _couponRepository;
        private readonly IPromotionRepository _promotionRepository;

        private readonly IUserService _userService;

        #endregion

        #region .ctor

        public CouponDataService(IUserService userService, ICouponRepository couponRepository, ITimeSeedRepository timeSeedRepository, IPromotionRepository promotionRepository)
        {
            this._timeSeedRepository = timeSeedRepository;
            this._couponRepository = couponRepository;
            this._promotionRepository = promotionRepository;
            this._userService = userService;
        }

        #endregion

        #region properties

        #endregion

        #region methods

        private void Get(CouponCouponRequest request, out int storeId, out int recommendedUserId)
        {
            switch (request.SType)
            {
                case SourceType.Promotion:
                    GetPromotion(request.SourceId, out storeId, out recommendedUserId);
                    break;
                default:
                    storeId = 0;
                    recommendedUserId = 0;
                    break;
            }
        }

        private void GetPromotion(int pid, out int storeId, out int recommendedUserId)
        {
            var entity = this._promotionRepository.GetItem(pid);
            if (entity == null)
            {
                storeId = 0;
                recommendedUserId = 0;

                return;
            }

            storeId = entity.Store_Id;
            recommendedUserId = 0;
        }

        #endregion

        #region Implementation of ICouponDataService

        /// <summary>
        /// 创建优惠券
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public ExecuteResult<CouponCodeResponse> CreateCoupon(CouponCouponRequest request)
        {
            if (request == null)
            {
                return new ExecuteResult<CouponCodeResponse>(null) { StatusCode = StatusCode.ClientError, Message = "参数错误" };
            }

            /*
             * 8位, 前三位+5位流水
             * 前三位 当天距离2012/12/31的天数，不足补0
             * 后5位为自增的流水码
             */
            return CreateCoupon4NewBy8W(request);
        }

        /// <summary>
        /// 8位code
        /// </summary>
        /// <returns></returns>
        private ExecuteResult<CouponCodeResponse> CreateCoupon4NewBy8W(CouponCouponRequest request)
        {
            /*
             * 8位, 前三位+5位流水
             * 前三位 当天距离2012/12/31的天数，不足补0
             * 后5位为自增的流水码 
             * 99999 超过就不能增加了
            */
            var date = DateTime.Now;
            var c = new DateTime(2012, 12, 31);
            var timeSpan = date - c;
            var day = timeSpan.Days;

            if (day > 999)
            {
                throw new ArgumentException("天数差超过了999天，请更改算法");
            }

            var keyPre = Architecture.Common.Helper.UtilHelper.PreFilled(day, 3, '0');

            var timeSeed = this._timeSeedRepository.CreateLimitMaxSeedV2(new TimeSeedEntity
            {
                Date = date,
                Day = date.Day,
                Hour = date.Hour,
                Month = date.Month,
                Year = date.Year
            }, 99999, keyPre);


            if (timeSeed == null)
            {
                //超出限制了
                return new ExecuteResult<CouponCodeResponse>(null)
                {
                    StatusCode = StatusCode.InternalServerError,
                    Message = "超出优惠码限制"
                };
            }

            var storeId = 0;
            var recommendedUserId = 0;

            Get(request, out storeId, out recommendedUserId);

            //create coupon
            var totay = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            var coupon = this._couponRepository.Insert(new CouponHistoryEntity
            {
                CouponId = timeSeed.KeySeed,
                CreatedDate = date,
                CreatedUser = request.AuthUid,
                FromProduct =
                    request.SType == SourceType.Product ? request.SourceId : 0,
                FromPromotion =
                    request.SType == SourceType.Promotion ? request.SourceId : 0,
                FromStore = storeId,
                FromUser = recommendedUserId,
                Id = 0,
                Status = 1,
                User_Id = request.AuthUid,
                //TODO:修改时间
                ValidStartDate = totay,//默认有效期7天
                ValidEndDate = totay.AddDays(7).AddSeconds(-1)
            });

            //增加用户优惠券数
            if (coupon != null)
            {
                //TODO: 增加用户账户 优惠券 1张
                _userService.AddCoupon(coupon.User_Id, 1, request.AuthUid);
            }

            return new ExecuteResult<CouponCodeResponse>(MappingManager.CouponCodeResponseMapping(coupon));
        }

        // /*
        /// <summary>
        /// 创建优惠券
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public ExecuteResult<CouponCodeResponse> CreateCouponOld(CouponCouponRequest request)
        {
            if (request == null)
            {
                return new ExecuteResult<CouponCodeResponse>(null) { StatusCode = StatusCode.ClientError, Message = "参数错误" };
            }

            var date = DateTime.Now;
            //            Coupon的code规则如下：
            //                YY+MM+DD+HH+{000}
            //                YY: 当天自然年的后两位
            //                MM:当天自然年的月份两位
            //                Dd：当天自然年的天数两位
            //                Hh：当时小时的两位表示
            //                {000}: 自增长三位数，如不足三位，左补0

            //Example:
            //                2012/11/23 领取的一个coupon：
            //                12112313001

            var keyPre = date.ToString("yyMMddHH");
            //create seedkey;
            var timeSeed = this._timeSeedRepository.CreateLimitMaxSeed(new TimeSeedEntity
                                                            {
                                                                Date = date,
                                                                Day = date.Day,
                                                                Hour = date.Hour,
                                                                Month = date.Month,
                                                                Year = date.Year
                                                            }, 999, keyPre);
            if (timeSeed == null)
            {
                //超出限制了
                return new ExecuteResult<CouponCodeResponse>(null)
                           {
                               StatusCode = StatusCode.InternalServerError,
                               Message = "超出优惠码限制"
                           };
            }

            var storeId = 0;
            var recommendedUserId = 0;

            Get(request, out storeId, out recommendedUserId);

            //create coupon
            var coupon = this._couponRepository.Insert(new CouponHistoryEntity
                                              {
                                                  CouponId = timeSeed.KeySeed,
                                                  CreatedDate = date,
                                                  CreatedUser = request.AuthUid,
                                                  FromProduct =
                                                      request.SType == SourceType.Product ? request.SourceId : 0,
                                                  FromPromotion =
                                                      request.SType == SourceType.Promotion ? request.SourceId : 0,
                                                  FromStore = storeId,
                                                  FromUser = recommendedUserId,
                                                  Id = 0,
                                                  Status = 1,
                                                  User_Id = request.AuthUid,
                                                  ValidStartDate = DateTime.Now,//默认有效期7天
                                                  ValidEndDate = DateTime.Now.AddDays(7)
                                              });

            return new ExecuteResult<CouponCodeResponse>(MappingManager.CouponCodeResponseMapping(coupon));
        }

        // */
        /// <summary>
        /// 获取 COUPON
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public ExecuteResult<CouponCodeResponse> Get(CouponGetRequest request)
        {
            if (request == null)
            {
                return new ExecuteResult<CouponCodeResponse>(null) { StatusCode = StatusCode.ClientError, Message = "参数错误" };
            }

            var entity = this._couponRepository.GetItem(request.CouponId);

            if (entity.User_Id == request.AuthUid && entity.CouponId == request.CouponCode)
            {
                return new ExecuteResult<CouponCodeResponse>(MappingManager.CouponCodeResponseMapping(entity));
            }

            return new ExecuteResult<CouponCodeResponse>(null) { StatusCode = StatusCode.ClientError, Message = "您不能领取他人的优惠券" };
        }

        /// <summary>
        /// 获取优惠券列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public ExecuteResult<CouponCodeCollectionResponse> GetList(CustomerCouponCodeGetListRequest request)
        {
            if (request == null)
            {
                return new ExecuteResult<CouponCodeCollectionResponse>(null) { StatusCode = StatusCode.ClientError, Message = "参数错误" };
            }

            int totalCount;
            var datas = this._couponRepository.GetPagedListByUserId(request.PagerRequest, out totalCount, request.AuthUid,
                                                        request.CouponSortOrder);
            var response = new CouponCodeCollectionResponse(request.PagerRequest, totalCount)
                {
                    CouponCodeResponses = MappingManager.CouponCodeResponseMapping(datas)
                };

            var result = new ExecuteResult<CouponCodeCollectionResponse>(response);

            return result;
        }

        public ExecuteResult<CouponInfoResponse> Get(CouponInfoGetRequest request)
        {
            if (request == null)
            {
                return new ExecuteResult<CouponInfoResponse>(null) { StatusCode = StatusCode.ClientError, Message = "参数错误" };
            }

            var entity = this._couponRepository.GetItem(request.CouponId);

            if (entity.User_Id == request.AuthUid && entity.CouponId == request.CouponCode)
            {
                return new ExecuteResult<CouponInfoResponse>(MappingManager.CouponInfoResponseMapping(entity));
            }

            return new ExecuteResult<CouponInfoResponse>(null) { StatusCode = StatusCode.ClientError, Message = "您不能领取他人的优惠券" };
        }

        public ExecuteResult<CouponInfoCollectionResponse> GetList(CouponInfoGetListRequest request)
        {
            if (request == null)
            {
                return new ExecuteResult<CouponInfoCollectionResponse>(null) { StatusCode = StatusCode.ClientError, Message = "参数错误" };
            }

            int totalCount;
            var datas = this._couponRepository.GetPagedListByUserId(request.PagerRequest, out totalCount, request.AuthUid,
                                                        request.CouponSortOrder);
            var response = new CouponInfoCollectionResponse(request.PagerRequest, totalCount)
            {
                CouponInfoResponses = MappingManager.CouponInfoResponseMapping(datas).ToList()
            };

            var result = new ExecuteResult<CouponInfoCollectionResponse>(response);

            return result;
        }

        #endregion
    }
}
