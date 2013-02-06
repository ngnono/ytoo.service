<<<<<<< HEAD
﻿
using System;
using System.Web;
using System.Web.Mvc;
using Yintai.Architecture.Common.Logger;
using Yintai.Architecture.Framework.ServiceLocation;
using Yintai.Hangzhou.Cms.WebSiteCoreV1.Manager;
using Yintai.Hangzhou.Model;
using Yintai.Hangzhou.Service.Contract;

namespace Yintai.Hangzhou.Cms.WebSiteCoreV1.Controllers
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
        /// 禁用 Response
        /// </summary>
        [Obsolete]
        public new HttpResponseBase Response
        {
            get
            {
                throw new NotSupportedException("禁止直接使用Response");
            }
        }

        /// <summary>
        /// 禁止直接使用Request
        /// </summary>
        [Obsolete]
        public new HttpRequestBase Request
        {
            get
            {
                throw new NotSupportedException("禁止直接使用Request");
            }
        }

        /// <summary>
        /// 禁止直接使用Session
        /// </summary>
        [Obsolete]
        public new HttpSessionStateBase Session
        {
            get
            {
                throw new NotSupportedException("禁止直接使用Session");
            }
        }

        /// <summary>
        /// 禁止直接使用HttpContext
        /// </summary>
        //[Obsolete]
        //public new HttpContextBase HttpContext
        //{
        //    get
        //    {
        //        throw new NotSupportedException("禁止直接使用HttpContext");
        //    }
        //}

        ///// <summary>
        ///// 禁止直接使用 ControllerContext.
        ///// </summary>
        ////[Obsolete]
        //public new ControllerContext ControllerContext
        //{
        //    get
        //    {
        //        throw new NotSupportedException("禁止直接使用ControllerContext");
        //    }
        //}

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
    }
}
=======
﻿
using System;
using System.Web;
using System.Web.Mvc;
using Yintai.Architecture.Common.Logger;
using Yintai.Architecture.Framework.ServiceLocation;
using Yintai.Hangzhou.Cms.WebSiteCoreV1.Manager;
using Yintai.Hangzhou.Model;
using Yintai.Hangzhou.Service.Contract;

namespace Yintai.Hangzhou.Cms.WebSiteCoreV1.Controllers
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
        /// 禁用 Response
        /// </summary>
        [Obsolete]
        public new HttpResponseBase Response
        {
            get
            {
                throw new NotSupportedException("禁止直接使用Response");
            }
        }

        /// <summary>
        /// 禁止直接使用Request
        /// </summary>
        [Obsolete]
        public new HttpRequestBase Request
        {
            get
            {
                throw new NotSupportedException("禁止直接使用Request");
            }
        }

        /// <summary>
        /// 禁止直接使用Session
        /// </summary>
        [Obsolete]
        public new HttpSessionStateBase Session
        {
            get
            {
                throw new NotSupportedException("禁止直接使用Session");
            }
        }

        /// <summary>
        /// 禁止直接使用HttpContext
        /// </summary>
        [Obsolete]
        public new HttpContextBase HttpContext
        {
            get
            {
                throw new NotSupportedException("禁止直接使用HttpContext");
            }
        }

        ///// <summary>
        ///// 禁止直接使用 ControllerContext.
        ///// </summary>
        ////[Obsolete]
        //public new ControllerContext ControllerContext
        //{
        //    get
        //    {
        //        throw new NotSupportedException("禁止直接使用ControllerContext");
        //    }
        //}

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
    }
}
>>>>>>> edit create coupon logic
