﻿
using Microsoft.Practices.Prism.MefExtensions.Modularity;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;
using OPCApp.Infrastructure;
using OPCApp.TransManage.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace OPCApp.TransManage
{
    //test
    //liuyh test 222
    [ModuleExport(typeof(TransManageModule))]
    public class TransManageModule : IModule
    {
        [Import]
        public IRegionManager RegionManager;
        public void Initialize()
        {
           // this.RegionManager.RegisterViewWithRegion(RegionNames.MainNavigationRegion, typeof(NavigationItemView));
        }
    }
}
