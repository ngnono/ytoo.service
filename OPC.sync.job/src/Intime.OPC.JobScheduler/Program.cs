using System;
using System.ServiceProcess;
using Intime.OPC.Job.Order.OrderStatusSync;
using Intime.OPC.Job.Product.ProductSync.Supports.Intime.Jobs;
using Intime.OPC.Job.Trade.SplitOrder;

namespace Intime.OPC.JobScheduler
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(String[] args)
        {
            new PropuctPropertySyncJob().Execute(null);
            return;
            if (args.Length > 0)
            {
                var job = new OrderNotifyJob();
                job.Execute(null);
            }
            else
            {

                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[]
                {
                    new MainJobService()
                };
                ServiceBase.Run(ServicesToRun);
            }
        }
    }
}
