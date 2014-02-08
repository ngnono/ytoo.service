using System.Text;
using com.intime.fashion.data.sync.Wgw.Request.Item;
using com.intime.jobscheduler.Job.Wgw;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Yintai.Hangzhou.Data.Models;

namespace com.intime.fashion.data.sync.Wgw.Request.Builder
{
    public class ItemRequestParamsBuilder : RequestParamsBuilder
    {
        public ItemRequestParamsBuilder(ISyncRequest request)
            : base(request)
        {

        }
        private ISyncRequest BuildParameters(ProductEntity entity)
        {
            this.BuildLeafClassId(entity.Id);
            this.BuildLabel(entity.Brand_Id);
            this.BuildUpTime();
            this.BuildProductImages(entity);
            this.BuildStockAndColorImage(entity);
            Request.Put("buyLimit", WgwConfigHelper.BuyLimit);
            Request.Put("city", WgwConfigHelper.City);
            Request.Put("price_bin", Math.Ceiling(entity.Price * 100));
            Request.Put("province", WgwConfigHelper.Province);
            Request.Put("marketPrice",
                entity.UnitPrice.HasValue ? Math.Ceiling(entity.UnitPrice.Value * 100) : int.MaxValue);
            Request.Put("defStockId", entity.Id);
            Request.Put("title", entity.Name);
            Request.Put("sendType", "0");
            Request.Put("transportType", "1");
            Request.Put("desc", entity.Description);
            return Request;
        }

        /// <summary>
        /// 构建库存和颜色图片等参数
        /// </summary>
        /// <param name="item"></param>
        protected void BuildStockAndColorImage(ProductEntity item)
        {
            using (var db = GetDbContext())
            {
                var inventories = db.Inventories.Where(i => i.ProductId == item.Id);
                var stocks = new List<dynamic>();
                var images = new List<dynamic>();
                
                foreach (var inventory in inventories)
                {
                    Map4Inventory inventoryMap = db.Map4Inventories.FirstOrDefault(
                        m => m.InventoryId == inventory.Id && m.Channel == ConstValue.WGW_CHANNEL_NAME && m.ProductId == inventory.ProductId);
                    var colorEntity = db.ProductPropertyValues.FirstOrDefault(t => t.Id == inventory.PColorId);
                    var sizeEntity = db.ProductPropertyValues.FirstOrDefault(t => t.Id == inventory.PSizeId);
                    var color = db.ProductProperties.FirstOrDefault(t => t.Id == colorEntity.PropertyId);
                    var size = db.ProductProperties.FirstOrDefault(t => t.Id == sizeEntity.PropertyId);

                    if (colorEntity == null || color == null || sizeEntity == null || size == null)
                    {
                        continue;
                    }

                    var saleAttr = string.Format("{0}:{1}|{2}:{3}",
                        colorEntity.PropertyId,
                        colorEntity.Id,
                        sizeEntity.PropertyId,
                        sizeEntity.Id);

                    var attr = string.Format("{0}:{1}|{2}:{3}",
                        color.PropertyDesc,
                        colorEntity.ValueDesc,
                        size.PropertyDesc,
                        sizeEntity.ValueDesc);
                    stocks.Add(new
                    {
                        stockId = inventory.Id,
                        price = (int)Math.Ceiling(item.Price * 100),
                        num = inventory.Amount,
                        desc = attr,
                        saleAttr,
                        attr,
                        specAttr = string.Empty,
                        status = (int)(item.Is4Sale.HasValue && item.Is4Sale.Value && item.Status == 1 ? StockStatus.IS_FOR_SALE : StockStatus.IS_IN_STORE),
                        itemId = inventoryMap == null ? null : inventoryMap.itemId,
                        skuId = inventoryMap == null ? 0 : inventoryMap.skuId,
                    });
                    
                    if (images.Any(x=>x.value == colorEntity.ValueDesc))
                    {
                        continue; 
                    }
                    var img = db.Resources.FirstOrDefault(r => r.SourceId == inventory.ProductId && r.ColorId == inventory.PColorId);

                    if (img == null)
                    {
                        continue;
                    }
                    images.Add(new { property = color.PropertyDesc, value = colorEntity.ValueDesc,url = string.Format("{0}/{1}.{2}",WgwConfigHelper.Image_BaseUrl, img.Name, img.ExtName) });

                }
#if !DEBUG
                var sb = new StringBuilder();
                var colorImgStr = images.Aggregate(sb, (s, img) => sb.AppendFormat("{0}:{1}|{2};", img.property, img.value, img.url),
                   imgs => sb.ToString());

                if (colorImgStr.Length > 0)
                {
                    Request.Put("stockAttrImgs", colorImgStr.Substring(0, colorImgStr.Length - 1));
                }
#else
                Request.Put("stockAttrImgs", "颜色:花色|http://ec4.images-amazon.com/images/I/51%2B6RQwwhkL._SS45_.jpg;颜色:黑色|http://g-ec4.images-amazon.com/images/G/28/fanting_3P/shoes/123._SS75_V363140790_.jpg;颜色:绿色|http://ec4.images-amazon.com/images/I/41NZt4fsPrL._SS45_.jpg");
#endif
                if (stocks.Count > 0)
                {
                    Request.Put("stockstr", JsonConvert.SerializeObject(new { stockJsonStr = stocks }));
                }
                Request.Put("num", inventories.Any() ? inventories.Sum(q => q.Amount) : 0);
            }
        }

