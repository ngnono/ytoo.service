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

            var jobs = new List<IJob>();
            JobDataMap data = context.JobDetail.JobDataMap;
             var includeStore = data.ContainsKey("includeStore") ? data.GetBoolean("includeStore") : true;
            var includeSection = data.ContainsKey("includeSection") ? data.GetBoolean("includeSection") : true;
            var includeCat = data.ContainsKey("includeCat") ? data.GetBoolean("includeCat") : true;
            var includeBrand = data.ContainsKey("includeBrand") ? data.GetBoolean("includeBrand") : true;

            var includeProd = data.ContainsKey("includeProd") ? data.GetBoolean("includeProd") : true;
            var includeInv = data.ContainsKey("includeInv") ? data.GetBoolean("includeInv") : true;
             var includePic = data.ContainsKey("includePic") ? data.GetBoolean("includePic") : true;
            if (includeStore)
                jobs.Add(new StoreSyncJob());
            if (includeSection)
                jobs.Add(new SectionSyncJob());
            if (includeCat)
                jobs.Add(new CategorySyncJob());
            if (includeBrand)
                jobs.Add(new BrandSyncJob());
            if (includeProd)
                jobs.Add(new ProductSyncJob());
            if (includeInv)
                jobs.Add(new ProductInventorySyncJob());
            if (includePic)
                jobs.Add(new ProductPicSyncJob());
           
            foreach (var job in jobs)
            {
                job.Execute(context);
            }
        }
    }
}
