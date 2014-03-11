//===================================================================================
//OPCApp_Main
// 
//===================================================================================
// OPC项目主程序
// 作者：赵晓玉
// 创建日期：2014-2-5
//===================================================================================
// 修改记录
//
//===================================================================================
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace OPCApp.Main
{
    /// <summary>
    ///  App.xaml
    /// </summary>
    public partial class App : Application
    {
        public QuickStartBootstrapper bootstrapper;
        protected override void OnStartup(StartupEventArgs e)
        {
            
            bootstrapper = new QuickStartBootstrapper();
            bootstrapper.Run();
        }
    }
}
