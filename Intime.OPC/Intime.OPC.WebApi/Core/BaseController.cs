using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Results;
using Intime.OPC.Common.Logger;
using Intime.OPC.Domain.Exception;
using Intime.OPC.WebApi.Core.MessageHandlers.AccessToken;
using System.Web;

namespace Intime.OPC.WebApi.Core
{
    public class BaseController : ApiController
    {
        public int UserID
        {
            get
            {
                // 修改串用户的BUG,临时解决方案
                if (!HttpContext.Current.Items.Contains("__ACCESS_TOKEN__USERID__"))
                {
                    return -1;
                }

                int tmp = -1;

                int.TryParse(HttpContext.Current.Items["__ACCESS_TOKEN__USERID__"].ToString(),out tmp);

                return tmp;
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
        protected int GetCurrentUserID()
        {
            return UserID;
        }

        private int GetUserID(HttpControllerContext controllerContext)
        {
            if (controllerContext.Request.Properties.ContainsKey(AccessTokenConst.UseridPropertiesName))
            {
                string userid = controllerContext.Request.Properties[AccessTokenConst.UseridPropertiesName].ToString();
                int id = -1;
                bool bl = int.TryParse(userid, out id);
                if (bl)
                {
                    return id;
                }
                throw new UserIdConverException(userid);
            }
            throw new HttpResponseException(HttpStatusCode.Unauthorized);
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