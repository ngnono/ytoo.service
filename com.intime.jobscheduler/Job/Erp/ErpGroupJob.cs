using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.intime.jobscheduler.Job.Erp
{
    [DisallowConcurrentExecution]
    class ErpGroupJob:IJob
    {

        public void Execute(IJobExecutionContext context)
        {
            IJob[] jobs = new IJob[] { 
                new StoreSyncJob(),
                new CategorySyncJob(),
                new BrandSyncJob(),
                new ProductSyncJob(),
                new ProductInventorySyncJob(),
                new ProductPicSyncJob()
            };
            foreach (var job in jobs)
            {
                job.Execute(context);
            }
        }
    }
}
