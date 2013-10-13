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
    class CategorySyncJob:IJob
    {
        private void DoQuery(Expression<Func<PRO_CLASS_DICT, bool>> whereCondition, Action<IQueryable<PRO_CLASS_DICT>> callback)
        {
            using (var context = new ErpContext())
            {
                var linq = context.PRO_CLASS_DICT.Where(l=>l.PRO_CLASS_BIT==1);
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
            Expression<Func<PRO_CLASS_DICT, bool>> whereCondition = null;
            if (!isRebuild)
                whereCondition = b => b.OPT_UPDATE_TIME >= benchTime;

            DoQuery(whereCondition, cats =>
            {
                totalCount = cats.Count();
            });
            int cursor = 0;
            int successCount = 0;
            int size = 100;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            while (cursor < totalCount)
            {
                List<PRO_CLASS_DICT> oneTimeList = null;
                DoQuery(whereCondition, cats =>
                {
                    oneTimeList = cats.OrderBy(b => b.SID).Skip(cursor).Take(size).ToList();
                });
                foreach (var cat in oneTimeList)
                {
                    try
                    {
                        SyncOne(cat);
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
            log.Info(string.Format("total categories:{0},{1} ex category in {2} => {3} docs/s", totalCount, successCount, sw.Elapsed, successCount / sw.Elapsed.TotalSeconds));


        }
        public static void SyncOne(PRO_CLASS_DICT cat)
        {
            using (var db = new YintaiHangzhouContext("YintaiHangzhouContext"))
            {
                var existCat = db.Set<CategoryEntity>().Where(b => b.ExCatId == cat.SID).FirstOrDefault();
                if (existCat == null)
                {
                    db.Categories.Add(new CategoryEntity()
                    {
                        ExCatCode = cat.PRO_CLASS_NUM,
                        ExCatId = (int)cat.SID,
                        Name = cat.PRO_CLASS_DESC,
                        Status = 1,
                        UpdateDate = cat.OPT_UPDATE_TIME ?? DateTime.Now

                    });
                }
                else
                {
                    existCat.ExCatCode = cat.PRO_CLASS_NUM;
                    existCat.Name = cat.PRO_CLASS_DESC;
                    existCat.UpdateDate = cat.OPT_UPDATE_TIME ?? DateTime.Now;
                }
                db.SaveChanges();

            }
        }
    }
}
