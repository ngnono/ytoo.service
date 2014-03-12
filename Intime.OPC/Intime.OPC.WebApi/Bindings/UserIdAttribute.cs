using System;
using System.Web.Http;

namespace Intime.OPC.WebApi.Bindings
{
    public class UserIdAttribute : ParameterBindingAttribute
    {
        public override System.Web.Http.Controllers.HttpParameterBinding GetBinding(System.Web.Http.Controllers.HttpParameterDescriptor parameter)
        {
            if (parameter.ParameterType == typeof(int) || parameter.ParameterType == typeof(Nullable<int>))
            {
                return new UserIdHttpParameterBinding(parameter);
            }

            return null;
        }
    }
}
