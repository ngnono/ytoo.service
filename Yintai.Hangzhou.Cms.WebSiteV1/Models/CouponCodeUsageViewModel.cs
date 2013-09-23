using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Yintai.Hangzhou.Model.Enums;
using Yintai.Hangzhou.Cms.WebSiteV1.Util;

namespace Yintai.Hangzhou.Cms.WebSiteV1.Models
{
    public class CouponCodeUsageViewModel:BaseViewModel
    {
        public string Code { get; set; }
        public string PromotionName { get; set; }
        public string PromotionDate { get; set; }
        public DateTime CreateDate { get; set; }
        public string CustomerNick { get; set; }
        public string CustomerPhone { get; set; }
        public string ReceiptNo
        {
            get
            {
                if (Logs == null)
                    return string.Empty;
                var log = Logs.FirstOrDefault();
                if (log == null)
                    return string.Empty;
                return log.ReceiptNo;
            }
        }
        public string StoreName
        {
            get {
                if (Logs == null)
                    return string.Empty;
                var log = Logs.FirstOrDefault();
                if (log == null)
                    return string.Empty;
                return log.StoreName;
            }
        }
        public string StatusName
        {
            get {
                return ((CouponStatus)Status).ToFriendlyString();
            }
        }
        public int Status { get; set; }
        public IEnumerable<CouponLogViewModel> Logs { get; set; }

       
    }
    public class CouponUsageOption
    {
        [Display(Name = "兑换活动代码")]
        public int? PromotionId { get; set; }
        [Display(Name = "领券时间>")]
        public DateTime? CreateDateFrom { get; set; }
        [Display(Name = "领券时间<")]
        public DateTime? CreateDateTo { get; set; }
        [Display(Name="状态")]
        public int? Status { get; set; }

    }
}