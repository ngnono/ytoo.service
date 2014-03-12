using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Yintai.Hangzhou.Model.Enums;

namespace Yintai.Hangzhou.Cms.WebSiteCoreV1.Models
{
    public class StorePromotionViewModel : BaseViewModel,IValidatableObject
    {
        [Display(Name = "编码")]
        [Key]
        public int Id { get; set; }
        [Display(Name = "名称")]
        [Required]
        public string Name { get; set; }
        [Display(Name = "描述")]
        public string Description { get; set; }
        [Display(Name = "兑换开始时间")]
        [Required]
        public System.DateTime ActiveStartDate { get; set; }
        [Display(Name = "兑换结束时间")]
        [Required]
        public System.DateTime ActiveEndDate { get; set; }
        [Display(Name = "活动类型")]
        [Required]
        [Range(1,100)]
        public int PromotionType { get; set; }
        [Display(Name = "兑换积点方式")]
        [Range(1, 100)]
        public int AcceptPointType { get; set; }
        [Display(Name = "注意事项")]
        public string Notice { get; set; }
        [Display(Name = "券开始时间")]
        public Nullable<System.DateTime> CouponStartDate { get; set; }
        [Display(Name = "券结束时间")]
        public Nullable<System.DateTime> CouponEndDate { get; set; }
        [Display(Name = "积点最低起点")]
        public Nullable<int> MinPoints { get; set; }
        [Display(Name = "状态")]
        public Nullable<int> Status { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public Nullable<int> CreateUser { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
        public Nullable<int> UpdateUser { get; set; }
        [Display(Name = "使用须知")]
        public string UsageNotice { get; set; }
        [Display(Name = "使用范围")]
        public string InScopeNotice { get; set; }

        [Display(Name = "使用范围")]
        public IEnumerable<StorePromotionScopeViewModel> Scope { get; set; }
        [Display(Name = "使用规则")]
        public IEnumerable<StorePromotionRuleViewModel> Rules { get; set; }

        [Display(Name="积分兑换单位")]
        public Nullable<int> UnitPerPoints { get; set; }

        public string ComposedScopeNotice
        {
            get
            { 
                var composed = string.Empty;
                if (Scope ==null || Scope.Count()<1)
                 return composed;
               return JsonConvert.SerializeObject(Scope.Where(s=>s.Status!=(int)DataStatus.Deleted).Select(s=>new {
                     storeid = s.StoreId,
                     storename = s.StoreName,
                     excludes = s.Excludes
                }),Formatting.None);
            }
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (ActiveStartDate >= ActiveEndDate)
                yield return new ValidationResult("兑换开始时间小于结束时间");
            if (CouponStartDate >= CouponEndDate)
                yield return new ValidationResult("代金券开始时间小于结束时间");
            if (Scope == null || Scope.Count() < 1)
                yield return new ValidationResult("使用范围没有设置");
            if (Rules == null || Rules.Count() < 1)
                yield return new ValidationResult("使用规则没有设置");
        }
    }
    public class StorePromotionScopeViewModel : BaseViewModel
    {
        public int Id { get; set; }
        [Display(Name = "使用门店代码")]
        [UIHint("Association")]
        [AdditionalMetadata("controller", "store")]
        [AdditionalMetadata("valuefield", "Id")]
        [AdditionalMetadata("iscascad","true")]
        [Required]
        public Nullable<int> StoreId { get; set; }
        [Display(Name = "使用门店")]
        [Required]
        public string StoreName { get; set; }
        [Display(Name = "不参加品类描述")]
        public string Excludes { get; set; }
        public Nullable<int> Status { get; set; }
    }
     public class StorePromotionRuleViewModel:BaseViewModel,IValidatableObject
    {
        [Display(Name="规则代码")]
        public int Id { get; set; }
        public int StorePromotionId { get; set; }
        [Display(Name = "积点大于等于：")]
        public Nullable<int> RangeFrom { get; set; }
        [Display(Name = "积点小于：")]
        public Nullable<int> RangeTo { get; set; }
        [Display(Name = "兑换比例(100积点)")]
        [Range(0.1,10)]
        public decimal Ratio { get; set; }
          [Display(Name = "状态")]
        public Nullable<int> Status { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
        public Nullable<int> UpdateUser { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!RangeFrom.HasValue && !RangeTo.HasValue)
                yield return new ValidationResult("至少需要设置一个积点");
            if (RangeFrom.HasValue && RangeTo.HasValue && RangeFrom >= RangeTo)
                yield return new ValidationResult("积点设置错误");

        }
    }
   
}
