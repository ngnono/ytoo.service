using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

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
       
    }
    public class CouponUsageOption
    {
        [Display(Name = "兑换活动代码")]
        public int? PromotionId { get; set; }
        [Display(Name = "领券时间>")]
        public DateTime? CreateDateFrom { get; set; }
        [Display(Name = "领券时间<")]
        public DateTime? CreateDateTo { get; set; }
       


    }
}