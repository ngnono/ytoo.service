using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Yintai.Architecture.Common.Models;

namespace Yintai.Hangzhou.Cms.WebSiteCoreV1.Models
{
    public class StoreViewModel : BaseViewModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(16, MinimumLength = 1)]
        [Display(Name = "名称")]
        public string Name { get; set; }

        [StringLength(256, MinimumLength = 0)]
        [Display(Name = "说明")]
        public string Description { get; set; }

        [Required]
        [StringLength(256, MinimumLength = 1)]
        [Display(Name = "地址")]
        public string Location { get; set; }
        [Required]
        [RegularExpression(@"(^(\d{3,4}-)?\d{7,8})$|(1[0-9]{9})")]
        [Display(Name = "电话")]
        public string Tel { get; set; }

        [Range(typeof(decimal), "-180.000000", "180.000000")]
        [Display(Name = "经度")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:n6}")]
        public decimal Longitude { get; set; }

        [Display(Name = "纬度")]
        [Range(typeof(decimal), "-90.000000", "90.000000")]
        [UIHint("Decimal")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:n6}")]
        public decimal Latitude { get; set; }

        [Display(Name = "集团Id")]
        [Range(0, Int32.MaxValue)]
        public int Group_Id { get; set; }

        [Display(Name = "地区Id")]
        [Range(0, Int32.MaxValue)]
        public int Region_Id { get; set; }

        [Display(Name = "店铺Level")]
        [Range(0, Int32.MaxValue)]
        public int StoreLevel { get; set; }


        [Display(Name = "状态")]
        public int Status { get; set; }
        [Display(Name = "创建人")]
        public int CreatedUser { get; set; }
        [Display(Name = "创建日期")]
        [DataType(DataType.DateTime)]
        public System.DateTime CreatedDate { get; set; }
        [Display(Name = "修改日期")]
        [DataType(DataType.DateTime)]
        public System.DateTime UpdatedDate { get; set; }
        [Display(Name = "修改人")]
        public int UpdatedUser { get; set; }
    }

    public class StoreCollectionViewModel : PagerInfo, IViewModel
    {
        public StoreCollectionViewModel(PagerRequest request)
            : base(request)
        {
        }

        public StoreCollectionViewModel(PagerRequest request, int totalCount)
            : base(request, totalCount)
        {
        }

        public List<StoreViewModel> Stores { get; set; }
    }
}
