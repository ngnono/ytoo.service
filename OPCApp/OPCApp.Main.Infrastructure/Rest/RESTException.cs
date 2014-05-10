using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace OPCApp.Infrastructure.REST
{
    public class RestException : Exception
    {
        public RestException(string message)
            :base(message)
        { }

        protected RestException(SerializationInfo info, StreamingContext context)
            :base(info, context)
        { }

        public RestException(string message, Exception innerException)
            :base(message, innerException)
        { }
    }
}
