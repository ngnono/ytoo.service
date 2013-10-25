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
    class BrandSyncJob:IJob
    {
          private void DoQuery(Expression<Func<BRAND,bool>> whereCondition, Action<IQueryable<BRAND>> callback)
        {
            using (var context = new ErpContext())
            { 
                var linq = context.BRANDs.AsQueryable();
                if (whereCondition != null)
                    linq = linq.Where( whereCondition);
                if (callback != null)
                    callback(linq);
            }
        }
        public void Execute(IJobExecutionContext context)
        {
            ILog log = LogManager.GetLogger(this.GetType());

            JobDataMap data = context.JobDetail.JobDataMap;
            var isRebuild = data.ContainsKey("isRebuild") ? data.GetBoolean("isRebuild") : false ;
            var interval = data.ContainsKey("intervalOfSecs") ? data.GetInt("intervalOfSecs") : 5 * 60;
            var totalCount = 0;
            var benchTime = DateTime.Now.AddSeconds(-interval);
            Expression<Func<BRAND,bool>> whereCondition = null;
            if (!isRebuild)
                whereCondition = b=>b.OPT_UPDATE_TIME>=benchTime;

                DoQuery(whereCondition, brands =>
                {
                    totalCount = brands.Count();
                });
            int cursor = 0;
            int successCount = 0;
           int size = JobConfig.DEFAULT_PAGE_SIZE;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            while (cursor < totalCount)
            {
                List<BRAND> oneTimeList = null;
                DoQuery(whereCondition, brands => {
                    oneTimeList = brands.OrderBy(b => b.SID).Skip(cursor).Take(size).ToList();
                });
                foreach(var brand in oneTimeList)
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
            log.Info(string.Format("total brands:{0},{1} ex brands in {2} => {3} docs/s",totalCount, successCount, sw.Elapsed, successCount / sw.Elapsed.TotalSeconds));

         
        }

        public static void SyncOne(BRAND brand)
        {
            using (var db = new YintaiHangzhouContext("YintaiHangzhouContext"))
            {
                var existBrand = db.Set<BrandEntity>().Where(b => b.ChannelBrandId == brand.SID).FirstOrDefault();
                if (existBrand == null)
                {
                    db.Brands.Add(new BrandEntity()
                    {
                        ChannelBrandId = (int)brand.SID,
                        CreatedDate = DateTime.Now,
                        CreatedUser = 0,
                        Name = brand.BRAND_NAME,
                        EnglishName = brand.BRAND_NAME_SECOND,
                        Logo = brand.PICTURE_URL??string.Empty,
                        Status = brand.BRAND_ACTIVE_BIT,
                        UpdatedDate = brand.OPT_UPDATE_TIME ?? DateTime.Now,
                        UpdatedUser = 0,
                        Description = brand.BRAND_STORY??string.Empty,
                        Group = (brand.BRAND_GROUP_NUM??0).ToString(),
                         WebSite = string.Empty

                    });
                }
                else
                {
                    existBrand.Name = brand.BRAND_NAME;
                    existBrand.EnglishName = brand.BRAND_NAME_SECOND;
                    existBrand.Logo = brand.PICTURE_URL??string.Empty;;
                    existBrand.Status = brand.BRAND_ACTIVE_BIT;
                    existBrand.UpdatedUser = 0;
                    existBrand.UpdatedDate = brand.OPT_UPDATE_TIME ?? DateTime.Now;
                    existBrand.Description = brand.BRAND_STORY??string.Empty;
                    existBrand.Group = (brand.BRAND_GROUP_NUM ?? 0).ToString();
                  
                }
                db.SaveChanges();

            }
        }
      
    }
}
