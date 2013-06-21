using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Yintai.Hangzhou.Cms.WebSiteV1.Models
{
    public class ProductPropertyViewModel:BaseViewModel,IValidatableObject
    {
        [Display(Name = "商品代码")]
        [Required]
        public int ProductId { get; set; }
        [Display(Name = "设置属性")]
        public List<TagPropertyValueViewModel> Values { get; set; }
        public string CategoryName { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Values == null || Values.Count <= 0)
            {
                yield return new ValidationResult("需要至少设置一个属性");
            }
        }
        [Display(Name = "商品名")]
        public string ProductName { get; set; }
        public TagPropertyViewModel TagProperty { get; set; } 
    }
    public class ProductPropertyValueSearchOption
    {
        [Display(Name = "属性名")]
        public string PropertyDesc { get; set; }
        [Display(Name = "商品代码")]
        public int? ProductId { get; set; }
        [Display(Name = "属性值")]
        public string ValueDesc { get; set; }
    }
}