using Intime.OPC.Infrastructure.Service;
using OPCApp.Domain.Models;
using OPCApp.Infrastructure.REST;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ploeh.AutoFixture;
using OPCApp.Domain.Enums;

namespace Intime.OPC.Modules.Logistics.Services
{
    //[Export(typeof(IService<OPC_Sale>))]
    public class SalesOrderService : ServiceBase<OPC_Sale>
    {
        private IService<Order> _orderService;
        private IService<OPC_ShippingSale> _deliveryOrderService;
        private IService<Counter> _counterService;

        [ImportingConstructor]
        public SalesOrderService(IService<Order> orderService, IService<OPC_ShippingSale> deliveryOrderService, IService<Counter> counterService)
        {
            _orderService = orderService;
            _deliveryOrderService = deliveryOrderService;
            _counterService = counterService;
        }

        public override PagedResult<OPC_Sale> Query(IQueryCriteria queryCriteria)
        {
            var result = base.Query(queryCriteria);
            if (result.TotalCount > 0)
            {
                result.Data.ForEach(salesOrder => BuildSalesOrder(salesOrder));
            }

            return result;
        }

        public override OPC_Sale Query(int id)
        {
            return Query(id.ToString());
        }

        public override OPC_Sale Query(string uniqueID)
        {
            var salesOrder = base.Query(uniqueID);
            BuildSalesOrder(salesOrder);

            return salesOrder;
        }

        private void BuildSalesOrder(OPC_Sale salesOrder)
        {
            if (salesOrder.Order == null) salesOrder.Order = _orderService.Query(salesOrder.OrderNo);
            if (salesOrder.Counter == null && salesOrder.SectionId.HasValue) salesOrder.Counter = _counterService.Query(salesOrder.SectionId.Value);
            if (salesOrder.DeliveryOrder == null) salesOrder.DeliveryOrder = _deliveryOrderService.Query(salesOrder.SaleOrderNo);
        }
    }
}
