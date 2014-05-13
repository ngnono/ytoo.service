using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    }

    public class BrandFilter : PageFilter
    {
        public string NamePrefix { get; set; }

        public int? Status { get; set; }

        /// <summary>
        /// 专柜
        /// </summary>
        public int? CounterId { get; set; }

        public string Name { get; set; }

        public BrandSortOrder? SortOrder { get; set; }
    }

    public class SupplierFilter : PageFilter
    {
        public string NamePrefix { get; set; }

        public int? Status { get; set; }

        public SupplierSortOrder? SortOrder { get; set; }
    }
}
