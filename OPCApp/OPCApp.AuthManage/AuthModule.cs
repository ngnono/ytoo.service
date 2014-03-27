using System.ComponentModel.Composition;
using Microsoft.Practices.Prism.MefExtensions.Modularity;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;
using OPCApp.AuthManage.Views;
using OPCApp.Infrastructure;

namespace OPCApp.AuthManage
{
    [ModuleExport(typeof (AuthModule))]
    public class AuthModule : IModule
    {
        [Import] public IRegionManager RegionManager;

        public void Initialize()
        {
            Logon();
            RegionManager.RegisterViewWithRegion(RegionNames.MainNavigationRegion, typeof (AuthNavigationItemView));
        }

        public void Logon()
        {
        }
    }
}