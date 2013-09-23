using com.intime.fashion.data.erp.Models;
using Common.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Configuration;
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
    class ProductSyncJob:IJob
    {
        private const   decimal NULL_PRICE = 99999999.00m;
        private static string DEFAULT_TAG_ID = ConfigurationManager.AppSettings["ERPSYN_DEFAULT_TAG"];

        private void DoQuery(Expression<Func<SUPPLY_MIN_PRICE, bool>> whereCondition, Action<IQueryable<SUPPLY_MIN_PRICE>> callback)
        {
            using (var context = new ErpContext())
            {
                var linq = context.SUPPLY_MIN_PRICE.AsQueryable();
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
            Expression<Func<SUPPLY_MIN_PRICE, bool>> whereCondition = null;
            if (!isRebuild)
                whereCondition = b => b.OPT_UPDATE_TIME >= benchTime;

            DoQuery(whereCondition, products =>
            {
                totalCount = products.Count();
            });
            int cursor = 0;
            int successCount = 0;
            int size = 100;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            while (cursor < Math.Min(totalCount,100))
            {
                List<SUPPLY_MIN_PRICE> oneTimeList = null;
                DoQuery(whereCondition, products =>
                {
                    oneTimeList = products.OrderBy(b => b.SID).Skip(cursor).Take(size).ToList();
                });
                foreach (var product in oneTimeList)
                {
                    SyncOne(product);
                    successCount++;
                }
                cursor += size;
            }

            sw.Stop();
            log.Info(string.Format("total products:{0},{1} ex brands in {2} => {3} docs/s", totalCount, successCount, sw.Elapsed, successCount / sw.Elapsed.TotalSeconds));


        }

        private static void  EnsureProductContext(SUPPLY_MIN_PRICE product)
        {
            using (var db = new YintaiHangzhouContext("YintaiHangzhouContext"))
            {
                var exBrand = db.Set<BrandEntity>().Where(b => b.ChannelBrandId == product.BRAND_SID).FirstOrDefault();
                if (null == exBrand)
                {
                    using(var erpDb = new ErpContext())
                    {
                        var brand = erpDb.Set<BRAND>().Find(product.BRAND_SID);
                        if (null != brand)
                          BrandSyncJob.SyncOne(brand);
                    }
                }
                var exCat = db.Set<CategoryEntity>().Where(c => c.ExCatId == product.PRO_CLASS_SID).FirstOrDefault();
                if (null == exCat)
                {
                    using (var erpDb = new ErpContext())
                    {
                        var cat = erpDb.Set<PRO_CLASS_DICT>().Find(product.PRO_CLASS_SID);
                        if (null!= cat)
                        CategorySyncJob.SyncOne(cat);
                    }
                }
                var exStore = db.Set<StoreEntity>().Where(c => c.ExStoreId == product.SHOP_SID).FirstOrDefault();
                if (null == exStore)
                {
                    using (var erpDb = new ErpContext())
                    {
                        var store = erpDb.Set<SHOP_INFO>().Find(product.SHOP_SID);
                        if (null != store)
                            StoreSyncJob.SyncOne(store);
                    }
                }
            }
        }
       public  static void  SyncOne(SUPPLY_MIN_PRICE product)
        {
            EnsureProductContext(product);
            using (var ts = new TransactionScope())
            {
                using (var db = new YintaiHangzhouContext("YintaiHangzhouContext"))
                {
                    var existProduct = db.Set<ProductMapEntity>().Where(b => b.ChannelPId == product.PRODUCT_SID).FirstOrDefault();
                    var tagEntity = db.Set<TagEntity>().Join(db.Set<CategoryMapEntity>().Where(cm=>cm.ChannelCatId==product.PRO_CLASS_SID),o=>o.Id,i=>i.CatId,(o,i)=>o)
                                    .FirstOrDefault();
                    var storeEntity = db.Set<StoreEntity>().Where(s=>s.ExStoreId== product.SHOP_SID).FirstOrDefault();
                    var brandEntity = db.Set<BrandEntity>().Where(b=>b.ChannelBrandId == product.BRAND_SID).FirstOrDefault();
                    if (existProduct == null)
                    {
                        var newProduct = new ProductEntity()
                        {
                            CreatedDate = DateTime.Now,
                            CreatedUser = 0,
                            SkuCode = product.PRO_SKU,
                            Brand_Id = brandEntity==null?0:brandEntity.Id,
                             Description = product.PRO_DESC??string.Empty,
                              Is4Sale = false,
                               Name = product.PRODUCT_NAME??string.Empty,
                                UnitPrice = product.ORIGINAL_PRICE??NULL_PRICE,
                                 RecommendedReason = product.PRO_DESC??string.Empty,
                                  RecommendUser = 0,
                                   SortOrder=0,
                                    Status=1,
                                     Store_Id = storeEntity==null?0:storeEntity.Id,
                                      Tag_Id =tagEntity==null?int.Parse(DEFAULT_TAG_ID):tagEntity.Id,
                                       Price = product.PROMOTION_PRICE??NULL_PRICE,
                                        UpdatedDate = product.OPT_UPDATE_TIME??DateTime.Now,
                                         UpdatedUser=0,
                                          BarCode = product.BARCODE,
                                          Favorable ="1"
                                         

                        };
                        db.Products.Add(newProduct);
                        db.SaveChanges();
                        db.ProductMaps.Add(new ProductMapEntity()
                        {
                            Channel = "ERP",
                             ChannelBrandId = (int)product.BRAND_SID,
                              ChannelPId = (int)product.PRODUCT_SID,
                               ChannelCatId = (int)product.PRO_CLASS_SID,
                                ProductId = newProduct.Id,
                                 UpdateDate = product.OPT_UPDATE_TIME??DateTime.Now
                        });
                    }
                    else
                    {
                        var existProductEntity = db.Set<ProductEntity>().Find(existProduct.ProductId);
                        existProductEntity.BarCode = product.BARCODE;
                        existProductEntity.UpdatedDate = product.OPT_UPDATE_TIME ?? DateTime.Now;
                        existProductEntity.Store_Id = storeEntity==null?0:storeEntity.Id;
                        existProductEntity.Brand_Id = brandEntity==null?0:brandEntity.Id;
                        existProductEntity.Tag_Id = tagEntity ==null?int.Parse(DEFAULT_TAG_ID):tagEntity.Id;
                        existProductEntity.SkuCode = product.PRO_SKU;
                        existProductEntity.Name = product.PRODUCT_NAME??string.Empty;
                        existProductEntity.UnitPrice = product.ORIGINAL_PRICE??NULL_PRICE;
                        existProductEntity.Price =product.PROMOTION_PRICE??NULL_PRICE;
                        existProductEntity.Description = product.PRO_DESC??string.Empty;
                        existProductEntity.RecommendedReason = product.PRO_DESC??string.Empty;
                       
                    }
                    db.SaveChanges();

                }
                ts.Complete();
            }
        }
    }
}
