﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OPCApp.Common.Interface
{

    //
    public interface ILog
    {
        string WriteLog(string OprUser, string ModuleName, DateTime OprDate, string Operation);

    }
}
