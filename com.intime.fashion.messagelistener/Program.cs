using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace com.intime.fashion.messagelistener
{
    class Program
    {
        static void Main(string[] args)
        {
#if !DEBUG
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] 
            { 
                new MainJobService() 
            };
            ServiceBase.Run(ServicesToRun);
#else

            new MainJobService().ConsoleDebug();

            Console.ReadLine();
#endif
        }
    }
}
