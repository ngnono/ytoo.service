using System.ComponentModel.Composition.Hosting;
using OPCApp.Infrastructure.Config;
using OPCApp.Infrastructure.Interfaces;

namespace OPCApp.Infrastructure
{
    public class AppEx
    {
        private static ILoginManager logManager;
        public static IConfig Config { get; private set; }
        public static IContainer Container { get; private set; }

        public static ILoginModel LoginModel { get; private set; }

        public static void Init(CompositionContainer container)
        {
            Container = new MefContainer(container);
            logManager = Container.GetInstance<ILoginManager>();

            Config = new DefaultConfig();
        }

        public static bool Login(string userName, string password)
        {
            LoginModel = logManager.Login(userName, password);
            return logManager.IsLogin;
        }
    }
}