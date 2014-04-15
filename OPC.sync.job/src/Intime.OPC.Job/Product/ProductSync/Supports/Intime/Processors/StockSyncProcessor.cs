using System;
using System.Linq;

using Common.Logging;
using Intime.OPC.Domain.Models;

namespace Intime.OPC.Job.Product.ProductSync.Supports.Intime.Processors
{
    /// <summary>
    /// 库存同步处理器
    /// </summary>
    public class StockSyncProcessor : IStockSyncProcessor
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();
        private readonly ISectionSyncProcessor _sectionSyncProcessor;
        private readonly IChannelMapper _channelMapper;

        public StockSyncProcessor(ISectionSyncProcessor sectionSyncProcessor, IChannelMapper channelMapper)
        {
            _sectionSyncProcessor = sectionSyncProcessor;
            _channelMapper = channelMapper;
        }

        public OPC_Stock Sync(int skuId, string channelSectionId, string channelStoreNo, int channelCount, decimal channelPrice, string sourceStockId, string productSaleCode)
        {
            using (var db = new YintaiHZhouContext())
            {
                /**
                 * 库存同步逻辑
                 * 1. 检查门店信息
                 * 2. 检查商品Id映射关系
                 */
                // 同步门店相关信息
                var sectionExt = _sectionSyncProcessor.Sync(channelSectionId, channelStoreNo);
                if (sectionExt == null)
                {
                    Log.ErrorFormat("同步库存时发生错误,专柜sectionId:{0}不存在", channelStoreNo);
                    return null;
                }


                // 获取单品信息
                var skuExt = db.OPC_SKU.FirstOrDefault(s => s.Id == skuId);
                if (skuExt == null)
                {
                    Log.ErrorFormat("单品id:{0}不存在,请排查数据", skuId);
                    return null;
                }

                // 检查库存信息,进行价格和库存更新
                var stockExt = db.OPC_Stock.FirstOrDefault(s => s.SkuId == skuExt.Id && s.SectionId == sectionExt.Id);

                if (stockExt == null)
                {
                    var newStock = new OPC_Stock()
                    {
                        SkuId = skuExt.Id,
                        SectionId = sectionExt.Id,
                        Price = channelPrice,
                        Count = channelCount,
                        SourceStockId = sourceStockId ?? string.Empty,
                        ProdSaleCode = productSaleCode,
                        Status = 1,
                        IsDel = false,
                        CreatedDate = DateTime.Now,
                        CreatedUser = SystemDefine.SystemUser,
                        UpdatedUser = SystemDefine.SystemUser,
                        UpdatedDate = DateTime.Now
                    };

                    db.OPC_Stock.Add(newStock);
                    db.SaveChanges();

                    Log.TraceFormat("单品id:{1}创建成功", skuId);

                    return newStock;
                }

                stockExt.Price = channelPrice;
                stockExt.Count = channelCount;
                stockExt.SourceStockId = sourceStockId ?? string.Empty;
                stockExt.ProdSaleCode = productSaleCode;
                stockExt.CreatedDate = DateTime.Now;
                stockExt.CreatedUser = SystemDefine.SystemUser;
                stockExt.UpdatedDate = DateTime.Now;
                stockExt.UpdatedUser = SystemDefine.SystemUser;

                // 保存更新价格
                db.SaveChanges();

                Log.TraceFormat("单品id:{1}更新成功", skuId);

                return stockExt;
            }
        }
    }
}
