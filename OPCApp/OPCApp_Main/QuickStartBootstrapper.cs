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
using MahApps.Metro.Controls;
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
            return Container.GetExportedValue<MetroWindow>();
        }

        protected override void InitializeShell()
        {
            base.InitializeShell();
            AppEx.Init(Container);
            var logon = AppEx.Container.GetInstance<Login>();
            //Application.Current.MainWindow.WindowState = WindowState.Maximized;
            if (logon.ShowDialog() == true)
            {
                Application.Current.MainWindow = (Window) Shell;
                Application.Current.MainWindow.ShowDialog();
            }
        }
    }
}