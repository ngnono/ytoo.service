using Intime.OPC.Domain.Enums.SortOrder;

namespace Intime.OPC.Domain.BusinessModel
{
    public class SectionFilter : PageFilter
    {
        public string NamePrefix { get; set; }

        public SectionSortOrder? SortOrder { get; set; }

        public int? Status { get; set; }

        public string Name { get; set; }

        public int? BrandId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int? AuthUserId { get; set; }

        public int? StoreId { get; set; }
    }
}