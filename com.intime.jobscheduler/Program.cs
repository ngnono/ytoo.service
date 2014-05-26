using com.intime.fashion.common;
using com.intime.fashion.common.Wxpay;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using com.intime.jobscheduler.Job.Erp;
using com.intime.jobscheduler.Job.Wgw;
using Quartz;
using System.Xml.Serialization;

namespace com.intime.jobscheduler
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
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
