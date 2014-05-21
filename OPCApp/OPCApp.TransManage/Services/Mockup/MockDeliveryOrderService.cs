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
using Intime.OPC.Modules.Logistics.Enums;

namespace Intime.OPC.Modules.Logistics.Services
{
    [Export(typeof(IDeliveryOrderService))]
    public class MockDeliveryOrderService : ServiceBase<OPC_ShippingSale>, IDeliveryOrderService
    {
        private Fixture fixture = new Fixture();

        public override OPC_ShippingSale Create(OPC_ShippingSale obj)
        {
            var salesOrders = fixture.Build<OPC_Sale>()
                .Without(so => so.DeliveryOrder)
                .Without(so => so.Counter)
                .Do(so => so.IsSelected = false)
                .Do(so => so.Status = EnumSaleOrderStatus.PrintInvoice)
                .CreateMany(3);

            var deliveryOrder = fixture.Build<OPC_ShippingSale>()
                .Without(delivery => delivery.SalesOrders)
                .Create();

            deliveryOrder.SalesOrders = salesOrders.ToList();

            return deliveryOrder;
        }

        public override OPC_ShippingSale Update(OPC_ShippingSale obj)
        {
            return obj;
        }

        public override void Update<TData>(OPC_ShippingSale obj, TData data)
        {
            
        }

        public override PagedResult<OPC_ShippingSale> Query(IQueryCriteria queryCriteria)
        {
            var salesOrders = fixture.Build<OPC_Sale>()
                .Without(so => so.DeliveryOrder)
                .Without(so => so.Counter)
                .Do(so => so.IsSelected = false)
                .Do(so => so.Status = EnumSaleOrderStatus.PrintInvoice)
                .CreateMany(3);

            var deliveryOrders = fixture.Build<OPC_ShippingSale>()
                .Without(delivery => delivery.SalesOrders)
                .Do(deliveryOrder => deliveryOrder.SalesOrders = salesOrders.ToList())
                .CreateMany(100);

            var result = new PagedResult<OPC_ShippingSale>() { PageIndex = queryCriteria.PageIndex, PageSize = queryCriteria.PageSize, TotalCount = 200, Data = deliveryOrders.ToList() };
            return result;
        }

        public override IList<OPC_ShippingSale> QueryAll(IQueryCriteria queryCriteria)
        {
            var result = Query(queryCriteria);
            return result.Data;
        }

        public void Print(OPC_ShippingSale deliveryOrder, ReceiptType receiptType)
        {

        }

        public void CompleteHandOver(OPC_ShippingSale deliveryOrder)
        {

        }
    }
}
