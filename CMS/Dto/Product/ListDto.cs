using Yintai.Hangzhou.Cms.WebSiteV1.Dto.Resource;
using Yintai.Hangzhou.Cms.WebSiteV1.Models;
using Yintai.Hangzhou.Model.Enums;

namespace Yintai.Hangzhou.Cms.WebSiteV1.Dto.Product
{
    public class SearchParamsRequest
    {
        public int? BrandId { get; set; }

        public int? PromotionId { get; set; }

        public int? RecommendUser { get; set; }

        public int? TopicId { get; set; }

        public int? TagId { get; set; }

        public string Name { get; set; }

        public int? Sort { get; set; }

        public int? Status { get; set; }


        public DataStatus? DataStatus
        {
            get { return Status == null ? new DataStatus?() : (DataStatus)Status; }
            set { Status = value == null ? null : (int?)value; }
        }

        public ProductSortOrder? SortOrder
        {
            get { return Sort == null ? new ProductSortOrder?() : (ProductSortOrder)Sort; }
            set { Sort = value == null ? null : (int?)value; }
        }
    }

    public class ListDto : DtoBase
    {
        public ListDto()
            : this(new SearchParamsRequest())
        {
        }

        public ListDto(SearchParamsRequest searchParams)
        {
            SearchParams = searchParams;
        }

        public SearchParamsRequest SearchParams { get; set; }

        public ProductCollectionViewModel Collection { get; set; }
    }
}
