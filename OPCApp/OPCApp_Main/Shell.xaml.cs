
using System;
using System.ComponentModel.Composition;
using System.Windows.Controls;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;
using OPCApp.Main.Infrastructure;
using System.Windows;
using MahApps.Metro;
using MahApps.Metro.Controls;
namespace OPCApp.Main
{
    [Export]
    public partial class Shell : MahApps.Metro.Controls.MetroWindow, IPartImportsSatisfiedNotification
    {

        public Shell()
        {
            InitializeComponent();
            this.Title = "ÒøÌ©°Ù»õOPC v" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }

        [Import(AllowRecomposition = false)]
        public IModuleManager ModuleManager;

        [Import(AllowRecomposition = false)]
        public IRegionManager RegionManager;

        public void OnImportsSatisfied()
        {
          
        }
    }
}
