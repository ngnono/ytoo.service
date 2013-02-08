using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Yintai.Architecture.Common;
using Yintai.Architecture.Common.Models;

namespace Yintai.Hangzhou.Cms.WebSiteCoreV1.Models
{
    public class ProductCollectionViewModel : PagerInfo, IViewModel
    {
        public ProductCollectionViewModel(PagerRequest request)
            : base(request)
        {
        }

        public ProductCollectionViewModel(PagerRequest request, int totalCount)
            : base(request, totalCount)
        {
        }

        public List<ProductViewModel> Products { get; set; }
    }

    public class ProductViewModel : BaseViewModel
    {
        [Key]
        [Display(Name = "Id")]
        public int Id { get; set; }

        [Required]
        [StringLength(16, MinimumLength = 1)]
        [Display(Name = "名称")]
        public string Name { get; set; }

        [DisplayName("品牌")]
        [UIHint("Association")]
        [AdditionalMetadata("controller", "brand")]
        [AdditionalMetadata("displayfield", "Name")]
        [AdditionalMetadata("searchfield", "name")]
        [AdditionalMetadata("valuefield", "Id")]
        [Range(0, Int32.MaxValue)]
        [Required]
        [Display(Name = "品牌Id")]
        public int Brand_Id { get; set; }

        [StringLength(140, MinimumLength = 0)]
        [Display(Name = "说明")]
        public string Description { get; set; }

        [RegularExpression(RegularDefine.Money, ErrorMessage = "只能输入金额,1~2位小数")]
        [Display(Name = "价格")]
        public decimal Price { get; set; }

        [StringLength(140, MinimumLength = 0)]
        [Display(Name = "推荐理由")]
        public string RecommendedReason { get; set; }

        [StringLength(140, MinimumLength = 0)]
        [Display(Name = "优惠信息")]
        public string Favorable { get; set; }


        [DisplayName("店铺")]
        [UIHint("Association")]
        [AdditionalMetadata("controller", "store")]
        [AdditionalMetadata("displayfield", "Name")]
        [AdditionalMetadata("searchfield", "name")]
        [AdditionalMetadata("valuefield", "Id")]
        [Required]
        [Range(0, Int32.MaxValue)]
        [Display(Name = "店铺Id")]
        public int Store_Id { get; set; }



        [DisplayName("tag名")]
        [UIHint("Association")]
        [AdditionalMetadata("controller", "tag")]
        [AdditionalMetadata("displayfield", "Name")]
        [AdditionalMetadata("searchfield", "name")]
        [AdditionalMetadata("valuefield", "Id")]
        [Range(0, Int32.MaxValue)]
        [Display(Name = "Tag_Id")]
        public int Tag_Id { get; set; }

        [Range(0, Int32.MaxValue)]
        [Display(Name = "收藏数")]
        public int FavoriteCount { get; set; }

        [Range(0, Int32.MaxValue)]
        [Display(Name = "分享数")]
        public int ShareCount { get; set; }

        [Range(0, Int32.MaxValue)]
        [Display(Name = "参与（领取优惠券）数")]
        public int InvolvedCount { get; set; }

        [Range(0, Int32.MaxValue)]
        [Display(Name = "推荐人")]
        public int RecommendUser { get; set; }

        [Range(0, Int32.MaxValue)]
        [Display(Name = "推荐来源")]
        public int RecommendSourceType { get; set; }

        [Range(0, Int32.MaxValue)]
        [Display(Name = "推荐来源Id")]
        public int RecommendSourceId { get; set; }

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

        [Display(Name = "资源")]
        public List<ResourceViewModel> Resources { get; set; }

        [Display(Name = "排序值")]
        [Range(0, Int32.MaxValue)]
        public int SortOrder { get; set; }
    }
}
