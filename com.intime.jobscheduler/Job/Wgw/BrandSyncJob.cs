﻿using com.intime.fashion.data.sync.Wgw.Executor;
using Common.Logging;
using Quartz;
using System;

namespace com.intime.jobscheduler.Job.Wgw
{
    [DisallowConcurrentExecution]
    public class BrandSyncJob : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
#if DEBUG
            DateTime benchTime = DateTime.Now.AddYears(-1);
#else
            JobDataMap data = context.JobDetail.JobDataMap;
            var interval = data.ContainsKey("intervalOfHrs") ? data.GetInt("intervalOfHrs") : 1;
            if (!data.ContainsKey("benchtime"))
            {
                data.Put("benchtime", DateTime.Now.AddHours(-interval));
            }
            else
            {
                data["benchtime"] = data.GetDateTimeValue("benchtime").AddHours(interval);
            }
            var benchTime = data.GetDateTime("benchtime");
#endif
            ILog logger = LogManager.GetLogger(this.GetType());
            ExecuteResult executeInfo = null;
            var analytic = ActionTimer.Perform(() =>
            {
                var brandSyncExecutor = new BrandSyncExecutor(benchTime, logger);
                executeInfo = brandSyncExecutor.Execute();
            });


            if (executeInfo.Status != ExecuteStatus.Succeed)
            {
                foreach (var msg in executeInfo.MessageList)
                {
                    logger.Error(msg);
                }
            }
            else
            {
                logger.Info(string.Format("映射商品,成功{0},失败{1},总数{2},执行时间{3},效率{4}", 
                    executeInfo.SucceedCount,
                    executeInfo.FailedCount, 
                    executeInfo.TotalCount, 
                    analytic.TotalSeconds,
                    executeInfo.SucceedCount/analytic.TotalSeconds));
            }
        }
    }
}
