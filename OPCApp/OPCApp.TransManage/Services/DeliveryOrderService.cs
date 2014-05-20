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

namespace Intime.OPC.Modules.Logistics.Services
{
    [Export(typeof(IService<OPC_ShippingSale>))]
    public class DeliveryOrderService : ServiceBase<OPC_ShippingSale>
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
    }
}
