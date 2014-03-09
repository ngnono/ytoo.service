
using Microsoft.Practices.Prism.MefExtensions.Modularity;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;
using OPCApp.Main.Infrastructure;
using OPCApp.AuthManage.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace OPCApp.AuthManage
{
    [ModuleExport(typeof(AuthModule))]
    public class AuthModule : IModule
    {
        [Import]
        public IRegionManager RegionManager;
        public void Initialize()
        {
            this.RegionManager.RegisterViewWithRegion(RegionNames.MainNavigationRegion, typeof(AuthNavigationItemView));
        }
    }
}
