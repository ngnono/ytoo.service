using com.intime.fashion.data.erp.Models;
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

namespace com.intime.jobscheduler.Job.Erp
{
    [DisallowConcurrentExecution]
    class SaleProductNoPicSyncOneTimeJob:IJob
    {
        private void Query(Action<IQueryable<ProductMapEntity>> callback)
        {
            using (var db = new YintaiHangzhouContext("YintaiHangzhouContext"))
            {
                var pms = db.Set<ProductMapEntity>()
                          .Join(db.Set<ProductEntity>().Where(p=>p.Status==1),o=>o.ProductId,i=>i.Id,(o,i)=>o)
                          .Where(pm => !db.Set<ResourceEntity>().Where(r => r.SourceType == (int)SourceType.Product).Any(r => r.SourceId == pm.ProductId));

                if (callback != null)
                    callback(pms);
            }
        }
        public void Execute(IJobExecutionContext context)
        {
            ILog log = LogManager.GetLogger(this.GetType());
            var totalCount = 0;

            Query( orders => totalCount = orders.Count());

            int cursor = 0;
            int successCount = 0;
            int size = JobConfig.DEFAULT_PAGE_SIZE;
            int lastCursor = 0;
            log.Info("begin job:SaleProductNoPicSyncOneTime");
            Stopwatch sw = new Stopwatch();
            sw.Start();
            while (cursor < totalCount)
            {
                List<ProductMapEntity> oneTimeList = null;
                Query(orders =>
                {
                    oneTimeList = orders.Where(a => a.Id > lastCursor).OrderBy(a => a.Id).Take(size).ToList();
                });
                foreach (var order in oneTimeList)
                {
                    try
                    {
                        IEnumerable<PRO_PICTURE> pics = null;
                        using (var erp = new ErpContext())
                        {
                          pics=  erp.PRO_PICTURE.Where(p => p.PRODUCT_SID == order.ChannelPId).ToList();
                          
                        }
                        if (pics == null || pics.Count() <= 0)
                            continue;
                        bool isSuccess = false;
                        foreach (var pic in pics)
                            isSuccess = ProductPicSyncJob.SyncOne(pic);
                       if (isSuccess)
                         successCount++;


                    }
                    catch (Exception ex)
                    {
                        log.Info(ex);
                    }
                }
                cursor += size;
                if (oneTimeList !=null && oneTimeList.Count()> 0)
                    lastCursor = oneTimeList.Max(o => o.Id);
            }

            sw.Stop();
            log.Info(string.Format("total no pic products :{0},{1} synced products in {2} => {3} docs/s", totalCount, successCount, sw.Elapsed, successCount / sw.Elapsed.TotalSeconds));
            log.Info("end job:SaleProductNoPicSyncOneTime");

        }
    }
}
