
using Microsoft.Practices.Prism.MefExtensions.Modularity;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;
using OPCApp.Main.Infrastructure;
using OPCApp.BaseInfoManage.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace OPCApp.BaseInfoManage
{
    //test push
    [ModuleExport(typeof(BaseInfoModule))]
    public class BaseInfoModule : IModule
    {
        [Import]
        public IRegionManager RegionManager;
        public void Initialize()
        {
            this.RegionManager.RegisterViewWithRegion(RegionNames.MainNavigationRegion, typeof(NavigationItemView));
        }
    }
}
