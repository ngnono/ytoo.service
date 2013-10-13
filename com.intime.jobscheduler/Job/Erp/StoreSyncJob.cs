using com.intime.fashion.data.erp.Models;
using Common.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Yintai.Hangzhou.Data.Models;

namespace com.intime.jobscheduler.Job.Erp
{
    [DisallowConcurrentExecution]
    class StoreSyncJob:IJob
    {
        private void DoQuery(Expression<Func<SHOP_INFO, bool>> whereCondition, Action<IQueryable<SHOP_INFO>> callback)
        {
            using (var context = new ErpContext())
            {
                var linq = context.SHOP_INFO.AsQueryable();
                if (whereCondition != null)
                    linq = linq.Where(whereCondition);
                if (callback != null)
                    callback(linq);
            }
        }
        public void Execute(IJobExecutionContext context)
        {
            ILog log = LogManager.GetLogger(this.GetType());

            JobDataMap data = context.JobDetail.JobDataMap;
            var isRebuild = data.ContainsKey("isRebuild") ? data.GetBoolean("isRebuild") : false;
            var interval = data.ContainsKey("intervalOfSecs") ? data.GetInt("intervalOfSecs") : 5 * 60;
            var totalCount = 0;
            var benchTime = DateTime.Now.AddSeconds(-interval);
            Expression<Func<SHOP_INFO, bool>> whereCondition = null;
            if (!isRebuild)
                whereCondition = b => b.OPT_UPDATE_TIME >= benchTime;

            DoQuery(whereCondition, brands =>
            {
                totalCount = brands.Count();
            });
            int cursor = 0;
            int successCount = 0;
            int size = 100;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            while (cursor < totalCount)
            {
                List<SHOP_INFO> oneTimeList = null;
                DoQuery(whereCondition, stores =>
                {
                    oneTimeList = stores.OrderBy(b => b.SID).Skip(cursor).Take(size).ToList();
                });
                foreach (var store in oneTimeList)
                {
                    try
                    {
                        SyncOne(store);
                        successCount++;
                    }
                    catch (Exception ex)
                    {
                        log.Info(ex);
                    }
                }
                cursor += size;
            }

            sw.Stop();
            log.Info(string.Format("total stores:{0},{1} ex brands in {2} => {3} docs/s", totalCount, successCount, sw.Elapsed, successCount / sw.Elapsed.TotalSeconds));


        }
        public static void SyncOne(SHOP_INFO store)
        {
            using (var db = new YintaiHangzhouContext("YintaiHangzhouContext"))
            {
                var existStore = db.Set<StoreEntity>().Where(b => b.ExStoreId == store.SID).FirstOrDefault();
                if (existStore == null)
                {
                    db.Stores.Add(new StoreEntity()
                    {

                        CreatedDate = DateTime.Now,
                        CreatedUser = 0,
                        ExStoreId = (int)store.SID,
                        Description = string.Empty,
                        Location = store.SHOP_ADDR??string.Empty,
                        Name = store.SHOP_NAME,
                        RMAAddress = store.SHOP_ADDR,
                        RMAPerson = store.REFUND_LINKER,
                        RMAPhone = store.REFUND_TEL,
                        RMAZipCode = store.POSTCODE.HasValue?store.POSTCODE.ToString():string.Empty,
                        UpdatedDate = store.OPT_UPDATE_TIME ?? DateTime.Now,
                        UpdatedUser = 0,
                        Status = 1,
                        Tel = store.LINKER_PHONE??string.Empty

                    });
                }
                else
                {
                    existStore.RMAZipCode = store.POSTCODE.HasValue ? store.POSTCODE.ToString() : string.Empty;
                    existStore.RMAPhone = store.REFUND_TEL;
                    existStore.RMAAddress = store.SHOP_ADDR;
                    existStore.RMAPerson = store.REFUND_LINKER;
                    existStore.UpdatedDate = store.OPT_UPDATE_TIME ?? DateTime.Now;
                    existStore.Location = store.SHOP_ADDR??string.Empty;
                    existStore.Tel = store.LINKER_PHONE ?? string.Empty;
                    existStore.Name = store.SHOP_NAME;
                }
                db.SaveChanges();

            }
        }
    }
}
