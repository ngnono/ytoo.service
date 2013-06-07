using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Yintai.Architecture.Framework.ServiceLocation;
using Yintai.Hangzhou.Cms.WebSiteV1.Controllers;
using Yintai.Hangzhou.Model.Enums;
using Yintai.Hangzhou.Repository.Contract;

namespace Yintai.Hangzhou.Cms.WebSiteV1.Util
{
    public class UserAuthDataAttribute:AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            DoAuthorization(filterContext);
            base.OnAuthorization(filterContext);
        }

        private void DoAuthorization(AuthorizationContext filterContext)
        {
            var httpContext = filterContext.HttpContext;
            int targetId = int.Parse(httpContext.Request["id"]);
            string action_filtered = "delete";
            string errorUnAuthorizedDataAccess = "没有授权操作该门店和品牌！";
            //authorize
            string controllerName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
            string actionName = filterContext.ActionDescriptor.ActionName;
            UserController currentController = filterContext.Controller as UserController;
            if (string.Compare(actionName, action_filtered, true) != 0)
                return;
            var currentUser = currentController.CurrentUser;
            if (currentUser == null)
            {
                httpContext.Response.StatusCode = 401;
                return;
            }
            IUserAuthRepository authRepo = ServiceLocator.Current.Resolve<IUserAuthRepository>();
            if (currentUser.Role == (int)UserRole.Admin)
                return;
            if (currentController is ProductController)
            {  
                var entity = ServiceLocator.Current.Resolve<IProductRepository>().Find(targetId);
                if (entity == null)
                    return;
                if (!authRepo.Get(a => a.UserId == currentUser.CustomerId)
                    .Any(a => a.StoreId == entity.Store_Id &&
                             (a.BrandId == 0 || a.BrandId == entity.Brand_Id)))
                {
                    httpContext.Response.StatusCode = 401;
                    httpContext.Response.StatusDescription = errorUnAuthorizedDataAccess;
                    return;
                }
            }
            else if (currentController is PromotionController)
            {
                var entity = ServiceLocator.Current.Resolve<IPromotionRepository>().Find(targetId);
                if (entity == null)
                    return;
                if (!authRepo.Get(a => a.UserId == currentUser.CustomerId)
                    .Any(a => a.StoreId == entity.Store_Id))
                {
                    httpContext.Response.StatusCode = 401;
                    httpContext.Response.StatusDescription = errorUnAuthorizedDataAccess;
                    return;
                }
            }
            
        }
    }
}
