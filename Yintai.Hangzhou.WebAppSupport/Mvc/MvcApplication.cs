using System;
using System.Web.Mvc;
using System.Web.Routing;
using Yintai.Architecture.Common.Data.EF;
using Yintai.Architecture.Common.Models;
using Yintai.Architecture.Common.Web.Mvc.Routes;
using Yintai.Architecture.Framework.ServiceLocation;
using Yintai.Hangzhou.WebSupport.Binder;
using Yintai.Hangzhou.WebSupport.Ioc;

namespace Yintai.Hangzhou.WebSupport.Mvc
{
    public class CmsV1Application : MvcApplication
    {
        private readonly string _controller;

        protected CmsV1Application()
            : this("Yintai.Hangzhou.Cms.WebSiteCoreV1.Controllers")
        {

        }

        protected CmsV1Application(string defaultControllerNamespace)
        {
            _controller = defaultControllerNamespace;
        }
    }


    public class WebApiTestApplication : MvcApplication
    {
        public WebApiTestApplication()
            : base("Yintai.Hangzhou.Tools.ApiTest.Controllers")
        {
        }
    }

    public class WebApiApplication : MvcApplication
    {
        public WebApiApplication()
            : base("Yintai.Hangzhou.WebApiCore.Controllers")
        {
        }

        protected override void CApplication_Start()
        {
            //ImageMagickNET.MagickNet.InitializeMagick();
            base.CApplication_Start();
        }
    }

    public abstract class MvcApplication : System.Web.HttpApplication
    {
        private readonly string _controller;

        protected MvcApplication()
            : this(String.Empty)
        {

        }

        public void Application_EndRequest(Object sender, EventArgs e)
        {
            try
            {
                var unit = ServiceLocator.Current.Resolve<IUnitOfWork>();
                unit.Commit();
                unit.Dispose();
            }
            catch
            {

            }



            //PerRequestUnityServiceLocator.DisposeOfChildContainer();
        }

        public void Application_Error(Object sender, EventArgs e)
        {
            try
            {
                var unit = ServiceLocator.Current.Resolve<IUnitOfWork>();
                unit.Dispose();
            }
            catch
            {
            }

            // PerRequestUnityServiceLocator.DisposeOfChildContainer();
            //don't forget to treat the error here
        }

        protected MvcApplication(string defaultControllerNamespace)
        {
            _controller = defaultControllerNamespace;
        }

        public void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");


            routes.IgnoreRoute("{*alljs}", new { alljs = @".*\.js(/.*)?" });
            routes.IgnoreRoute("{*allcss}", new { allcss = @".*\.css(/.*)?" });
            routes.IgnoreRoute("{*alljpg}", new { alljpg = @".*\.jpg(/.*)?" });
            routes.IgnoreRoute("{*allgif}", new { allgif = @".*\.gif(/.*)?" });
            routes.IgnoreRoute("{*allpng}", new { allpng = @".*\.png(/.*)?" });

            routes.IgnoreRoute("{*allxls}", new { allxls = @".*\.xls(/.*)?" });
            //routes.IgnoreRoute("{*PRO_UPLOAD_TMPxls}", new { PRO_UPLOAD_TMPxls = @"(.*/)?PRO_UPLOAD_TMP.xls(/.*)?" });

            routes.IgnoreRoute("{*robotstxt}", new { robotstxt = @"(.*/)?robots.txt(/.*)?" });
            routes.IgnoreRoute("{*favicon}", new { favicon = @"(.*/)?favicon.ico(/.*)?" });

            routes.RouteExistingFiles = false;

            routes.MapLowerCaseUrlRoute(
                "Default_Page", // Route name
                "{controller}/{action}/{page}", // URL with parameters
                new { controller = "Home", action = "Index", id = 0, page = 1 } // Parameter defaults
                , new { action = @".*List", page = @"\d*" } //正则列表页结尾 list
                , new[] { _controller }
            );

            routes.MapLowerCaseUrlRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional }// Parameter defaults
                , new[] { _controller }
            );
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);

            ServiceLocatorInit();

            CApplication_Start();
        }

        protected virtual void CApplication_Start()
        {

        }

        /// <summary>
        /// 服务定位 初始化
        /// </summary>
        protected void ServiceLocatorInit()
        {
            //Register

            IocRegisterRun.Current.Register();

            DependencyResolver.SetResolver(ServiceLocator.Current.Resolve<IDependencyResolver>());

            ModelBinders.Binders.Add(typeof(PagerRequest), ServiceLocator.Current.Resolve<PagerRequestBinder>());
        }
    }
}
