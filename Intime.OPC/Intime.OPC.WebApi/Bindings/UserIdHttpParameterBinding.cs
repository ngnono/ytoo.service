using Intime.OPC.WebApi.Core.MessageHandlers.AccessToken;

using System.Threading.Tasks;
using System.Web.Http.Controllers;

namespace Intime.OPC.WebApi.Bindings
{
    public class UserIdHttpParameterBinding : HttpParameterBinding
    {
        public UserIdHttpParameterBinding(HttpParameterDescriptor descriptor)
            : base(descriptor)
        {
        }
        public override Task ExecuteBindingAsync(System.Web.Http.Metadata.ModelMetadataProvider metadataProvider, HttpActionContext actionContext, System.Threading.CancellationToken cancellationToken)
        {
            if (actionContext.Request.Properties.ContainsKey(AccessTokenConst.USERID_PROPERTIES_NAME))
            {
                actionContext.ActionArguments[Descriptor.ParameterName] = actionContext.Request.Properties[AccessTokenConst.USERID_PROPERTIES_NAME];
            }
            else
            {
                actionContext.ActionArguments[Descriptor.ParameterName] = 0;
            }

            var tsc = new TaskCompletionSource<object>();
            tsc.SetResult(null);
            return tsc.Task;
        }
    }
}
