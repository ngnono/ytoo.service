using System.Collections.Generic;

namespace OPCApp.Infrastructure.DataService
{
    public interface IFilter
    {
        IDictionary<string, object> GetFilter();
    }
}