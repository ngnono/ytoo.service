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
        public int UserId
        {
            get
            {
                int uid;
                var userId = this.Request.Properties[AccessTokenConst.UseridPropertiesName].ToString();
                if (int.TryParse(userId, out uid))
                {
                    return uid;
                }
                return -1;
            }
        }

        /// <summary>
        ///     Gets the log.
        /// </summary>
        /// <returns>ILog.</returns>
        protected ILog GetLog()
        {
            return LoggerManager.Current();
        }

        /// <summary>
        ///     获得当前用户
        /// </summary>
        /// <returns>System.Nullable{System.Int32}.</returns>
        /// <exception cref="Intime.OPC.Domain.Exception.UserIdConverException"></exception>
        protected int GetCurrentUserId()
        {
            return UserId;
        }

        protected IHttpActionResult DoFunction(Func<dynamic> action, string falseMeg = "")
        {
            try
            {
                var o = action();
                return Ok(o);
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