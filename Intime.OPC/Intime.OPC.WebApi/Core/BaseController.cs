﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
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
        /// Gets the log.
        /// </summary>
        /// <returns>ILog.</returns>
        protected ILog GetLog()
        {
            return LoggerManager.Current();
        }

        /// <summary>
        /// 获得当前用户
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
                else
                {
                    throw new UserIdConverException(userid);

                }
            }
            else
            {
                throw new HttpResponseException(HttpStatusCode.Unauthorized); 
 
            }
        }
        protected IHttpActionResult DoFunction(Func<bool> action,string falseMeg)
        {
            try
            {
                return Ok(action());
            }
            catch (HttpResponseException ex)
            {
                this.GetLog().Error(ex);

                return BadRequest("用户未登录！");
            }
            catch (UserIdConverException ex)
            {
                this.GetLog().Error(ex);

                return BadRequest("用户未登录或非法用户！");
            }

            catch (Exception ex)
            {
                this.GetLog().Error(ex);
                return InternalServerError();
            }
        }
    }
}
