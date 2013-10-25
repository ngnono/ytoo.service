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
using Yintai.Hangzhou.Model.Enums;

namespace com.intime.jobscheduler.Job.Erp
{
    [DisallowConcurrentExecution]
    class SectionSyncJob : IJob
    {
        private void DoQuery(Expression<Func<SALE_CODE, bool>> whereCondition, Action<IQueryable<SALE_CODE>> callback)
        {
            using (var context = new ErpContext())
            {
                var linq = context.SALE_CODE.AsQueryable();
                if (whereCondition != null)
                    linq = linq.Where(whereCondition);
                if (callback != null)
                    callback(linq);
            }
        }
        private static void EnsureSectionContext(SALE_CODE section)
        {
            using (var db = new YintaiHangzhouContext("YintaiHangzhouContext"))
            {
                var entity = db.Set<StoreEntity>().Where(b => b.ExStoreId == section.SHOP_SID).FirstOrDefault();
                if (null == entity)
                {
                    StoreSyncJob.SyncOne(section.SHOP_INFO);
                }

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
            Expression<Func<SALE_CODE, bool>> whereCondition = null;
            if (!isRebuild)
                whereCondition = b => b.OPT_UPDATE_TIME >= benchTime;

            DoQuery(whereCondition, sections =>
            {
                totalCount = sections.Count();
            });
            int cursor = 0;
            int successCount = 0;
           int size = JobConfig.DEFAULT_PAGE_SIZE;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            while (cursor < totalCount)
            {
                List<SALE_CODE> oneTimeList = null;
                DoQuery(whereCondition, sections =>
                {
                    oneTimeList = sections.OrderBy(b => b.SID).Skip(cursor).Take(size).ToList();
                });
                foreach (var brand in oneTimeList)
                {
                    try
                    {
                        SyncOne(brand);
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
            log.Info(string.Format("total brands:{0},{1} ex brands in {2} => {3} docs/s", totalCount, successCount, sw.Elapsed, successCount / sw.Elapsed.TotalSeconds));


        }

        public static void SyncOne(SALE_CODE section)
        {
            EnsureSectionContext(section);
            using (var db = new YintaiHangzhouContext("YintaiHangzhouContext"))
            {
                var sectionId = section.SID;
                var existBrand = db.Set<SectionEntity>().Where(b => b.ChannelSectionId == sectionId).FirstOrDefault();
                var storeEntity = db.Set<StoreEntity>().Where(s => s.ExStoreId == section.SHOP_SID).FirstOrDefault();
                if (existBrand == null)
                {
                    db.Sections.Add(new SectionEntity()
                    {
                        ChannelSectionId = (int)section.SID,
                        CreateDate = DateTime.Now,
                        CreateUser = 0,
                        Location = section.ADDRESS ?? string.Empty,
                        Name = section.SALE_CODE_NAME ?? string.Empty,
                        ContactPhone = string.Empty,
                        StoreId = storeEntity.Id,
                        Status = (int)DataStatus.Normal,
                        UpdateDate = DateTime.Now,
                        UpdateUser = 0

                    });
                }
                else
                {
                    existBrand.Name = section.SALE_CODE_NAME ?? string.Empty;
                    existBrand.UpdateDate = section.OPT_UPDATE_TIME ?? DateTime.Now;
                    existBrand.Location = section.ADDRESS ?? string.Empty;
                    existBrand.StoreId = storeEntity.Id;

                }
                db.SaveChanges();

            }

        }

    }
}
