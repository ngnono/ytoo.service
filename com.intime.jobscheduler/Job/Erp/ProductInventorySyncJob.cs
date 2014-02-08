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
using System.Transactions;
using Yintai.Hangzhou.Data.Models;

namespace com.intime.jobscheduler.Job.Erp
{
    [DisallowConcurrentExecution]
    class ProductInventorySyncJob:IJob
    {
        private void DoQuery(Expression<Func<SUPPLY_MIN_PRICE_MX, bool>> whereCondition, Action<IQueryable<SUPPLY_MIN_PRICE_MX>> callback)
        {
            using (var context = new ErpContext())
            {
                var linq = context.SUPPLY_MIN_PRICE_MX.AsQueryable();
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
            var benchTime = data.GetDateTime("benchtime");
            Expression<Func<SUPPLY_MIN_PRICE_MX, bool>> whereCondition = null;
            if (!isRebuild)
                whereCondition = b => b.OPT_UPDATE_TIME >= benchTime;

            DoQuery(whereCondition, products =>
            {
                totalCount = products.Count();
            });
            int cursor = 0;
            int successCount = 0;
           int size = JobConfig.DEFAULT_PAGE_SIZE;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            while (cursor < totalCount)
            {
                List<SUPPLY_MIN_PRICE_MX> oneTimeList = null;
                DoQuery(whereCondition, products =>
                {
                    oneTimeList = products.OrderBy(b => b.SID).Skip(cursor).Take(size).ToList();
                });
                foreach (var product in oneTimeList)
                {
                    try
                    {
                        SyncOne(product);
                        successCount++;
                    }
                    catch (Exception ex)
                    {
                        log.Error(string.Format("{0} product inventory error", product.PRO_DETAIL_SID));
                        log.Error(ex);
                    }
                }
                cursor += size;
            }

            sw.Stop();
            log.Info(string.Format("total product inventory:{0},{1} ex products in {2} => {3} docs/s", totalCount, successCount, sw.Elapsed, successCount / sw.Elapsed.TotalSeconds));


        }

        private static bool EnsureProductContext(SUPPLY_MIN_PRICE_MX product)
        {
            bool shouldSyncProduct = false;
            using (var db = new YintaiHangzhouContext("YintaiHangzhouContext"))
            {

                var colorEntity = db.Set<ProductPropertyValueEntity>().Where(ppv => ppv.ChannelValueId == product.PRO_COLOR_SID)
                                    .Join(db.Set<ProductPropertyEntity>()
                                            .Join(db.Set<ProductMapEntity>().Where(pm => pm.ChannelPId == product.PRODUCT_SID), o => o.ProductId, i => i.ProductId, (o, i) => o)
                                     ,o => o.PropertyId, i => i.Id, (o, i) => o).FirstOrDefault();
                var sizeEntity = db.Set<ProductPropertyValueEntity>().Where(ppv => ppv.ChannelValueId == product.PRO_STAN_SID)
                                    .Join(db.Set<ProductPropertyEntity>()
                                            .Join(db.Set<ProductMapEntity>().Where(pm => pm.ChannelPId == product.PRODUCT_SID)
                                                   , o => o.ProductId, i => i.ProductId, (o, i) => o),
                                            o => o.PropertyId, i => i.Id, (o, i) => o).FirstOrDefault();
                if (colorEntity == null || sizeEntity == null)
                    shouldSyncProduct = true;
                
                if (shouldSyncProduct)
                   return ProductPropertySyncJob.SyncOne(product.PRODUCT_SID, product.PRO_STAN_SID ?? 0, product.PRO_STAN_NAME, product.PRO_COLOR_SID ?? 0, product.PRO_COLOR);
            }
            return true;
        }
        public static void SyncOne(SUPPLY_MIN_PRICE_MX product)
        {
            if (!EnsureProductContext(product))
                return;
            using (var ts = new TransactionScope())
            {
                using (var db = new YintaiHangzhouContext("YintaiHangzhouContext"))
                {
                    var existProduct = db.Set<ProductEntity>().Join(db.Set<ProductMapEntity>().Where(ep => ep.ChannelPId == product.PRODUCT_SID), o => o.Id, i => i.ProductId, (o, i) => o).FirstOrDefault();
                    var color = db.Set<ProductPropertyValueEntity>().Where(b => b.ChannelValueId == product.PRO_COLOR_SID)
                                .Join(db.Set<ProductPropertyEntity>().Where(pp => pp.ProductId == existProduct.Id), o => o.PropertyId, i => i.Id, (o, i) => o).FirstOrDefault();
                    var size = db.Set<ProductPropertyValueEntity>().Where(b => b.ChannelValueId == product.PRO_STAN_SID)
                                .Join(db.Set<ProductPropertyEntity>().Where(pp => pp.ProductId == existProduct.Id), o => o.PropertyId, i => i.Id, (o, i) => o).FirstOrDefault();
                    var inventory = db.Set<InventoryEntity>().Where(i => i.ChannelInventoryId == product.PRO_DETAIL_SID).FirstOrDefault();
                    int amount = (int)product.PRO_STOCK_SUM;
                    bool no4sale = product.PRO_ACTIVE_BIT.GetValueOrDefault()==0;
                    if (no4sale)
                        amount = 0;
                    if (inventory == null)
                    {
                        db.Inventories.Add(new InventoryEntity()
                        {
                            ProductId = existProduct.Id,
                            PColorId = color.Id,
                            PSizeId = size.Id,
                            UpdateDate = DateTime.Now,
                            UpdateUser = 0,
                            Amount = amount,
                            ChannelInventoryId = (int)product.PRO_DETAIL_SID
                        });
                    }
                    else
                    {
                        inventory.Amount = amount;
                        inventory.UpdateDate = DateTime.Now;
                        db.Entry(inventory).State = System.Data.EntityState.Modified;
                    }
                    //update product.is4sale
                    if ((existProduct.Is4Sale ?? false) == false && !no4sale)
                    {
                        existProduct.Is4Sale = true;
                        existProduct.UpdatedDate = DateTime.Now;
                        db.Entry(existProduct).State = System.Data.EntityState.Modified;
                    }
                    db.SaveChanges();

                }
                ts.Complete();
            }
        }
    }
}
