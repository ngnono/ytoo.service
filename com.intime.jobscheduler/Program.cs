using com.intime.fashion.common;
using com.intime.fashion.common.Wxpay;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace com.intime.jobscheduler
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
#if !DEBUG
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] 
            { 
                new MainJobService() 
            };
            ServiceBase.Run(ServicesToRun);
#else
            var querydata = new OrderQuery()
            {
                AppId = WxPayConfig.APP_ID,
                SignMethod = "sha1",
                TimeStamp = 1383696365,
                Package = new OrderQueryPackage() {
                    TradeNo = "1217446001201311058031690715"
                }
            };
            
            WxServiceHelper.Query(querydata,r=>{
                var ret = r;
            },r=>{
                var ret = r;
            });
            new MainJobService().ConsoleDebug();
#endif
        }
    }
}
