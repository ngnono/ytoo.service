//===================================================================================
//OPCApp_Main
// 
//===================================================================================
// OPC��Ŀ������
// ���ߣ�������
// �������ڣ�2014-2-5
//===================================================================================
// �޸ļ�¼
//
//===================================================================================

using System.Windows;

namespace OPCApp.Main
{
    /// <summary>
    ///     App.xaml
    /// </summary>
    public partial class App : Application
    {
        public QuickStartBootstrapper bootstrapper;

        protected override void OnStartup(StartupEventArgs e)
        {
            //UploadManager.Copy();
            //UploadManager.UpLoad();

            bootstrapper = new QuickStartBootstrapper();
            bootstrapper.Run();
           
        }
    }
}