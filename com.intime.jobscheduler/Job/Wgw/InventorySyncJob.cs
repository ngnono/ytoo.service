using com.intime.fashion.data.sync.Wgw.Executor;
using Common.Logging;
using Quartz;
using System;

namespace com.intime.jobscheduler.Job.Wgw
{
    [DisallowConcurrentExecution]
    public class InventorySyncJob : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            DateTime benchTime = DateTime.Now.AddYears(-1);
#if !DEBUG
            JobDataMap data = context.JobDetail.JobDataMap;
            var isRebuild = data.ContainsKey("isRebuild") && data.GetBoolean("isRebuild");
            if (!isRebuild)
            {
                var interval = data.ContainsKey("intervalOfSecs") ? data.GetInt("intervalOfSecs") : 5 * 60;
                benchTime = DateTime.Now.AddSeconds(-interval);
            }
#endif
            ILog logger = LogManager.GetLogger(this.GetType());
            ExecuteResult executeInfo = null;
            var analytic = ActionTimer.Perform(() =>
            {
                var inventorySyncExecutor = new InventorySyncExecutor(benchTime, logger);
                executeInfo = inventorySyncExecutor.Execute();
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
                logger.Info(string.Format("同步商品库存,成功{0},失败{1},总数{2},执行时间{3},效率{4}",
                    executeInfo.SucceedCount,
                    executeInfo.FailedCount,
                    executeInfo.TotalCount,
                    analytic.TotalSeconds,
                    executeInfo.SucceedCount / analytic.TotalSeconds));
            }

        }







    }
}
