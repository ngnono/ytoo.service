using OPCApp.Infrastructure.Auth;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OPCApp.Main.Infrastructure
{
    public  class AppEx
    {
        public static void Init(CompositionContainer container) {
            Container = new MefContainer(container);
        }
        public static IContainer Container { get; private set; }

        public static IUser User { get; private set; }
    }
}
