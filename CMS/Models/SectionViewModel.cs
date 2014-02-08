using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Yintai.Hangzhou.Cms.WebSiteV1.Models
{
    public class SectionViewModel:BaseViewModel
    {
        [Display(Name = "专柜楼层地址")]
        public string Location { get; set; }
        [Display(Name = "联系电话")]
        public string ContactPhone { get; set; }
        [Display(Name = "状态")]
        public Nullable<int> Status { get; set; }
        [Required]
        [Display(Name = "专柜品牌")]
        [UIHint("Association")]
        [AdditionalMetadata("controller", "brand")]
        [AdditionalMetadata("displayfield", "Name")]
        [AdditionalMetadata("searchfield", "name")]
        [AdditionalMetadata("valuefield", "Id")]
        public Nullable<int> BrandId { get; set; }
        [Required]
        [Display(Name="专柜所属门店")]
        [UIHint("Association")]
        [AdditionalMetadata("controller", "store")]
        [AdditionalMetadata("displayfield", "Name")]
        [AdditionalMetadata("searchfield", "name")]
        [AdditionalMetadata("valuefield", "Id")]
        public int StoreId { get; set; }
        [Display(Name = "联系人")]
        [Required]
        public string ContactPerson { get; set; }
        [Display(Name="专柜名称")]
        [Required]
        public string Name { get; set; }
        [Display(Name="专柜编码")]
        public string StoreCode { get; set; }
        [Key]
        public int Id { get; set; }

        public StoreViewModel Store { get; set; }
        public BrandViewModel Brand { get; set; }

    }
    public class SectionSearchOption
    {
        [Display(Name = "专柜编码")]
        public int? SId { get; set; }
        [Display(Name = "专柜名称")]
        public string Name { get; set; }
        [Display(Name = "门店")]
        [UIHint("Association")]
        [AdditionalMetadata("controller", "store")]
        [AdditionalMetadata("displayfield", "Name")]
        [AdditionalMetadata("searchfield", "name")]
        [AdditionalMetadata("valuefield", "Id")]
        public int? StoreId { get; set; }
        [Display(Name = "品牌")]
        [UIHint("Association")]
        [AdditionalMetadata("controller", "brand")]
        [AdditionalMetadata("displayfield", "Name")]
        [AdditionalMetadata("searchfield", "name")]
        [AdditionalMetadata("valuefield", "Id")]
        public int? BrandId { get; set; }
    }
}