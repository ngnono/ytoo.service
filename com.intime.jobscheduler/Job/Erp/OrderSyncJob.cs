﻿using com.intime.fashion.common;
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
                var accounts = db.Set<OrderTransactionEntity>().Where(ot => ot.IsSynced == false);

                if (callback != null)
                    callback(accounts);
            }
        }
        public void Execute(IJobExecutionContext context)
        {
            ILog log = LogManager.GetLogger(this.GetType());

            JobDataMap data = context.JobDetail.JobDataMap;
            var interval = data.ContainsKey("intervalOfSecs") ? data.GetInt("intervalOfSecs") : 5 * 60;
            var totalCount = 0;
            var benchTime = DateTime.Now.AddSeconds(-interval);

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
                        bool isSuccess = OrderRule.OrderPaid2Erp(order);
                         if (isSuccess)
                             successCount++;

                      
                    }
                    catch (Exception ex)
                    {
                        log.Info(ex);
                    }
                }
                cursor += size;
                lastCursor = oneTimeList.Max(o => o.Id);
            }

            sw.Stop();
            log.Info(string.Format("total paid orders:{0},{1} synced orders in {2} => {3} docs/s", totalCount, successCount, sw.Elapsed, successCount / sw.Elapsed.TotalSeconds));


        }

    }
}
