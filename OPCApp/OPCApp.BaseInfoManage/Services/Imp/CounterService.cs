using Intime.OPC.Modules.Dimension.Common;
using Intime.OPC.Modules.Dimension.Models;
using OPCApp.Infrastructure;
using OPCApp.Infrastructure.REST;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace Intime.OPC.Modules.Dimension.Services.Imp
{
    //[Export(typeof(ICounterService))]
    public class CounterService : DimensionService<Counter>
    {

    }
}
