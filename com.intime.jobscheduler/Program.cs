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
            //var querydata = new OrderQuery()
            //{
            //    AppId = WxPayConfig.APP_ID,
            //    SignMethod = "sha1",
            //    TimeStamp = 1383696365,
            //    Package = new OrderQueryPackage() {
            //        TradeNo = "1217446001201311058031690715"
            //    }
            //};
            
            //WxServiceHelper.Query(querydata,r=>{
            //    var ret = r;
            //},r=>{
            //    var ret = r;
            //});
            new MainJobService().ConsoleDebug();
            IJob job = null;
            //job = new Job.Wgw.ProductSyncJob();

            //job = new Job.Wgw.BrandSyncJob();
            //job = new Job.Wgw.OrderSyncJob();
            //job = new Job.Wgw.InventorySyncJob();
            //job = new Job.Wgw.ProductStatusSyncJob();
            //job = new GetItemMultiStockJob();
            //job = new Order2ExSyncJob();
            //job.Execute(null);
            Console.WriteLine("Job执行完成！");
            Console.ReadLine();
#endif
        }
    }
}
