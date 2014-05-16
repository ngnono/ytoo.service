using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intime.OPC.Infrastructure.Service
{
    public class UriParameterAttribute : Attribute
    {
        public UriParameterAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
    }
}
