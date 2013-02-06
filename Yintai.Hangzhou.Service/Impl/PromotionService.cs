using System;
using System.Collections.Generic;
using Yintai.Hangzhou.Data.Models;
using Yintai.Hangzhou.Model.Enums;
using Yintai.Hangzhou.Repository.Contract;
using Yintai.Hangzhou.Service.Contract;

namespace Yintai.Hangzhou.Service.Impl
{
    public class PromotionService : BaseService, IPromotionService
    {
        private readonly IPromotionProductRelationRepository _pprRepository;
        private readonly IPromotionRepository _promotionRepository;
        private readonly ICouponService _couponService;

        public PromotionService(ICouponService couponService, IPromotionRepository promotionRepository, IPromotionProductRelationRepository pprRepository)
        {
            _pprRepository = pprRepository;
            _promotionRepository = promotionRepository;
            _couponService = couponService;
        }

        public bool Exists(int promotionid, int productid)
        {
            return _pprRepository.Exists(promotionid, productid);
        }

        public PromotionEntity Get(int promotionid)
        {
            return _promotionRepository.GetItem(promotionid);
        }

        public string Verification(PromotionEntity promotionEntity)
        {
            if (promotionEntity == null)
            {
                return "活动不存在";
            }

            if (promotionEntity.StartDate > DateTime.Now)
            {
                return "活动还没有开始";
            }

            if (promotionEntity.EndDate < DateTime.Now)
            {
                return "活动已经结束";
            }

            if (promotionEntity.PublicationLimit != null && promotionEntity.PublicationLimit > 0)
            {
                //验证优惠券领取数
                var c = _couponService.GetCouponCount(promotionEntity.Id, SourceType.Promotion);
                if (promotionEntity.PublicationLimit >= c)
                {
                    return "活动优惠码已售罄";
                }
            }

            return null;
        }

        public string Verification(int promotionId)
        {
            return Verification(Get(promotionId));
        }
    }
}
