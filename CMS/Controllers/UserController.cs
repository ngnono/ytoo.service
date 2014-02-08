
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Linq;
using Yintai.Architecture.Common.Logger;
using Yintai.Architecture.Framework.ServiceLocation;
using Yintai.Hangzhou.Cms.WebSiteV1.Manager;
using Yintai.Hangzhou.Model;
using Yintai.Hangzhou.Service.Contract;
using Yintai.Hangzhou.WebSupport.Mvc;
using Yintai.Hangzhou.Model.Enums;
using Yintai.Hangzhou.Cms.WebSiteV1.Util;
using CrystalDecisions.CrystalReports.Engine;
using System.IO;
using CrystalDecisions.Shared;
using System.Data.Entity;

namespace Yintai.Hangzhou.Cms.WebSiteV1.Controllers
{
    public abstract class UserController : System.Web.Mvc.Controller
    {
        public MappingManager MappingManager { get; set; }

        protected UserController()
        {
            
            Log = ServiceLocator.Current.Resolve<ILog>();
            AuthenticationService = ServiceLocator.Current.Resolve<IAuthenticationService>();
            MappingManager = new MappingManager();
        }

        protected void SetAuthorize(WebSiteUser webSiteUser)
        {
            AuthenticationService.SetAuthorize(base.HttpContext, webSiteUser);
        }

        protected void Signout()
        {
            AuthenticationService.Signout(base.HttpContext);
        }

        /// <summary>
        /// 当前登录用户
        /// </summary>
        private WebSiteUser _currentUser;

        /// <summary>
        /// Gets or sets ILog.
        /// </summary>
        public ILog Log
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets AuthenticationService.
        /// </summary>
        public IAuthenticationService AuthenticationService { get; set; }

        /// <summary>
        /// 获取当前登录用户
        /// </summary>
        public WebSiteUser CurrentUser
        {
            get { return this._currentUser ?? (this._currentUser = this.AuthenticationService.GetCurrentUser(base.HttpContext)); }
            /*get
            {
                if (HttpContext.Items["_CurrentUser_"]==null)
                {
                    HttpContext.Items["_CurrentUser_"] = AuthenticationService.GetCurrentUser(this.HttpContext);
                }
                return HttpContext.Items["_CurrentUser_"] as User;
            }*/
        }

      
      
       
        /// <summary>
        /// 根据Ajax请求切换对应视图
        /// </summary>
        /// <param name="viewName">
        /// The view name.
        /// </param>
        /// <param name="viewNameForAjax">
        /// The view name for ajax.
        /// </param>
        /// <returns>
        /// the view
        /// </returns>
        protected ViewResult ViewWithAjax(string viewName, string viewNameForAjax)
        {
            return this.ViewWithAjax(viewName, viewNameForAjax, null);
        }

        /// <summary>
        /// 根据Ajax请求切换对应视图
        /// </summary>
        /// <param name="viewName">
        /// The view name.
        /// </param>
        /// <param name="viewNameForAjax">
        /// The partial view name.
        /// </param>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <returns>
        /// The view.
        /// </returns>
        protected ViewResult ViewWithAjax(string viewName, string viewNameForAjax, object model)
        {
            return this.View(base.Request.IsAjaxRequest() ? viewNameForAjax : viewName, model);
        }

        /// <summary>
        /// 向摘要增加错误信息
        /// 使用@Html.ValidationSummary()在view中输出 
        /// </summary>
        /// <param name="errMessage">
        /// The err message.
        /// </param>
        protected void AppendErrorSummary(string errMessage)
        {
            this.ModelState.AddModelError("summary", errMessage);
        }

        protected ActionResult Success(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;

            return View("Success");
        }
        protected JsonResult SuccessResponse()
        {
            return Json(new { 
                 Success = true
            });
        }
        protected JsonResult FailResponse()
        {
            return FailResponse(string.Empty);
        }
        protected JsonResult FailResponse(string message)
        {
            return Json(new
            {
                Success = false,
                Message = message
            });
        }
        protected ActionResult RedirectToAction2(Func<ActionResult> nextaction)
        {
                var url = ControllerContext.RequestContext.HttpContext.Request.Params["returnUrl"];
                if (url != null)
                    return Redirect(url.ToString());
                return nextaction();
        }
        protected ActionResult RenderReport(string reportName, Action<ReportClass> reportCallback)
        {
            return RenderReport(reportName, reportCallback,ExportFormatType.PortableDocFormat );
        }
        protected ActionResult RenderReport(string reportName, Action<ReportClass> reportCallback,ExportFormatType format)
        {
            ReportClass rptH = new ReportClass();
            rptH.FileName = Server.MapPath(string.Format("~/Content/report/{0}.rpt", reportName));
            reportCallback(rptH);

            Stream stream = rptH.ExportToStream(format);
            var contentype = "application/pdf";
            switch (format)
            { 
                case ExportFormatType.Excel:
                    contentype = "application/vnd.ms-excel";
                    break;
            }
            return File(stream, contentype);
        }
        protected void ExcludeModelFieldsFromValidate(IEnumerable<string> keys)
        {
            if (keys == null)
                return;
            foreach (var key in keys)
            {
                if (key.EndsWith("."))
                {
                    var removeKeys = ModelState.Keys.Where(k => k.StartsWith(key)).ToArray();
                    foreach (var removeKey in removeKeys)
                    {
                        ModelState.Remove(removeKey);
                    }
                }
                else
                {
                    ModelState.Remove(key);
                }
            }
        }
        public DbContext Context
        {
            get
            {
                return ServiceLocator.Current.Resolve<DbContext>();
            }
        }
        public bool HasRightForCurrentAction()
        {
            object action = RouteData.Values["action"];
            string actionName = string.Empty;
            if (action != null)
                actionName = action.ToString();
            return HasRightForAction(actionName);
        }
        public bool HasRightForAction(string actionName)
        {
            string controllName = RouteData.Values["controller"].ToString();
            return HasRightForAction(controllName, actionName);
            
        }
        public bool HasRightForAction(string controller,string actionName)
        {
           
            if (string.IsNullOrEmpty(controller))
                controller= RouteData.Values["controller"].ToString();
            if (string.IsNullOrEmpty(actionName))
                actionName = RouteData.Values["action"].ToString(); 
            actionName = actionName.ToLower();
            string controllName = controller.ToLower();
            if (CurrentUser == null)
                return true;
            int hasRoles = (int)CurrentUser.Role;
            if ((hasRoles & (int)UserRole.Admin) == (int)UserRole.Admin)
                return true;
            //check controll+action whether need authorize
            bool needAuthroize = (from right in CMSApplication.Current.RightsMap
                                 select string.Concat(right.ControllName.ToLower()
                                        , right.ActionName.ToLower())
                                 ).Contains(string.Concat(controllName,actionName));
            if (!needAuthroize)
                return true;
            var result = (from role in CMSApplication.Current.RoleRightMap
                          from right in role.RoleAccessRights
                          where (hasRoles & role.Val) == role.Val &&
                                right.AdminAccessRight!=null && 
                                 right.AdminAccessRight.ControllName.ToLower() == controllName &&
                                right.AdminAccessRight.ActionName.ToLower() == actionName
                          select new { id = role.Id }
                        ).FirstOrDefault();
            return result != null;


        }
        [HttpGet]
        public virtual JsonResult AutoComplete(string name)
        {
            return Json(new [] {new{Name=string.Empty} }
                , JsonRequestBehavior.AllowGet);
        }
    }
    [AdminAuthorize]
    public class ConfigurationController : UserController
    {
        public ActionResult Index()
        {
            return View();
        }
    }   
}
