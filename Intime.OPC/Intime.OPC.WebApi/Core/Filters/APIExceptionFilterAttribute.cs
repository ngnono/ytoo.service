using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Web.Http.ModelBinding;

namespace Intime.OPC.WebApi.Core.Filters
{
    public class APIExceptionFilterAttribute : ExceptionFilterAttribute
    {
        private log4net.ILog log = log4net.LogManager.GetLogger(typeof(APIExceptionFilterAttribute));

        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            if (actionExecutedContext == null)
            {
                return;
            }

            log.Error(actionExecutedContext.ActionContext.ActionArguments);
            log.Error(actionExecutedContext.ActionContext.RequestContext);

            while (actionExecutedContext.Exception != null)
            {
                log.Error(actionExecutedContext.Exception);

                actionExecutedContext.Exception = actionExecutedContext.Exception.InnerException;
            }

            actionExecutedContext.Response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.InternalServerError,
                Content = new StringContent("系统内部 错误")
            };
        }




        public override Task OnExceptionAsync(HttpActionExecutedContext actionExecutedContext, CancellationToken cancellationToken)
        {

            return base.OnExceptionAsync(actionExecutedContext, cancellationToken);
            // return Task.FromResult<object>(null);
        }
    }
}
