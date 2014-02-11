using com.intime.fashion.common;
using com.intime.fashion.common.Wxpay;
using Common.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Yintai.Hangzhou.Data.Models;
using Yintai.Hangzhou.Model.Enums;
using Yintai.Hangzhou.Service.Logic;

namespace com.intime.jobscheduler.Job.Erp
{
    [DisallowConcurrentExecution]
    class OrderSyncJob : IJob
    {

        private void Query(DateTime benchTime, Action<IQueryable<OrderTransactionEntity>> callback)
        {
            using (var db = new YintaiHangzhouContext("YintaiHangzhouContext"))
            {
                var accounts = db.Set<OrderTransactionEntity>().Where(ot => ot.IsSynced == false && ot.CanSync == 0);

                if (callback != null)
                    callback(accounts);
            }
        }
        public void Execute(IJobExecutionContext context)
        {
            ILog log = LogManager.GetLogger(this.GetType());
            var totalCount = 0;
#if DEBUG
            DateTime benchTime = DateTime.Now.AddMonths(-1);
#else
            JobDataMap data = context.JobDetail.JobDataMap;
            var interval = data.ContainsKey("intervalOfSecs") ? data.GetInt("intervalOfSecs") : 5 * 60;
            var benchTime = DateTime.Now.AddSeconds(-interval);
#endif
            Query(benchTime, orders => totalCount = orders.Count());

            int cursor = 0;
            int successCount = 0;
            int size = JobConfig.DEFAULT_PAGE_SIZE;
            int lastCursor = 0;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            while (cursor < totalCount)
            {
                List<OrderTransactionEntity> oneTimeList = null;
                Query(benchTime, orders =>
                {
                    oneTimeList = orders.Where(a => a.Id > lastCursor).OrderBy(a => a.Id).Take(size).ToList();
                });
                foreach (var order in oneTimeList)
                {
                    try
                    {
                        bool isOnlinePaid = order.OrderType == (int)PaidOrderType.Self;
                        bool isSuccess = OrderRule.OrderPaid2Erp(order,isOnlinePaid);
                         if (isSuccess)
                             successCount++;

                      
                    }
                    catch (Exception ex)
                    {
                        log.Info(ex);
                    }
                }
                cursor += size;
                if (oneTimeList!=null && oneTimeList.Count>0)
                    lastCursor = oneTimeList.Max(o => o.Id);
            }

            sw.Stop();
            log.Info(string.Format("total paid orders:{0},{1} synced orders in {2} => {3} docs/s", totalCount, successCount, sw.Elapsed, successCount / sw.Elapsed.TotalSeconds));


        }

    }
}
