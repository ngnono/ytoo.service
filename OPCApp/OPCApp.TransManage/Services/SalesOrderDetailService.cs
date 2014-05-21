using Intime.OPC.Infrastructure.Service;
using OPCApp.Domain.Models;
using OPCApp.Infrastructure.REST;
using Ploeh.AutoFixture;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intime.OPC.Modules.Logistics.Services
{
    //[Export(typeof(IService<OPC_SaleDetail>))]
    public class SalesOrderDetailService : ServiceBase<OPC_SaleDetail>
    {

    }
}
