using Intime.OPC.Common.Logger;
using Intime.OPC.WebApi.Core.MessageHandlers.AccessToken;
using System;
using System.Net;
using System.Web.Http;
using System.Web.Http.Results;

namespace Intime.OPC.WebApi.Core
{
    public class BaseController : ApiController
    {
       /// <summary>
        ///     Gets the log.
        /// </summary>
        /// <returns>ILog.</returns>
        protected ILog GetLog()
        {
            return LoggerManager.Current();
        }

        protected IHttpActionResult DoFunction(Func<dynamic> action, string errorMessage = "")
        {
            try
            {
                return Ok(action());
            }
            catch (HttpResponseException ex)
            {
                GetLog().Error(ex);
                return new StatusCodeResult(HttpStatusCode.Unauthorized, this);
            }
            catch (Exception ex)
            {
                GetLog().Error(ex);
                return BadRequest(ex.Message);
            }
        }

        protected IHttpActionResult DoAction(Action action, string falseMeg = "")
        {
            try
            {
                action();
                return Ok();
            }
            catch (HttpResponseException ex)
            {
                GetLog().Error(ex);
                return new StatusCodeResult(HttpStatusCode.Unauthorized, this);
            }
            catch (Exception ex)
            {
                GetLog().Error(ex);
                return InternalServerError();
            }
        }
    }
}