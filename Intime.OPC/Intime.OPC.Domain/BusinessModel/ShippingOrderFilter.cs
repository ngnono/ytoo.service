using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intime.OPC.Domain.Enums;

namespace Intime.OPC.Domain.BusinessModel
{
    public class ShippingOrderFilter
    {
        public EnumSaleOrderStatus? Status { get; set; }
    }
}
