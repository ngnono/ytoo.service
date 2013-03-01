using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;
using System.Web.Routing;
using Yintai.Architecture.Common.Data.EF;
using Yintai.Architecture.Common.Models;
using Yintai.Architecture.Common.Web.Mvc.Routes;
using Yintai.Architecture.Framework.ServiceLocation;
using Yintai.Hangzhou.Data.Models;
using Yintai.Hangzhou.Service.Contract;
using Yintai.Hangzhou.WebSupport.Binder;
using Yintai.Hangzhou.WebSupport.Ioc;

namespace Yintai.Hangzhou.WebSupport.Mvc
{
    public class CmsV1Application : MvcApplication
    {
        private readonly string _controller;
        private const string CMS_ROLERIGHT_MAP_KEY = "CMS_ROLESRIGHT_MAP";
        private const string CMS_RIGHT_MAP_KEY = "CMS_RIGHT_MAP";

        protected CmsV1Application()
            : this("Yintai.Hangzhou.Cms.WebSiteCoreV1.Controllers")
        {
        }

        protected CmsV1Application(string defaultControllerNamespace)
            : base(defaultControllerNamespace)
        {
            _controller = defaultControllerNamespace;
        }

        protected override void RegisterRoutesC(RouteCollection routes)
        {
            routes.MapLowerCaseUrlRoute(
"Default_Page", // Route name
"{controller}/{action}/{page}", // URL with parameters
new { controller = "Home", action = "Index", id = 0, page = 1 } // Parameter defaults
, new { action = @".*List", page = @"\d*" } //正则列表页结尾 list
, new[] { _controller }
);
        }
        protected override void CApplication_Start()
        {
            base.CApplication_Start();
            //initial load roleright map into table
            PreLoadRolesRightMap();
        }

        private void PreLoadRolesRightMap()
        {
            IUserRightService userRightService = ServiceLocator.Current.Resolve<IUserRightService>();
            var roleRightMap = new List<RoleEntity>();
            var rightsMap = new List<AdminAccessRightEntity>();
            foreach (var rr in userRightService.LoadAllRolesRight())
            {
                roleRightMap.Add(new RoleEntity()
                {
                    Id = rr.Id
                    ,
                    CreatedDate = rr.CreatedDate
                    ,
                    CreatedUser = rr.CreatedUser
                    ,
                    Description = rr.Description
                    ,
                    Name = rr.Name
                    ,
                    Status = rr.Status
                    ,
                    Val = rr.Val
                    ,
                    RoleAccessRights = (from right in rr.RoleAccessRights
                                        select new RoleAccessRightEntity()
                                        {
                                            Id = right.Id
                                            ,
                                            AccessRightId = right.AccessRightId
                                            ,
                                            RoleId = right.RoleId
                                            ,
                                            AdminAccessRight = new AdminAccessRightEntity()
                                            {
                                                ControllName = right.AdminAccessRight.ControllName
                                                ,
                                                ActionName = right.AdminAccessRight.ActionName
                                                ,
                                                Id = right.AdminAccessRight.Id
                                            }
                                        }).ToList()
                });
            }
            foreach (var r in userRightService.LoaddAllRights())
            {
                rightsMap.Add(new AdminAccessRightEntity()
                {
                    Id = r.Id
                    ,
                    ActionName = r.ActionName
                    ,
                    ControllName = r.ControllName
                });
            }
            HttpRuntime.Cache.Add(CMS_ROLERIGHT_MAP_KEY
                , new object[]{
                     roleRightMap,
                     rightsMap
                }
                , null
                , Cache.NoAbsoluteExpiration
                , TimeSpan.FromMinutes(20)
                , CacheItemPriority.Normal
                , new CacheItemRemovedCallback((key, value, reason) =>
                {
                    this.PreLoadRolesRightMap();
                }));

        }
        public static CmsV1Application Current
        {
            get
            {
                return HttpContext.Current.ApplicationInstance as CmsV1Application;
            }
        }
        public List<RoleEntity> RoleRightMap
        {
            get
            {
                object map = HttpRuntime.Cache.Get(CMS_ROLERIGHT_MAP_KEY);
                if (map == null)
                {
                    PreLoadRolesRightMap();
                    map = HttpRuntime.Cache.Get(CMS_ROLERIGHT_MAP_KEY);
                }
                var roles = (map as object[])[0];
                return roles as List<RoleEntity>;

            }
        }
        public List<AdminAccessRightEntity> RightsMap
        {
            get
            {
                object map = HttpRuntime.Cache.Get(CMS_ROLERIGHT_MAP_KEY);
                if (map == null)
                {
                    PreLoadRolesRightMap();
                    map = HttpRuntime.Cache.Get(CMS_ROLERIGHT_MAP_KEY);
                }
                var roles = (map as object[])[1];
                return roles as List<AdminAccessRightEntity>;

            }
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
            var unit = ServiceLocator.Current.Resolve<IUnitOfWork>();
            if (unit != null)
                unit.Dispose();
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

            RegisterRoutesC(routes);

            routes.MapLowerCaseUrlRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional }// Parameter defaults
                , new[] { _controller }
            );
        }

        protected virtual void RegisterRoutesC(RouteCollection routes)
        {
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
