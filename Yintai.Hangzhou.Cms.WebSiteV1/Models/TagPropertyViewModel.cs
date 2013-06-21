using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Yintai.Hangzhou.Cms.WebSiteV1.Models
{
    public class TagPropertyViewModel : BaseViewModel,IValidatableObject
    {
        [Display(Name = "分类")]
        [Required]
        [UIHint("Association")]
        [AdditionalMetadata("controller", "tag")]
        [AdditionalMetadata("displayfield", "Name")]
        [AdditionalMetadata("searchfield", "name")]
        [AdditionalMetadata("valuefield", "Id")]
        public int CategoryId { get; set; }
        [Display(Name="设置属性")]
        public List<TagPropertyValueViewModel> Values { get; set; }
        public string CategoryName { get; set; }


        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Values == null || Values.Count <= 0)
            {
                yield return new ValidationResult("需要至少设置一个属性");
            }
        }
    }
    public class TagPropertyValueViewModel : BaseViewModel
    {
        [Display(Name = "属性名")]
        [Required]
        [StringLength(20,MinimumLength=1)]
        public string PropertyDesc { get; set; }
        [Display(Name = "属性值")]
        [Required]
        [StringLength(100, MinimumLength = 1)]
        public string ValueDesc { get; set; }
        [Display(Name = "排序值")]
        public int SortOrder { get; set; }
        [Display(Name = "状态")]
        public int Status { get; set; }
        [Display(Name = "属性代码")]
        public int PropertyId { get; set; }
        [Display(Name = "属性值代码")]
        public int ValueId { get; set; }


    }
    public class TagPropertyValueSearchOption
    {
        [Display(Name="属性代码")]
        public int? PId { get; set; }
         [Display(Name = "属性名")]
        public string PropertyDesc {get;set;}
         [Display(Name = "分类代码")]
         [UIHint("Association")]
         [AdditionalMetadata("controller", "tag")]
         [AdditionalMetadata("displayfield", "Name")]
         [AdditionalMetadata("searchfield", "name")]
         [AdditionalMetadata("valuefield", "Id")]
        public int? CategoryId { get; set; }
         [Display(Name = "属性值")]
        public string ValueDesc { get; set; }
    }
}