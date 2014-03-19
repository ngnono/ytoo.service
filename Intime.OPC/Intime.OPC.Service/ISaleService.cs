﻿using Intime.OPC.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intime.OPC.Service
{
    public interface ISaleService
    {
        IList<OPC_Sale> Select();
        bool UpdateSatus(OPC_Sale sale);

    }
}