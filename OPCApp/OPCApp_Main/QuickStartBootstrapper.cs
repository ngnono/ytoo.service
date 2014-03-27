//===================================================================================
// 
// 
//===================================================================================
// 
// 
//===================================================================================

using System;
using System.ComponentModel.Composition.Hosting;
using System.Windows;
using Microsoft.Practices.Prism.MefExtensions;
using Microsoft.Practices.Prism.Modularity;
using OPCApp.Infrastructure;

namespace OPCApp.Main
{
    public class QuickStartBootstrapper : MefBootstrapper
    {
        private const string ModuleCatalogUri = "/OPCApp.Main;component/ModulesCatalog.xaml";

        protected override void ConfigureAggregateCatalog()
        {
            base.ConfigureAggregateCatalog();

            AggregateCatalog.Catalogs.Add(new AssemblyCatalog(typeof (QuickStartBootstrapper).Assembly));

            AggregateCatalog.Catalogs.Add(new DirectoryCatalog(AppDomain.CurrentDomain.BaseDirectory));
        }

        protected override IModuleCatalog CreateModuleCatalog()
        {
            return new ConfigurationModuleCatalog();
        }

        protected override DependencyObject CreateShell()
        {
            return Container.GetExportedValue<Shell>();
        }

        protected override void InitializeShell()
        {
            base.InitializeShell();
            AppEx.Init(Container);
            Application.Current.MainWindow = (Window) Shell;
            Application.Current.MainWindow.WindowState = WindowState.Maximized;
            Application.Current.MainWindow.Show();
        }
    }
}