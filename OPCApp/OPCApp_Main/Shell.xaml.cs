using System.ComponentModel.Composition;
using System.Reflection;
using System.Windows;
using MahApps.Metro.Controls;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;

namespace OPCApp.Main
{
    [Export(typeof (MetroWindow))]
    public partial class Shell : MetroWindow, IPartImportsSatisfiedNotification
    {
        [Import(AllowRecomposition = false)] public IModuleManager ModuleManager;

        [Import(AllowRecomposition = false)] public IRegionManager RegionManager;

        public Shell()
        {
            InitializeComponent();
            Title = "��̩�ٻ�OPC v" + Assembly.GetExecutingAssembly().GetName().Version;
        }

        public void OnImportsSatisfied()
        {
        }

        private void NavigationItemsControl_OnLoaded(object sender, RoutedEventArgs e)
        {
        }
    }
}