        /// <summary>
        /// 构建商品图片参数
        /// </summary>
        /// <param name="entity"></param>
        protected virtual void BuildProductImages(ProductEntity entity)
        {
#if !DEBUG
            using (var db = GetDbContext())
            {
                var img =
                    db.Resources.Where(t => t.SourceId == entity.Id && t.Type == 1)
                        .OrderByDescending(t => t.IsDefault)
                        .Take(1).FirstOrDefault();
                if (img == null)
                {
                    throw new WgwSyncException(string.Format("商品 {0} ({1}) 没有图片",entity.Name,entity.Id));
                }
                Request.Put("uploadPicInfo1", string.Format("{0}/{1}.{2}", WgwConfigHelper.Image_BaseUrl, img.Name, img.ExtName));
            }
#else
            Request.Put("uploadPicInfo1", "http://ec4.images-amazon.com/images/I/71oMNPbikLL._AA1500_.jpg");
#endif
        }

        /// <summary>
        /// 构建上架日期参数
        /// </summary>
        private void BuildUpTime()
        {
            var st = new DateTime(1970, 1, 1);
            TimeSpan t = (DateTime.Now.AddDays(WgwConfigHelper.UpDelay).ToUniversalTime() - st);
            Request.Put("upTime",((Int64)(t.TotalMilliseconds)).ToString("D"));
        }

        /// <summary>
        /// 构建微购物叶级工业分类Id参数
        /// </summary>
        /// <param name="productId"></param>
        private void BuildLeafClassId(int productId)
        {
            using (var db = GetDbContext())
            {
                var cat =
                    db.Categories.Join(db.ProductMaps.Where(pm => pm.ProductId == productId), o => o.ExCatId,
                        m => m.ChannelCatId, (o, m) => o).FirstOrDefault();
                if (cat == null)
                {
                    throw new WgwSyncException(string.Format("商品分类不存在"));
                }
                var mapping = db.Map4Categories.FirstOrDefault(ccm => ccm.CategoryCode == cat.ExCatCode);
                if (mapping == null)
                {
                    throw new WgwSyncException(string.Format("未映射微购物商品分类{0}({1})", cat.Name, cat.ExCatCode));
                }

                Request.Put("leafClassId",mapping.ChannelCategoryId);
            }
        }
        
        /// <summary>
        /// 构建商品标签参数，当前只支持商品品牌标签
        /// </summary>
        /// <param name="brandId"></param>
        private void BuildLabel(int brandId)
        {
            using (var db = GetDbContext())
            {
                var map = db.Map4Brands.FirstOrDefault(b => b.BrandId == brandId);
                if (map == null)
                {
                    throw new WgwSyncException(string.Format("未映射商品品牌({0})", brandId));
                }
                Request.Put("lable", string.Format("brands|{0}", map.ChannelBrandId));
            }
        }

        public override ISyncRequest BuildParameters(object entity)
        {
            var item = entity as ProductEntity;
            if (item != null)
            {
                return BuildParameters(item);
            }
            throw new WgwSyncException("参数类型应为ProductEntity");
        }
    }
}