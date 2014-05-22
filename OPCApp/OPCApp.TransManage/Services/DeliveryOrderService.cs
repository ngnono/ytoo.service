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
using Intime.OPC.Modules.Logistics.Models;

namespace Intime.OPC.Modules.Logistics.Services
{
    [Export(typeof(IDeliveryOrderService))]
    public class DeliveryOrderService : ServiceBase<OPC_ShippingSale>, IDeliveryOrderService
    {
        private Lazy<IService<OPC_Sale>> _salesOrderService;

        [ImportingConstructor]
        public DeliveryOrderService(Lazy<IService<OPC_Sale>> salesOrderService)
        {
            _salesOrderService = salesOrderService;
        }

        public override IList<OPC_ShippingSale> QueryAll(IQueryCriteria queryCriteria)
        {
            var deliveryOrders = base.QueryAll(queryCriteria);
            deliveryOrders.ForEach(deliveryOrder => BuildDeliveryOrder(deliveryOrder));

            return deliveryOrders;
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

        public OPC_ShippingSale Create(DeliveryOrderCreationDTO deliveryOrderCreationDto)
        {
            var deliveryOrder = Create<DeliveryOrderCreationDTO>(deliveryOrderCreationDto);
            BuildDeliveryOrder(deliveryOrder);

            return deliveryOrder;
        }

        private void BuildDeliveryOrder(OPC_ShippingSale deliveryOrder)
        {
            if (deliveryOrder.SalesOrders == null)
            {
                var queryCriteria = new QuerySalesOrderByDeliveryOrderId() { DeliveryOrderId = deliveryOrder.Id };

                deliveryOrder.SalesOrders = _salesOrderService.Value.QueryAll(queryCriteria);
            }
        }
    }
}
