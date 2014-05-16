using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intime.OPC.Infrastructure.Properties;

namespace Intime.OPC.Infrastructure.Validation
{
    public class LocalizedMaxLengthAttribute : MaxLengthAttribute
    {
        public LocalizedMaxLengthAttribute(int length)
            :base(length)
        {
            ErrorMessageResourceType = typeof(Resources);
            ErrorMessageResourceName = this.GetType().Name.Replace("Attribute", string.Empty);
        }
    }
}
