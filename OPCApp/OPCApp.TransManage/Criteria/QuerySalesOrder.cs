using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intime.OPC.Infrastructure.Service;

namespace Intime.OPC.Modules.Logistics.Criteria
{
    public class QuerySalesOrder : QueryCriteria
    {
        public QuerySalesOrder()
        {
            StartDate = DateTime.Now;
            EndDate = DateTime.Now;
        }

        [UriParameter("startdate")]
        public DateTime StartDate { get; set; }

        [UriParameter("enddate")]
        public DateTime EndDate { get; set; }

        [UriParameter("orderno")]
        public string OrderNo { get; set; }

        [UriParameter("saleorderno")]
        public string SalesOrderNo { get; set; }
    }
}
