using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OPCAPP.Common.Interface
{
    public interface ILog
    {
        //test git
        string WriteLog(string OprUser, string ModuleName, DateTime OprDate, string Operation);

    }
}
