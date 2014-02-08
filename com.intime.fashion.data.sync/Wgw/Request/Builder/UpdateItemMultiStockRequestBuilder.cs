using com.intime.fashion.data.sync.Wgw.Request.Item;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Yintai.Hangzhou.Data.Models;

namespace com.intime.fashion.data.sync.Wgw.Request.Builder
{
    public class UpdateItemMultiStockRequestBuilder : RequestParamsBuilder
    {
        public UpdateItemMultiStockRequestBuilder(ISyncRequest request) : base(request)
        {
        }

        public override ISyncRequest BuildParameters(object entity)
        {
            var productId = (int)entity;
            return BuilderParamsters(productId);
        }

        private ISyncRequest BuilderParamsters(int productId)
        {
            Request.Put(ParamName.Param_ItemId,GetItemId(productId));
            using (var db = GetDbContext())
            {
                var product = db.Products.FirstOrDefault(t => t.Id == productId);
                this.BuildStockJsonStr(product);
            }
            return Request;
        }

        private string GetItemId(int productId)
        {
            using (var db = GetDbContext())
            {
                var map = db.Map4Products.FirstOrDefault(m => m.ProductId == productId && m.Channel == ConstValue.WGW_CHANNEL_NAME);
                return map == null?string.Empty:map.ChannelProductId;
            }
        }
        protected void BuildStockJsonStr(ProductEntity item)
        {
            using (var db = GetDbContext())
            {
                var inventories = db.Inventories.Where(i => i.ProductId == item.Id);

                var stocks = new List<dynamic>();
                foreach (var inventory in inventories)
                {
                    Map4Inventory inventoryMap = db.Map4Inventories.FirstOrDefault(
                        m => m.InventoryId == inventory.Id && m.Channel == "wgw" && m.ProductId == inventory.ProductId);
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
                        desc = saleAttr,
                        saleAttr,
                        attr,
                        specAttr = string.Empty,
                        status = (int)(item.Is4Sale.HasValue && item.Is4Sale.Value && item.Status == 1 ? StockStatus.IS_FOR_SALE : StockStatus.IS_IN_STORE),
                        itemId = inventoryMap == null ? null : inventoryMap.itemId,
                        skuId = inventoryMap == null ? 0 : inventoryMap.skuId,
                    });
                }

                if (stocks.Count > 0)
                {
                    Request.Put("newData", JsonConvert.SerializeObject(new { stockJsonStr = stocks }));
                }
            }

            //var stocks = inventories.Select(inventory => ConstructStockInfo(inventory, item)).ToList();
        }
    }
}
