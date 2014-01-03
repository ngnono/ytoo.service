using Common.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yintai.Hangzhou.Data.Models;
using Yintai.Hangzhou.Model.Enums;

namespace com.intime.jobscheduler.Job.Order
{
    [DisallowConcurrentExecution]
    class AutoVoidUnpaidOrderJob:IJob
    {
        private void Query(DateTime benchTime, Action<IQueryable<OrderEntity>> callback)
        {
            using (var db = new YintaiHangzhouContext("YintaiHangzhouContext"))
            {
                var orders = db.Set<OrderEntity>().Where(ot => ot.Status == (int)OrderStatus.Create 
                            && ot.CreateDate < benchTime);

                if (callback != null)
                    callback(orders);
            }
        }
        public void Execute(IJobExecutionContext context)
        {
            ILog log = LogManager.GetLogger(this.GetType());

            JobDataMap data = context.JobDetail.JobDataMap;
            var interval = data.ContainsKey("intervalOfHrs") ? data.GetInt("intervalOfHrs") : 1;
            var totalCount = 0;
            var benchTime = DateTime.Now.AddHours(-interval);

            Query(benchTime, orders => totalCount = orders.Count());

            int cursor = 0;
            int successCount = 0;
            int size = JobConfig.DEFAULT_PAGE_SIZE;
            int lastCursor = 0;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            while (cursor < totalCount)
            {
                List<OrderEntity> oneTimeList = null;
                Query(benchTime, orders =>
                {
                    oneTimeList = orders.Where(a => a.Id > lastCursor).OrderBy(a => a.Id).Take(size).ToList();
                });
                foreach (var order in oneTimeList)
                {
                    try
                    {
                        using (var db = new YintaiHangzhouContext("YintaiHangzhouContext"))
                        {
                            order.Status = (int)OrderStatus.Void;
                            order.UpdateDate = DateTime.Now;
                            db.Entry(order).State = System.Data.EntityState.Modified;

                            db.OrderLogs.Add(new OrderLogEntity() { 
                                 CreateDate = DateTime.Now,
                                  CreateUser = 0,
                                   CustomerId = order.CreateUser,
                                    Operation="过期未支付，自动取消",
                                     OrderNo = order.OrderNo,
                                      Type=(int)OrderOpera.SystemVoid
                            });
                            db.SaveChanges();
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

            sw.Stop();
            log.Info(string.Format("total voiding orders:{0},{1} voided orders in {2} => {3} docs/s", totalCount, successCount, sw.Elapsed, successCount / sw.Elapsed.TotalSeconds));
        }
    }
}
