using Common.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.intime.jobscheduler.Job.Analysis
{

    [DisallowConcurrentExecution]
    class PullAnalysisData2ESJob : IJob
    {
        
        public void Execute(IJobExecutionContext context)
        {
            ILog log = LogManager.GetLogger(this.GetType());

            JobDataMap data = context.JobDetail.JobDataMap;

            var totalCount = 0;
            var interval = data.ContainsKey("intervalOfDays") ? data.GetInt("intervalOfDays") : 1;
            if (!data.ContainsKey("benchtime"))
            {
                data.Put("benchtime", DateTime.Now.AddDays(-interval));
            }
            else
            {
                data["benchtime"] = data.GetDateTimeValue("benchtime").AddDays(interval);
            }
            var benchTime = data.GetDateTime("benchtime");

            var sw = new Stopwatch();

            sw.Stop();
            log.Info(string.Format("total income history:{0},{1} converted incomes in {2} => {3} docs/s", totalCount, successCount, sw.Elapsed, successCount / sw.Elapsed.TotalSeconds));
        }
    }
}
