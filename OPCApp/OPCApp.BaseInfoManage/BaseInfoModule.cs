using System.ComponentModel.Composition;
using Microsoft.Practices.Prism.MefExtensions.Modularity;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;

namespace OPCApp.BaseInfoManage
{
    [ModuleExport(typeof (BaseInfoModule))]
    public class BaseInfoModule : IModule
    {
        [Import] 
        public IRegionManager RegionManager { get; set; }

        public void Initialize()
        {
            //this.RegionManager.RegisterViewWithRegion(RegionNames.MainNavigationRegion, typeof(NavigationItemView));
        }
    }
}