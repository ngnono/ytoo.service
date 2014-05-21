using Intime.OPC.Infrastructure.Service;
using OPCApp.Domain.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ploeh.AutoFixture;
using OPCApp.Domain.Enums;
using OPCApp.Infrastructure.REST;
using Intime.OPC.Modules.Logistics.Criteria;
using Intime.OPC.Modules.Logistics.Enums;

namespace Intime.OPC.Modules.Logistics.Services
{
    //[Export(typeof(IDeliveryOrderService))]
    public class DeliveryOrderService : ServiceBase<OPC_ShippingSale>, IDeliveryOrderService
    {
        private IService<OPC_Sale> _salesOrderService;

        [ImportingConstructor]
        public DeliveryOrderService(IService<OPC_Sale> salesOrderService)
        {
            _salesOrderService = salesOrderService;
        }

        public override PagedResult<OPC_ShippingSale> Query(IQueryCriteria queryCriteria)
        {
            var result = base.Query(queryCriteria);
            if (result.TotalCount > 0)
            {
                result.Data.ForEach(deliveryOrder => BuildDeliveryOrder(deliveryOrder));
            }

            return result;
        }

        private void BuildDeliveryOrder(OPC_ShippingSale deliveryOrder)
        {
            if (deliveryOrder.SalesOrders == null && !string.IsNullOrEmpty(deliveryOrder.GoodsOutCode))
            {
                IQueryCriteria queryCriteria = new QuerySalesOrderByDeliveryOrderNo() { DeliveryOrderNo = deliveryOrder.GoodsOutCode };
                deliveryOrder.SalesOrders = _salesOrderService.QueryAll(queryCriteria);
            }
        }

        public void Print(OPC_ShippingSale deliveryOrder, ReceiptType receiptType)
        {
            string uri = string.Format("deliveryorder/{0}/print", deliveryOrder.Id);
            var data = new { Type = (int)receiptType };
            Update(uri, data);
        }

        public void CompleteHandOver(OPC_ShippingSale deliveryOrder)
        {
            string uri = string.Format("deliveryorder/{0}/finish", deliveryOrder.Id);
            Update(uri);
        }
    }
}
