//===================================================================================
//
//===================================================================================
//
//===================================================================================
//
//===================================================================================
using System;
using System.ComponentModel.Composition;
using System.Windows.Controls;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;
using OPCApp.Infrastructure;
using System.Windows;
using MahApps.Metro;
using MahApps.Metro.Controls;
namespace OPCApp.Main
{
    [Export]
    public partial class Shell : MahApps.Metro.Controls.MetroWindow, IPartImportsSatisfiedNotification
    {
        private const string EmailModuleName = "EmailModule";
        private static Uri InboxViewUri = new Uri("/InboxView", UriKind.Relative);
        public Shell()
        {
            InitializeComponent();
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
