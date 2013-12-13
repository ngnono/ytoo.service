using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Yintai.Hangzhou.Model.Enums;
using Yintai.Hangzhou.Cms.WebSiteV1.Util;

namespace Yintai.Hangzhou.Cms.WebSiteV1.Models
{
    public class StoreCouponUsageOption
    {
        [Required(ErrorMessage="请输入电子礼券码")]
        [Display(Name = "电子礼券码")]
        public string Code { get; set; }
    }
    public class StoreCouponUsageViewModel:BaseViewModel
    {
        public int Id { get; set; }
        public int Type { get; set; }
        [Display(Name="电子券码")]
        public string Code { get; set; }
        [Display(Name="日期")]
        public System.DateTime CreateDate { get; set; }
        public int CreateUser { get; set; }
        [Display(Name="门店")]
        public string ConsumeStoreNo { get; set; }
        [Display(Name = "小票号")]
        public string ReceiptNo { get; set; }
        public string BrandNo { get; set; }
        [Display(Name = "操作")]
        public int ActionType { get; set; }
        public string StoreNo { get {
            return ConsumeStoreNo;
        } }
        public string StoreName { get; set; }
        public string Operation { get {
            return ((CouponActionType)ActionType).ToFriendlyString();
        } }
    }
}