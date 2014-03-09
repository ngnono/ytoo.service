using Microsoft.Practices.Prism.MefExtensions.Modularity;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;
using OPCApp.Main.Infrastructure;
using OPCApp.Order.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OPCApp.Order
{
    [ModuleExport(typeof(OrderModule))]
    public class OrderModule : IModule
    {
        [Import]
        public IRegionManager RegionManager;
        public void Initialize()
        {
            this.RegionManager.RegisterViewWithRegion(RegionNames.MainNavigationRegion, typeof(OrderNavigationItemView));
        }
    }
}
