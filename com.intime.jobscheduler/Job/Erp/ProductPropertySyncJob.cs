using com.intime.fashion.data.erp.Models;
using Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yintai.Hangzhou.Data.Models;

namespace com.intime.jobscheduler.Job.Erp
{
    class ProductPropertySyncJob
    {
        public static bool SyncOne(decimal pid, decimal sizeId,string sizeName, decimal colorId,string colorName)
        {
            if (!EnsureProductContext(pid))
                return false;
            if (sizeId == 0 || colorId == 0)
            {
                Log.Error(string.Format("product sid:{0} with empty color sid or size sid",pid));
                return false;
            }

            using (var db = new YintaiHangzhouContext("YintaiHangzhouContext"))
            {
                var product = db.Set<ProductMapEntity>().Where(p=>p.ChannelPId == pid).FirstOrDefault();
                if (product == null)
                    return false; ;
                var existColor = db.Set<ProductPropertyValueEntity>().Where(b =>b.ChannelValueId == colorId)
                                .Join(db.Set<ProductPropertyEntity>().Where(pp=>pp.ProductId==product.ProductId),o=>o.PropertyId,i=>i.Id,(o,i)=>o)
                                .FirstOrDefault();
                if (existColor == null)
                {
                    var colorEntity = db.Set<ProductPropertyEntity>().Where(p => p.ProductId == product.ProductId && p.IsColor.HasValue && p.IsColor.Value == true).FirstOrDefault();
                    if (colorEntity == null)
                    {
                        colorEntity = db.ProductProperties.Add(new ProductPropertyEntity()
                        {
                            IsColor = true,
                            ChannelPropertyId = 0,
                            ProductId = product.ProductId,
                            PropertyDesc = "颜色",
                            SortOrder = 0,
                            Status = 1,
                            UpdateDate = DateTime.Now,
                            UpdateUser = 0

                        });
                        db.SaveChanges();
                    }
                    db.ProductPropertyValues.Add(new ProductPropertyValueEntity() { 
                         ChannelValueId = (int)colorId,
                          PropertyId = colorEntity.Id,
                           CreateDate = DateTime.Now,
                            Status =1,
                            UpdateDate  = DateTime.Now,
                             ValueDesc = colorName
                    });
                }
                else
                {
                    existColor.ValueDesc = colorName;
                    existColor.UpdateDate = DateTime.Now;
                    
                }
                var existSize = db.Set<ProductPropertyValueEntity>().Where(b => b.ChannelValueId == sizeId)
                                .Join(db.Set<ProductPropertyEntity>().Where(pp => pp.ProductId == product.ProductId), o => o.PropertyId, i => i.Id, (o, i) => o).FirstOrDefault();
                if (existSize == null)
                {
                    var sizeEntity = db.Set<ProductPropertyEntity>().Where(p => p.ProductId == product.ProductId && p.IsSize.HasValue && p.IsSize.Value == true).FirstOrDefault();
                    if (sizeEntity == null)
                    {
                        sizeEntity = db.ProductProperties.Add(new ProductPropertyEntity()
                        {
                            IsSize = true,
                            ChannelPropertyId = 0,
                            ProductId = product.ProductId,
                            PropertyDesc = "尺码",
                            SortOrder = 0,
                            Status = 1,
                            UpdateDate = DateTime.Now,
                            UpdateUser = 0

                        });
                        db.SaveChanges();
                    }
                    db.ProductPropertyValues.Add(new ProductPropertyValueEntity()
                    {
                        ChannelValueId = (int)sizeId,
                        PropertyId = sizeEntity.Id,
                        CreateDate = DateTime.Now,
                        Status = 1,
                        UpdateDate = DateTime.Now,
                        ValueDesc = sizeName
                    });
                }
                else
                {
                    existSize.ValueDesc = sizeName;
                    existSize.UpdateDate = DateTime.Now;
                }
                db.SaveChanges();

            }
            return true;
        }

        private static bool EnsureProductContext(decimal pid)
        {
            using (var db = new YintaiHangzhouContext("YintaiHangzhouContext"))
            {
                var productEntity = db.Set<ProductMapEntity>().Where(b => b.ChannelPId == pid).FirstOrDefault();
                if (null == productEntity)
                {
                    using (var erpDb = new ErpContext())
                    {
                        var exProduct = erpDb.Set<SUPPLY_MIN_PRICE>().Where(ep => ep.PRODUCT_SID == pid).FirstOrDefault();
                        if (null != exProduct)
                           return ProductSyncJob.SyncOne(exProduct);
                    }

                }
                return true;
            }
        }
        private static ILog Log
        {
            get
            {
                return LogManager.GetLogger(typeof(ProductSyncJob));
            }
        }
    }
}
