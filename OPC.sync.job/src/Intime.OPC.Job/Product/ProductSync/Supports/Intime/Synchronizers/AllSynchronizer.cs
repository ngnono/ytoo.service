using System;
using System.Linq;
using System.Threading;
using Common.Logging;
using Intime.OPC.Job.Product.ProductSync.Models;
using Intime.OPC.Job.Product.ProductSync.Supports.Intime.Mapper;
using Intime.OPC.Job.Product.ProductSync.Supports.Intime.Processors;
using Intime.OPC.Job.Product.ProductSync.Supports.Intime.Repository;
using Intime.OPC.Job.Product.ProductSync.Supports.Intime.Repository.DTO;
using Intime.OPC.Domain.Models;

namespace Intime.OPC.Job.Product.ProductSync.Supports.Intime.Synchronizers
{
    /// <summary>
    /// 渠道所有信息同步器
    /// </summary>
    public class AllSynchronizer
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();
        private readonly IRemoteRepository _remoteRepository;

        private readonly IProductSyncProcessor<ProductDto> _productSyncProcessor;
        private readonly ISkuSyncProcessor _skuSyncProcessor;
        private readonly IProductPropertySyncProcessor _productPropertySyncProcessor;
        private readonly IStockSyncProcessor _stockSyncProcessor;
        private readonly ISectionSyncProcessor _sectionSyncProcessor;
        private readonly IChannelMapper _channelMapper = new ChannelMapper();

        private readonly IUpdateDateStore _updateDateStore;
        private readonly string _lastUpdateDateTimeKey = string.Empty;

        private const int PageSize = 2000;

        public AllSynchronizer(IRemoteRepository remoteRepository, IUpdateDateStore updateDateStore)
        {
            _remoteRepository = remoteRepository;
            _updateDateStore = updateDateStore;

            _skuSyncProcessor = new SkuSyncProcessor();

            var storeSyncProcessor = new StoreSyncProcessor(_remoteRepository, _channelMapper);
            var sectionSyncProcessor = new SectionSyncProcessor(_remoteRepository, storeSyncProcessor, _channelMapper);
            _stockSyncProcessor = new StockSyncProcessor(sectionSyncProcessor, _channelMapper);

            _productSyncProcessor = new ProductSyncProcessor(_remoteRepository, _channelMapper);
            _productPropertySyncProcessor = new ProductPropertySyncProcessor(_channelMapper);

            _sectionSyncProcessor = new SectionSyncProcessor(_remoteRepository, new StoreSyncProcessor(_remoteRepository, _channelMapper), _channelMapper);

            _lastUpdateDateTimeKey = GetType().FullName;
        }

        public void Sync()
        {
            var pageIndex = 1;
            var lastUpdateDateTime = _updateDateStore.GetLast(_lastUpdateDateTimeKey);

            while (true)
            {
                var products = _remoteRepository.GetProductList(pageIndex, PageSize, lastUpdateDateTime).ToList();

                if (products.Count == 0)
                {
                    Log.ErrorFormat("没有可同步的信息,pageIndex:{0},pageSize:{1},lastUpdateDatetime:{2}", pageIndex, PageSize, lastUpdateDateTime);
                    break;
                }

                Log.InfoFormat("开始处理第{0}页商品,获取商品{1}", pageIndex, products.Count);

                foreach (var product in products)
                {
                    if (string.IsNullOrEmpty(product.ProductId)) {
                        Log.ErrorFormat("Failed to sync product, productid is empty {0}",product.ProductCode);
                        continue;
                    }
                    try
                    {
                        //同步商品
                        var p = _productSyncProcessor.Sync(product);


                        // 如果商品同步失败，跳过当前商品
                        if (p == null)
                        {
                            Log.ErrorFormat("同步商品失败 productId:{0},section:{1},storeNo:{2}", product.ProductId,
                                product.SectionId, product.StoreNo);
                            continue;
                        }


                        // 同步商品花色属性
                        var color = _productPropertySyncProcessor.Sync(p.Id, product.Color, product.ColorId,
                            ProductPropertyType.Color);
                        if (color == null)
                        {
                            Log.ErrorFormat("同步商品花色属性失败productId:{0},color:{1},colorId:{2}", p.Id,
                                product.Color, product.ColorId);
                            continue;
                        }


                        // 同步商品尺码属性
                        var size = _productPropertySyncProcessor.Sync(p.Id, product.Size, product.SizeId,
                            ProductPropertyType.Size);

                        if (size == null)
                        {
                            Log.ErrorFormat("同步商品尺码属性失败productId:{0},size:{1},sizeId:{2}", p.Id,
                                product.Size, product.SizeId);
                            continue;
                        }


                        // 同步SKU
                        var sku = _skuSyncProcessor.Sync(p.Id, color.Id, size.Id);
                        if (sku == null)
                        {
                            Log.ErrorFormat("同步SKU失败 productId:{0},colorId:{1},sizeId:{2}", p.Id, color.Id, size.Id);
                            continue;
                        }


                        // 同步库存
                        var stock = _stockSyncProcessor.Sync(sku.Id, product);

                        if (stock == null)
                        {
                            Log.ErrorFormat(
                                "库存更新失败 skuId:{0},channelSectionid:{1},channelStore:{2},stock:{3},channelPrice:{4},sourceStockId:{5}",
                                sku.Id, product.SectionId, product.StoreNo, product.Stock, product.CurrentPrice,
                                product.ProductId);
                        }
                    }
                    catch (Exception ex)
                    {
                        // 这里异常处理防止接口出现问题，造成别的商品同步也会出现问题
                        Log.ErrorFormat("同步商品发生异常,proudctId:{0}", product.ProductId);
                    }
                }

                // 进行下一页
                pageIndex += 1;

                Thread.Sleep(500);
            }

        }
    }
}
