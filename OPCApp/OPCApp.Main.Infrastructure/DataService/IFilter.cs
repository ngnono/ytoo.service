using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OPCApp.Infrastructure.DataService
{
    public interface IFilter
    {
        IDictionary<string,object> GetFilter();
    }
}
