using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intime.OPC.Domain.BusinessModel
{
    public class SaleOrderFilter
    {
        /// <summary>
        /// 
        /// </summary>
        public int? StoreId { get;set; }

        public string SaleOrderNo { get; set; }

        public string OrderNo { get; set; }

        public DateRangeFilter DateRange { get; set; }

        public int? Status { get; set; }
    }
}
