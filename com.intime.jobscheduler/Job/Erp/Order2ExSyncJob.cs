using com.intime.fashion.data.sync.Wgw;
using Common.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using Yintai.Hangzhou.Data.Models;
using Yintai.Hangzhou.Service.Logic;

namespace com.intime.jobscheduler.Job.Erp
{
    [DisallowConcurrentExecution]
    public class Order2ExSyncJob : IJob
    {
        private void Query(DateTime benchTime, Action<IQueryable<Map4Order>> callback)
        {
            using (var db = new YintaiHangzhouContext("YintaiHangzhouContext"))
            {
                var orders =
                    db.Set<Map4Order>()
                        .Where(
                            m =>
                                m.CreateDate >= benchTime && m.Channel == ConstValue.WGW_CHANNEL_NAME &&
                                !db.Set<Order2ExEntity>().Any(o => o.OrderNo == m.OrderNo));

                if (callback != null)
                    callback(orders);
            }
        }

        public void Execute(IJobExecutionContext context)
        {
            ILog log = LogManager.GetLogger(this.GetType());
            DateTime benchTime = DateTime.Now.AddYears(-1);
#if !DEBUG
             JobDataMap data = context.JobDetail.JobDataMap;
             var interval = data.ContainsKey("intervalOfSecs") ? data.GetInt("intervalOfSecs") : 5*60;
             benchTime = DateTime.Now.AddSeconds(-interval);
#endif
            var totalCount = 0;
            Query(benchTime, orders => { totalCount = orders.Count(); });
            int cursor = 0;
            int successCount = 0;
            int size = JobConfig.DEFAULT_PAGE_SIZE;
            int lastCursor = 0;
            var analytics = ActionTimer.Perform(() =>
            {
                while (cursor < totalCount)
                {
                    List<Map4Order> oneTimeList = null;
                    Query(benchTime, orders =>
                    {
                        oneTimeList = orders.Where(o => o.Id > lastCursor).OrderBy(a => a.Id).Take(size).ToList();
                    });
                    foreach (var order in oneTimeList)
                    {
                        try
                        {
                            if (OrderRule.SyncOrder2Erp(order.OrderNo))
                            {
                                successCount++;
                            }
                        }
                        catch (Exception ex)
                        {
                            log.Info(ex);
                        }
                    }
                    cursor += size;
                    lastCursor = oneTimeList.Max(o => o.Id);
                }
            }
           );
            log.Info(string.Format("total paid orders:{0},{1} synced orders in {2} => {3} docs/s", totalCount, successCount, analytics.Elapsed, successCount / analytics.TotalSeconds));

        }
    }
}
