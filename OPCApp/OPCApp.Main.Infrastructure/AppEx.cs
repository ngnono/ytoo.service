using OPCApp.Infrastructure.Auth;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OPCApp.Infrastructure.Interfaces;

namespace OPCApp.Infrastructure
{
    public  class AppEx
    {
        public static void Init(CompositionContainer container) {
            Container = new MefContainer(container);
            var loginManager = Container.GetInstance<ILoginManager>();

        }
        public static IContainer Container { get; private set; }

        public static ILoginModel LoginModel { get; private set; }
    }
}
