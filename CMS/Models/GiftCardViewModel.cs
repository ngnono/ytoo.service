using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Yintai.Architecture.Common.Models;
using Yintai.Hangzhou.Data.Models;

namespace Yintai.Hangzhou.Cms.WebSiteV1.Models
{
    public class GiftCardModel : PagerInfo, IViewModel
    {
        public GiftCardModel(PagerRequest request)
            : base(request)
        {
        }

        public GiftCardModel(PagerRequest request, int totalCount)
            : base(request, totalCount)
        {
        }

        public List<IMS_GiftCardEntity> CardList { get; set; }
    }

    public class GiftCardEntityModel
    {
        [Required]
        [StringLength(500, MinimumLength = 3)]
        [Display(Name = "名称")]
        public string Name { get; set; }

        [Display(Name = "是否上线")]
        public bool IsPublished { get; set; }

        public IEnumerable<IMS_GiftCardItemEntity> GiftCardItems { get; set; } 

    }

    public class GiftCardItemViewModel
    {
        public IMS_GiftCardEntity Card { get; set; }

        public IEnumerable<IMS_GiftCardItemEntity> CardItems { get; set; }
    }
}