using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Intime.OPC.WebApi.Core.MessageHandlers.AccessToken
{
    /// <summary>
    /// 绑定用户接口中的UserId
    /// </summary>
    public class UserIdFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            if (actionContext.Request.Properties.ContainsKey(AccessTokenConst.USERID_PROPERTIES_NAME))
            {
                if (actionContext.ActionArguments.ContainsKey("userId"))
                {
                    actionContext.ActionArguments["userId"] = actionContext.Request.Properties[AccessTokenConst.USERID_PROPERTIES_NAME];
                }
                else
                {
                    actionContext.ActionArguments.Add("userId", actionContext.Request.Properties[AccessTokenConst.USERID_PROPERTIES_NAME]);
                }
            }
        }
    }
}
