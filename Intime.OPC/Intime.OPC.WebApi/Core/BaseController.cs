using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Http;
using System.Web.Http.Results;
using Intime.OPC.Common.Logger;
using Intime.OPC.Domain.Exception;
using Intime.OPC.WebApi.Core.MessageHandlers.AccessToken;

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

        /// <summary>
        ///     获得当前用户
        /// </summary>
        /// <returns>System.Nullable{System.Int32}.</returns>
        /// <exception cref="Intime.OPC.Domain.Exception.UserIdConverException"></exception>
        protected int GetCurrentUserID()
        {
            if (ActionContext.Request.Properties.ContainsKey(AccessTokenConst.UseridPropertiesName))
            {
                string userid = ActionContext.Request.Properties[AccessTokenConst.UseridPropertiesName].ToString();
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

        protected IHttpActionResult DoFunction(Func<dynamic> action, string falseMeg)
        {
            try
            {
                var o = action();
                GetLog().Error(o);
                return Ok(o);
            }
            catch (HttpResponseException ex)
            {
                return new StatusCodeResult(HttpStatusCode.Unauthorized, this);
            }
            catch (UserIdConverException ex)
            {
                GetLog().Error(ex);

                return BadRequest("用户未登录或非法用户！");
            }
            catch (SaleOrderNotExistsException e)
            {
                return BadRequest("销售单编号不能为空");
            }

            catch (Exception ex)
            {
                GetLog().Error(ex);
                return BadRequest(ex.Message);
            }
        }

        protected IHttpActionResult DoAction(Action action, string falseMeg)
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
            catch (UserIdConverException ex)
            {
                GetLog().Error(ex);

                return BadRequest("用户未登录或非法用户！");
            }
            catch (SaleOrderNotExistsException e)
            {
                return BadRequest("销售单编号不能为空");
            }
            catch (Exception ex)
            {
                GetLog().Error(ex);
                return InternalServerError();
            }
        }
    }
}