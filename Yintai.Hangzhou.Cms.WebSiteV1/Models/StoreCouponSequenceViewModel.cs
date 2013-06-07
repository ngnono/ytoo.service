using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Yintai.Hangzhou.Data.Models;
using Yintai.Hangzhou.Model.Enums;
using Yintai.Hangzhou.Cms.WebSiteV1.Util;

namespace Yintai.Hangzhou.Cms.WebSiteV1.Models
{
    public class StoreCouponSequenceViewModel : BaseViewModel
    {
        public string Code { get; set; }
        public string PromotionName { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public StoreCouponSequenceStatus Status { get {
            if (RawStatus == (int)CouponStatus.Deleted)
                return StoreCouponSequenceStatus.Void;
            if (RawStatus == (int)CouponStatus.Used)
                return StoreCouponSequenceStatus.Consumed;
            if (RawStatus == (int)CouponStatus.Normal && ValidEndDate >= DateTime.Now)
                return StoreCouponSequenceStatus.CreateNotConsume;
            if (RawStatus == (int)CouponStatus.Normal && ValidEndDate < DateTime.Now)
                return StoreCouponSequenceStatus.Expired;
            if (RawStatus == (int)CouponStatus.Normal && Logs != null
                && Logs.FirstOrDefault()!=null
                && Logs.First().ActionType == (int)CouponActionType.Rebate)
                return StoreCouponSequenceStatus.Rebated;
            return StoreCouponSequenceStatus.Expired;
        } }
        public string StoreName { get {
            if (Logs == null)
                return string.Empty;
            var log = Logs.FirstOrDefault();
            if (log == null)
                return string.Empty;
            return log.StoreName;
        } }
        public decimal Amount { get; set; }
        public int CustomerId { get; set; }
        public string ReceiptNo { get {
            if (Logs == null)
                return string.Empty;
            var log = Logs.FirstOrDefault();
            if (log == null)
                return string.Empty;
            return log.ReceiptNo;
        } }
        public IEnumerable<CouponLogViewModel> Logs { get; set; }
 
        public int RawStatus { get; set; }
        public DateTime ValidEndDate { get; set; }
        public string StatusR { get {
           return Status.ToFriendlyString();
        } }
    }
    public class StoreCouponSequenceOption
    {
        [Display(Name="兑换活动名")]
        public string PromotionName { get; set; }
        [Display(Name = "领券时间>")]
        public DateTime? CreateDateFrom { get; set; }
        [Display(Name = "领券时间<")]
        public DateTime? CreateDateTo { get; set; }
        [Display(Name = "使用时间>")]
        public DateTime? ConsumeDateFrom { get; set; }
        [Display(Name = "使用时间<")]
        public DateTime? ConsumeDateTo { get; set; }
        [Display(Name = "状态")]
        public StoreCouponSequenceStatus? Status { get; set; }
        [Display(Name = "线下门店")]
        [UIHint("Association")]
        [AdditionalMetadata("controller", "storereal")]
        [AdditionalMetadata("displayfield", "Name")]
        [AdditionalMetadata("searchfield", "name")]
        [AdditionalMetadata("valuefield", "StoreNo")]
        public string StoreNo { get; set; }
        [Display(Name = "积点大于")]
        public int? PointFrom { get; set; }
        [Display(Name = "积点小于")]
        public int? PointTo { get; set; }
        [Display(Name = "用户ID")]
        public int? CustomerId { get; set; }
        [Display(Name = "小票号")]
        public string ReceiptNo { get; set; }
        [Display(Name = "降序")]
        public StoreCouponSequenceSort? Sort { get; set; }

        
    }

    public class CouponLogViewModel : BaseViewModel
    {

        public string Code { get; set; }
        public System.DateTime CreateDate { get; set; }
        public int CreateUser { get; set; }
        public string ConsumeStoreNo { get; set; }
        public string ReceiptNo { get; set; }
        public string BrandNo { get; set; }
        public int ActionType { get; set; }
        public string StoreName { get; set; }
    }
    public enum StoreCouponSequenceStatus
    {
        [Description("领取未使用")]
        CreateNotConsume =CouponStatus.Normal,
        [Description("已消费使用")]
        Consumed = CouponStatus.Used,
        [Description("消费退回")]
        Rebated = 3,
        [Description("未消费退回")]
        Expired = CouponStatus.Expired,
        [Description("未消费取消")]
        Void = CouponStatus.Deleted
    }
    public enum StoreCouponSequenceSort
    {
        [Description("券状态")]
        ByStatus,
        [Description( "领券时间")]
        ByCreateDate,
        [Description("券金额")]
        ByAmount
    }

}