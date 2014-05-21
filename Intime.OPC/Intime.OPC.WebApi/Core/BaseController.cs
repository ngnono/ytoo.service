using Intime.OPC.Common.Logger;
using Intime.OPC.WebApi.Core.MessageHandlers.AccessToken;
using System;
using System.Web.Http;

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

        public TextResult Error(string message)
        {
            return new TextResult(message,this.Request);
        }

        public int UserId
        {
            get
            {
                int uid;
                if (int.TryParse(this.Request.Properties[AccessTokenConst.UseridPropertiesName].ToString(), out uid))
                {
                    return uid;
                }
                throw new UnauthorizedAccessException();
            }
            set
            {
                throw new InvalidOperationException("不允许的操作");
            }
        }

        protected IHttpActionResult DoFunction(Func<dynamic> action, string errorMessage = "")
        {
            try
            {
                return Ok(action());
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
            catch (Exception ex)
            {
                GetLog().Error(ex);
                return InternalServerError();
            }
        }


        protected IHttpActionResult RetrunHttpActionResult<T>(T dto)
        {
            if (dto != null)
            {
                return Ok(dto);
            }

            return NotFound();
        }

        public IHttpActionResult ResultNotFound()
        {
            return NotFound();
        }
    }
}