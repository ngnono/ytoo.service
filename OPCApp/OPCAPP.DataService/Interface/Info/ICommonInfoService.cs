﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OPCApp.Domain.Models;

namespace OPCApp.DataService.Interface.Trans
{
    public interface ICommonInfo
  {
       List<ShipVia> GetShipViaList();
  }
}
