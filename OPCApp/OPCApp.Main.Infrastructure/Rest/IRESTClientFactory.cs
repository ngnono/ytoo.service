using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OPCApp.Infrastructure.REST
{
    public interface IRestClientFactory
    {
        IRestClient Create(string baseAddress, string privateKey, string from);
    }
}
