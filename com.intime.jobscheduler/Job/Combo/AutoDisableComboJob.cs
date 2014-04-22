using Common.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Yintai.Hangzhou.Data.Models;
using Yintai.Hangzhou.Model.Enums;
using Yintai.Hangzhou.Service.Logic;

namespace com.intime.jobscheduler.Job.Combo
{
    [DisallowConcurrentExecution]
    class AutoDisableComboJob:IJob
    {
        private void Query(DateTime benchTime, Action<IQueryable<IMS_ComboEntity>> callback)
        {
            using (var db = new YintaiHangzhouContext("YintaiHangzhouContext"))
            {
                var orders = db.Set<IMS_ComboEntity>().Where(ot => ot.Status == (int)DataStatus.Normal
                            && ot.ExpireDate < benchTime);

                if (callback != null)
                    callback(orders);
            }
        }
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

            Query(benchTime, orders => totalCount = orders.Count());

            int cursor = 0;
            int successCount = 0;
            int size = JobConfig.DEFAULT_PAGE_SIZE;
            int lastCursor = 0;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            while (cursor < totalCount)
            {
                List<IMS_ComboEntity> oneTimeList = null;
                Query(benchTime, orders =>
                {
                    oneTimeList = orders.Where(a => a.Id > lastCursor).OrderBy(a => a.Id).Take(size).ToList();
                });
                foreach (var order in oneTimeList)
                {
                    try
                    {
                        using (var ts = new TransactionScope())
                        {
                            using (var db = new YintaiHangzhouContext("YintaiHangzhouContext"))
                            {
                                order.Status = (int)DataStatus.Default;
                                order.UpdateDate = DateTime.Now;
                                db.Entry(order).State = System.Data.EntityState.Modified;

                                var associateItemEntity = db.Set<IMS_AssociateItemsEntity>().Where(ia => ia.ItemType == (int)ComboType.Product
                                                && ia.ItemId == order.Id
                                                && ia.Status == (int)DataStatus.Normal).FirstOrDefault();
                                if (associateItemEntity != null)
                                {
                                    associateItemEntity.Status = (int)DataStatus.Default;
                                    associateItemEntity.UpdateDate = DateTime.Now;
                                    db.Entry(associateItemEntity).State = System.Data.EntityState.Modified;
                                }
                                db.SaveChanges();

                                ts.Complete();
                                
                                successCount++;
                                
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        log.Info(ex);
                    }
                }
                cursor += size;
                if (oneTimeList != null && oneTimeList.Count > 0)
                    lastCursor = oneTimeList.Max(o => o.Id);
            }

            sw.Stop();
            log.Info(string.Format("total combo can be disabled:{0},{1} disabled combos in {2} => {3} docs/s", totalCount, successCount, sw.Elapsed, successCount / sw.Elapsed.TotalSeconds));
        }
    }
}
