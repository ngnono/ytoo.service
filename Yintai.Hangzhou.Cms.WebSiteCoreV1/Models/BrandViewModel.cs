using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Yintai.Architecture.Common.Models;

namespace Yintai.Hangzhou.Cms.WebSiteCoreV1.Models
{
    public class BrandCollectionViewModel : PagerInfo, IViewModel
    {
        public BrandCollectionViewModel(PagerRequest request)
            : base(request)
        {
        }

        public BrandCollectionViewModel(PagerRequest request, int totalCount)
            : base(request, totalCount)
        {
        }

        public List<BrandViewModel> Brands { get; set; }
    }

    public class BrandViewModel : BaseViewModel
    {
        [Key]
        [Display(Name="品牌代码")]
        public int Id { get; set; }

        [Required]
        [StringLength(16, MinimumLength = 1)]
        [Display(Name = "名称")]
        public string Name { get; set; }

        [StringLength(16, MinimumLength = 0)]
        [Display(Name = "副名称")]
        public string EnglishName { get; set; }

        [StringLength(256, MinimumLength = 0)]
        [Display(Name = "说明")]
        public string Description { get; set; }

        [StringLength(1024, MinimumLength = 0)]
        public string Logo { get; set; }

        [StringLength(1024, MinimumLength = 0)]
        [Display(Name="网站")]
        public string WebSite { get; set; }

        [Required]
        [Display(Name = "列表索引")]
        [StringLength(1, MinimumLength = 1)]
        public string Group { get; set; }

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
}
