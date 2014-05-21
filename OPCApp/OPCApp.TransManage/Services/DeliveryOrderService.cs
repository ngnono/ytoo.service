﻿using Intime.OPC.Infrastructure.Service;
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

namespace Intime.OPC.Modules.Logistics.Services
{
    //[Export(typeof(IService<OPC_ShippingSale>))]
    public class DeliveryOrderService : ServiceBase<OPC_ShippingSale>
    {
    }
}
