using System.ComponentModel.Composition;
using System.Reflection;
using MahApps.Metro.Controls;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;

namespace OPCApp.Main
{
    [Export]
    public partial class Shell : MetroWindow, IPartImportsSatisfiedNotification
    {
        [Import(AllowRecomposition = false)] public IModuleManager ModuleManager;

        [Import(AllowRecomposition = false)] public IRegionManager RegionManager;

        public Shell()
        {
            InitializeComponent();
            Title = "ÒøÌ©°Ù»õOPC v" + Assembly.GetExecutingAssembly().GetName().Version;
        }

        public void OnImportsSatisfied()
        {
        }
    }
}