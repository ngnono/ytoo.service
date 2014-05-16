﻿using Intime.OPC.Infrastructure.Service;
using Intime.OPC.Modules.Dimension.Common;
using Intime.OPC.Modules.Dimension.Models;
using System.ComponentModel.Composition;

namespace Intime.OPC.Modules.Dimension.Services
{
    [Export(typeof(IService<Brand>))]
    public class BrandService : DimensionService<Brand>
    {
        
    }
}
