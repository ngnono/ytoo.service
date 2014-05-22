using Intime.OPC.Infrastructure.Service;
using OPCApp.Domain.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intime.OPC.Modules.Logistics.Criteria
{
    public class QuerySalesOrderByDeliveryOrderNo : QueryCriteria
    {
        [UriParameter("deliveryorderno")]
        public string DeliveryOrderNo { get; set; }
    }
}
