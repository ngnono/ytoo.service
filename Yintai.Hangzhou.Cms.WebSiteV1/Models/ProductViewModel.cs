using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Yintai.Architecture.Common;
using Yintai.Architecture.Common.Models;
using Yintai.Hangzhou.Model.Enums;
using Yintai.Hangzhou.Model.Filters;
using System.Linq;
using Yintai.Hangzhou.Repository.Contract;
using Yintai.Architecture.Framework.ServiceLocation;
using Yintai.Hangzhou.Service.Contract;
using System.Web;

namespace Yintai.Hangzhou.Cms.WebSiteV1.Models
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

    public class ProductViewModel : BaseViewModel,IValidatableObject
    {
        [Key]
        [Display(Name = "商品代码")]
        public int Id { get; set; }

        [Required]
        [StringLength(16, MinimumLength = 1)]
        [Display(Name = "名称")]
        public string Name { get; set; }

        [UIHint("Association")]
        [AdditionalMetadata("controller", "brand")]
        [AdditionalMetadata("displayfield", "Name")]
        [AdditionalMetadata("searchfield", "name")]
        [AdditionalMetadata("valuefield", "Id")]
        [Range(0, Int32.MaxValue)]
        [Required]
        [Display(Name = "品牌代码")]
        public int Brand_Id { get; set; }

        [Display(Name = "品牌")]
        public string BrandName { get; set; }

        [StringLength(140, MinimumLength = 0)]
        [Display(Name = "商品描述")]
        public string Description { get; set; }

        [RegularExpression(RegularDefine.Money, ErrorMessage = "只能输入金额,1~2位小数")]
        [Display(Name = "价格")]
        public decimal Price { get; set; }

        [StringLength(140, MinimumLength = 1)]
        [Display(Name = "推荐理由")]
        public string RecommendedReason { get; set; }

        [StringLength(140, MinimumLength = 0)]
        [Display(Name = "优惠信息")]
        public string Favorable { get; set; }


        [UIHint("Association")]
        [AdditionalMetadata("controller", "store")]
        [AdditionalMetadata("displayfield", "Name")]
        [AdditionalMetadata("searchfield", "name")]
        [AdditionalMetadata("valuefield", "Id")]
        [Required]
        [Range(0, Int32.MaxValue)]
        [Display(Name = "店铺代码")]
        public int Store_Id { get; set; }

        [Display(Name = "店铺")]
        public string StoreName { get; set; }

        [UIHint("Association")]
        [AdditionalMetadata("controller", "tag")]
        [AdditionalMetadata("displayfield", "Name")]
        [AdditionalMetadata("searchfield", "name")]
        [AdditionalMetadata("valuefield", "Id")]
        [Range(0, Int32.MaxValue)]
        [Display(Name = "分类代码")]
        public int Tag_Id { get; set; }

        [Display(Name = "分类")]
        public string TagName { get; set; }

        [Range(0, Int32.MaxValue)]
        [Display(Name = "收藏数")]
        public int FavoriteCount { get; set; }

        [Range(0, Int32.MaxValue)]
        [Display(Name = "分享数")]
        public int ShareCount { get; set; }

        [Range(0, Int32.MaxValue)]
        [Display(Name = "参与（领取优惠券）数")]
        public int InvolvedCount { get; set; }

        [Required]
        [DisplayName("推荐人")]
        [UIHint("Association")]
        [AdditionalMetadata("controller", "Customer")]
        [AdditionalMetadata("displayfield", "Nickname")]
        [AdditionalMetadata("searchfield", "Nickname")]
        [AdditionalMetadata("valuefield", "Id")]
        [Range(0, Int32.MaxValue)]
        [Display(Name = "推荐人代码")]
        public int RecommendUser { get; set; }

        [Range(0, Int32.MaxValue)]
        [Display(Name = "推荐来源")]
        public int RecommendSourceType { get; set; }

        [Range(0, Int32.MaxValue)]
        [Display(Name = "推荐来源代码")]
        public int RecommendSourceId { get; set; }

        [Display(Name="专题代码(s)")]
        [UIHint("Association")]
        [AdditionalMetadata("controller", "specialtopic")]
        [AdditionalMetadata("displayfield", "Name")]
        [AdditionalMetadata("searchfield", "name")]
        [AdditionalMetadata("valuefield", "Id")]
        [AdditionalMetadata("multiple", "true")]
        public string TopicIds { get; set; }
        [Display(Name="专题名")]
        public IEnumerable<string> TopicName { get; set; }

        [Display(Name="活动代码(s)")]
        [UIHint("Association")]
        [AdditionalMetadata("controller", "promotion")]
        [AdditionalMetadata("displayfield", "Name")]
        [AdditionalMetadata("searchfield", "name")]
        [AdditionalMetadata("valuefield", "Id")]
        [AdditionalMetadata("multiple", "true")]
        public string PromotionIds { get; set; }
        [Display(Name="促销名")]
        public IEnumerable<string> PromotionName { get; set; }

        [Display(Name = "状态")]
        public int Status { get; set; }
        [Display(Name = "创建人编码")]
        public int CreatedUser { get; set; }
        [Display(Name = "创建人")]
        public string CreateUserName { get; set; }
        [Display(Name = "创建日期")]
        [DataType(DataType.DateTime)]
        public System.DateTime CreatedDate { get; set; }
        [Display(Name = "修改日期")]
        [DataType(DataType.DateTime)]
        public System.DateTime UpdatedDate { get; set; }
        [Display(Name = "修改人")]
        public int UpdatedUser { get; set; }

        [Display(Name = "资源")]
        public IEnumerable<ResourceViewModel> Resources { get; set; }

        [Display(Name = "优先级")]
        [Range(0, Int32.MaxValue)]
        public int SortOrder { get; set; }

        public IEnumerable<ResourceViewModel> Audios { get {
            if (Resources == null)
                return null;
            return Resources.Where(r => r.Type == (int)ResourceType.Sound);
        } }


        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            string errorUnAuthorizedDataAccess = "没有授权操作该门店和品牌！";
            var currentUser = ServiceLocator.Current.Resolve<IAuthenticationService>().CurrentUserFromHttpContext(HttpContext.Current);
            if (currentUser == null)
                yield return new ValidationResult(errorUnAuthorizedDataAccess);
            IUserAuthRepository authRepo = ServiceLocator.Current.Resolve<IUserAuthRepository>();
            if (currentUser.Role == (int)UserRole.Admin)
                yield break;
            if (!authRepo.Get(a=>a.UserId==currentUser.CustomerId)
                .Any(a=>a.StoreId==0 ||(a.StoreId== this.Store_Id &&
                         (a.BrandId==0 || a.BrandId == this.Brand_Id))))
                yield return new ValidationResult(errorUnAuthorizedDataAccess);
           
            
        }
    }
  
}
