using System;
using System.Linq;
using System.Collections.Generic;
using System.Web;
using System.Web.Caching;
using System.Web.Routing;
using Yintai.Hangzhou.Data.Models;
using Yintai.Hangzhou.Service.Contract;
using Yintai.Hangzhou.WebSupport.Mvc;
using Yintai.Architecture.Common.Web.Mvc.Routes;
using Yintai.Architecture.Framework.ServiceLocation;
using Recaptcha;
namespace Yintai.Hangzhou.Cms.WebSiteV1.Util
{
    public class CMSApplication: MvcApplication
    {
        private readonly string _controller;
        private const string CMS_ROLERIGHT_MAP_KEY = "CMS_ROLESRIGHT_MAP";
        private const string CMS_RIGHT_MAP_KEY = "CMS_RIGHT_MAP";

        protected CMSApplication()
            : this("Yintai.Hangzhou.Cms.WebSiteV1.Controllers")
        {
        }

        protected CMSApplication(string defaultControllerNamespace)
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
, new { action = @"(list|card|index)", page = @"\d*" } //正则列表页结尾 list
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
        public static CMSApplication Current
        {
            get
            {
                return HttpContext.Current.ApplicationInstance as CMSApplication;
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
}