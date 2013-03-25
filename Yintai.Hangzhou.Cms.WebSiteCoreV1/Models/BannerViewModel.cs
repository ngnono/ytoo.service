using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Yintai.Architecture.Common.Models;

namespace Yintai.Hangzhou.Cms.WebSiteCoreV1.Models
{
    public class BannerViewModel
    {
        [Display(Name="广告代码")]
        public int Id { get; set; }
        [Display(Name = "链接代码")]
        [Required]
        [UIHint("Association")]
        [AdditionalMetadata("controller", "promotion")]
        [AdditionalMetadata("displayfield", "Name")]
        [AdditionalMetadata("searchfield", "name")]
        [AdditionalMetadata("valuefield", "Id")]
        public int SourceId { get; set; }
        [Required]
        [Display(Name = "链接类型")]
        public int SourceType { get; set; }
        [Display(Name = "排序")]
        public int SortOrder { get; set; }
        [Display(Name = "状态")]
        public int Status { get; set; }

        public PromotionViewModel Promotion { get; set; }
        public ResourceViewModel Resource { get; set; }
    }
    public class BannerCollectionViewModel : PagerInfo
    {
        public BannerCollectionViewModel(PagerRequest request)
            : base(request)
        {
        }

        public BannerCollectionViewModel(PagerRequest request, int totalCount)
            : base(request, totalCount)
        {
        }

        public IEnumerable<BannerViewModel> Banners { get; set; }
    }
}
