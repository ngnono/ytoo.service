using System;
using System.Linq;

using Common.Logging;
using Intime.OPC.Domain.Models;
using Intime.OPC.Job.Product.ProductSync.Models;
using Intime.OPC.Job.Product.ProductSync.Supports.Intime.Repository;
using Intime.OPC.Job.Product.ProductSync.Supports.Intime.Repository.DTO;

namespace Intime.OPC.Job.Product.ProductSync.Supports.Intime.Processors
{
    /// <summary>
    /// 商品基本信息同步处理器
    /// <remarks>
    /// 商品同步自动会进行相关品牌，门店，分类，专柜的同步,
    /// </remarks>
    /// </summary>
    public class ProductSyncProcessor : IProductSyncProcessor<ProductDto>
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private readonly IBrandSyncProcessor _brandSyncProcessor;
        private readonly ICategorySyncProcessor _categorySyncProcessor;
        private readonly ISectionSyncProcessor _sectionSyncProcessor;
        private readonly IChannelMapper _channelMapper;

        public ProductSyncProcessor(IRemoteRepository remoteRepository, IChannelMapper channelMapper)
        {
            _channelMapper = channelMapper;
            _categorySyncProcessor = new CategorySyncProcessor(channelMapper);
            _brandSyncProcessor = new BrandSyncProcessor(remoteRepository, channelMapper);
            var storeSyncProcessor = new StoreSyncProcessor(remoteRepository, channelMapper);
            _sectionSyncProcessor = new SectionSyncProcessor(remoteRepository, storeSyncProcessor, channelMapper);
        }

        public ProductSyncProcessor(ICategorySyncProcessor categorySyncProcessor, IBrandSyncProcessor brandSyncProcessor, IStoreSyncProcessor storeSyncProcessor, ISectionSyncProcessor sectionSyncProcessor, IChannelMapper channelMapper)
        {
            _categorySyncProcessor = categorySyncProcessor;
            _brandSyncProcessor = brandSyncProcessor;
            _sectionSyncProcessor = sectionSyncProcessor;
            _channelMapper = channelMapper;
        }

        public Domain.Models.Product Sync(ProductDto channelProduct)
        {
            /**
             * 首先同步相关信息
             * 1. 同步品牌
             * 2. 同步分类
             * 3. 同步专柜及门店
             * 4. 同步商品基本信息
             */

            /**
             * 检查并同步品牌,目前集团的商品只有品牌名称，所以同步商品不成功，只是提供警告 
             */
            var brand = _brandSyncProcessor.SyncBrand(channelProduct.BrandId, channelProduct.BrandName);
            if (brand == null)
            {
                Log.WarnFormat("同步商品对应品牌失败:ProductId:[{0}],brandId:[{1}],brandName:[{2}]", channelProduct.ProductId, channelProduct.BrandId, channelProduct.BrandName);
            }

            /**
             * 检查并同步分类,目前集团分类信息为空，所以同步商品不成功，只是提供警告 
             */
            var category = _categorySyncProcessor.Sync(channelProduct.CategoryId4, channelProduct.Category4);
            if (category == null)
            {
                Log.WarnFormat("同步商品对应分类失败:[{0}],categoryId:[{1}],category:[{2}]", channelProduct.ProductId, channelProduct.CategoryId4, channelProduct.Category4);
            }


            /**
             * 检查并同步专柜, 同步专柜信息时同时会同步门店信息，在此无需再同步门店
             */
            var section = _sectionSyncProcessor.Sync(channelProduct.SectionId, channelProduct.StoreNo);
            if (section == null)
            {
                Log.ErrorFormat("同步商品对应专柜失败:productId:[{0}]，sectionId:[1],stroeNo:[{2}]", channelProduct.ProductId, channelProduct.SectionId, channelProduct.StoreNo);
                return null;
            }

            using (var db = new YintaiHZhouContext())
            {

                /**
                 * 同步商品基本信息,
                 * 说明 productId为商品唯一标识
                 *      productCode为商品的款编号
                 */

                var productCodeMap = _channelMapper.GetMapByChannelValue(channelProduct.ProductCode, ChannelMapType.ProductCode);

                if (productCodeMap == null)
                {
                    var newProduct = new Domain.Models.Product()
                    {
                        Brand_Id = brand == null ? 0 : brand.Id,
                        Name = channelProduct.ProductName ?? string.Empty,
                        UnitPrice = channelProduct.LabelPrice,
                        Store_Id = section.StoreId ?? 0,
                        Price = channelProduct.CurrentPrice,
                        Description = string.Empty,
                        SkuCode = string.Empty, //TODO:确认SKU
                        Is4Sale = false, //商品属性同步完成了，设置为true
                        RecommendedReason = string.Empty,
                        RecommendUser = 0,
                        SortOrder = 0,
                        Status = 1,
                        Tag_Id = 0,
                        BarCode = string.Empty,
                        Favorable = "1",
                        UpdatedDate = DateTime.Now,
                        UpdatedUser = SystemDefine.SystemUser,
                        CreatedDate = DateTime.Now,
                        CreatedUser = SystemDefine.SystemUser,
                    };

                    db.Products.Add(newProduct);
                    db.SaveChanges();

                    // 保存映射关系
                    _channelMapper.CreateMap(new ChannelMap()
                    {
                        LocalId = newProduct.Id,
                        ChannnelValue = channelProduct.ProductCode,
                        MapType = ChannelMapType.ProductCode
                    });

                    // 保存本地商品Id和渠道的对应的关系，用于如更新图片等功能的使用
                    _channelMapper.CreateMap(new ChannelMap()
                    {
                        LocalId = newProduct.Id,
                        ChannnelValue = channelProduct.ProductId,
                        MapType = ChannelMapType.ProductId
                    });

                    return newProduct;
                }

                var proudctExt = db.Products.FirstOrDefault(p => p.Id == productCodeMap.LocalId);

                if (proudctExt == null)
                {
                    Log.ErrorFormat("商品productId:[{0}],productcode:[{1}],存在映射关系，商品数据不存在", channelProduct.ProductId, channelProduct.ProductCode);
                    return null;
                }

                proudctExt.BarCode = string.Empty;

                proudctExt.Store_Id = section.StoreId ?? 0;
                proudctExt.Brand_Id = brand == null ? 0 : brand.Id;
                proudctExt.Tag_Id = 0;
                proudctExt.SkuCode = string.Empty;
                proudctExt.Name = channelProduct.ProductName ?? string.Empty;
                proudctExt.UnitPrice = channelProduct.LabelPrice;
                proudctExt.Price = channelProduct.CurrentPrice;
                proudctExt.Description = string.Empty;
                proudctExt.RecommendedReason = string.Empty;
                proudctExt.UpdatedDate = DateTime.Now;
                proudctExt.UpdatedUser = SystemDefine.SystemUser;
                db.SaveChanges();

                return proudctExt;
            }
        }
    }
}
