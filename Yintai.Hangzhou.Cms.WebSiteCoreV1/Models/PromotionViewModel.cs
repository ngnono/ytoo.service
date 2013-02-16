using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Yintai.Architecture.Common.Models;

namespace Yintai.Hangzhou.Cms.WebSiteCoreV1.Models
{
    public class PromotionCollectionViewModel : PagerInfo, IViewModel
    {
        public PromotionCollectionViewModel(PagerRequest request)
            : base(request)
        {
        }

        public PromotionCollectionViewModel(PagerRequest request, int totalCount)
            : base(request, totalCount)
        {
        }

        public List<PromotionViewModel> Promotions { get; set; }
    }

    public class PromotionViewModel : BaseViewModel
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
        [DataType(DataType.DateTime)]
        [Display(Name = "活动开始日期")]
        public System.DateTime StartDate { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        [Display(Name = "活动结束日期")]
        public System.DateTime EndDate { get; set; }

        [Range(0, Int32.MaxValue)]
        [Display(Name = "推荐来源")]
        public int RecommendSourceId { get; set; }

        [Range(0, Int32.MaxValue)]
        [Display(Name = "推荐来源类型")]
        public int RecommendSourceType { get; set; }

        [Range(0, Int32.MaxValue)]
        [Display(Name = "推荐人")]
        public int RecommendUser { get; set; }

        [Range(0, Int32.MaxValue)]
        [Display(Name = "喜欢数")]
        public int LikeCount { get; set; }

        [Range(0, Int32.MaxValue)]
        [Display(Name = "收藏数")]
        public int FavoriteCount { get; set; }

        [Range(0, Int32.MaxValue)]
        [Display(Name = "分享数")]
        public int ShareCount { get; set; }

        [Range(0, Int32.MaxValue)]
        [Display(Name = "参与（领取优惠券）数")]
        public int InvolvedCount { get; set; }

        [DisplayName("店铺")]
        [UIHint("Association")]
        [AdditionalMetadata("controller", "store")]
        [AdditionalMetadata("displayfield", "Name")]
        [AdditionalMetadata("searchfield", "name")]
        [AdditionalMetadata("valuefield", "Id")]
        [Range(0, Int32.MaxValue)]
        [Display(Name = "店铺")]
        public int Store_Id { get; set; }


        [Range(0, Int32.MaxValue)]
        [Display(Name = "Tag")]
        public int Tag_Id { get; set; }

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

        [Display(Name = "置顶")]
        public bool IsTop { get; set; }

        [Display(Name = "是否有商品")]
        public Nullable<bool> IsProdBindable { get; set; }

        [Display(Name = "发行量（不填或-1视为不限制）")]
        public Nullable<int> PublicationLimit { get; set; }

        public List<ResourceViewModel> Resources { get; set; }
    }
}
