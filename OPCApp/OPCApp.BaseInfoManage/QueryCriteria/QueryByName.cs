using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Intime.OPC.Modules.Dimension.Common;

namespace Intime.OPC.Modules.Dimension
{
    public class QueryByName : QueryCriteria
    {
        [UriParameter("nameprefix")]
        public string Name { get; set; }
    }
}
