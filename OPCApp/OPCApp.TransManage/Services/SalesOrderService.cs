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
    }
}
