using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OPCApp.Domain.Attributes
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
