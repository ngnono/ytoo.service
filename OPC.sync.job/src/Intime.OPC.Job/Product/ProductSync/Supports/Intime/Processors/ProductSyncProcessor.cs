﻿using System;
using System.Configuration;
using System.Data.Entity.Infrastructure;
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
        private readonly int DefaultTagId = int.Parse(ConfigurationManager.AppSettings["ERPSYN_DEFAULT_TAG"]);
        private static int DEFAULT_OWNER_ID = int.Parse(ConfigurationManager.AppSettings["ERPSYN_DEFAULT_USER"]);

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
             * 检查并同步专柜, 同步专柜信息时同时会同步门店信息，在此无需再同步门店
             */
            var section = _sectionSyncProcessor.Sync(channelProduct.SectionId, channelProduct.StoreNo);
            if (section == null)
            {
                Log.ErrorFormat("同步商品对应专柜失败:productId:[{0}]，sectionId:[1],stroeNo:[{2}]", channelProduct.ProductId, channelProduct.SectionId, channelProduct.StoreNo);
                return null;
            }

            /**
             * 检查并同步品牌,目前集团的商品只有品牌名称，所以同步商品不成功，只是提供警告 
             */
            var brand = _brandSyncProcessor.SyncBrand(channelProduct.BrandId, channelProduct.BrandName);
            if (brand == null)
            {    
                //zxy modi 2014-4-14
                //品牌没有的，按所属专柜在查找，如果没有的，如果有则关联此品牌；如果没有，则依据专柜创建品牌并关联
                using (var db = new YintaiHZhouContext())
                {
                    brand = db.Brands.FirstOrDefault(b => b.Name == section.Name ||b.EnglishName==section.Name);
                    if (brand==null)
                    {
                        /**
                        * 品牌映射关系不存在，
                        * 
                        * 1. 创建新品牌
                        * 2. 保存映射关系
                        */

                        var newBrand = new Brand()
                        {
                            //ChannelBrandId = brandSource.BrandId,
                            Name = section.Name,
                            EnglishName = string.Empty,
                            Logo = string.Empty,
                            Status = 1,
                            Description = string.Empty,
                            Group = string.Empty,
                            WebSite = string.Empty,
                            CreatedDate = DateTime.Now,
                            CreatedUser = SystemDefine.SystemUser,
                            UpdatedDate = DateTime.Now,
                            UpdatedUser = SystemDefine.SystemUser,
                        };

                        // 添加品牌并保持
                        db.Brands.Add(newBrand);
                        db.SaveChanges();

                        // 添加映射关系
                        var newChannelMap = new ChannelMap()
                        {
                            LocalId = newBrand.Id,
                            ChannnelValue = section.Name,
                            MapType = ChannelMapType.Brand
                        };

                        _channelMapper.CreateMap(newChannelMap);

                        brand = newBrand;

                    }

                }
                Log.WarnFormat("同步商品对应品牌失败:ProductId:[{0}],brandId:[{1}],brandName:[{2}]", channelProduct.ProductId, channelProduct.BrandId, channelProduct.BrandName);
            }

            /**
             * 检查并同步分类,目前集团分类信息为空，所以同步商品不成功，只是提供警告 
             */
            //var category = _categorySyncProcessor.Sync(channelProduct.CategoryId4, channelProduct.Category4);
            //if (category == null)
            //{
            //    //如果分类为空，设置一个默认值
            //    Log.WarnFormat("同步商品对应分类失败:[{0}],categoryId:[{1}],category:[{2}]", channelProduct.ProductId, channelProduct.CategoryId4, channelProduct.Category4);
            //}

            var tagId = FetchTagId(channelProduct);

            using (var db = new YintaiHZhouContext())
            {
                /**
                 * 同步商品基本信息,
                 * 说明 productId为商品唯一标识
                 *      productCode为商品的款编号
                 *      商品款号加上品牌id唯一标识
                 */
                var mapKey = string.Format("{1}-{0}", channelProduct.ProductCode, brand.Id);
                var productCodeMap = _channelMapper.GetMapByChannelValue(mapKey, ChannelMapType.ProductCode);

                if (productCodeMap == null)
                {
                    var newProduct = new Domain.Models.Product()
                    {
                        ProductType = 1,
                        Brand_Id = brand.Id,
                        Name = string.IsNullOrEmpty(channelProduct.ProductName)?string.Format("{0}-{1}",brand.Name,channelProduct.ProductCode):channelProduct.ProductName,
                        UnitPrice = channelProduct.LabelPrice,
                        Store_Id = section.StoreId ?? 0,
                        Price = channelProduct.CurrentPrice,
                        Description = string.Empty,
                        SkuCode = channelProduct.ProductCode,
                        Is4Sale = false, //商品属性同步完成了，设置为true
                        RecommendedReason = string.Empty,
                        RecommendUser = DEFAULT_OWNER_ID,
                        SortOrder = 0,
                        Status = 1,
                        Tag_Id = tagId,
                        BarCode = string.Empty,
                        Favorable = "1",
                        UpdatedDate = DateTime.Now,
                        UpdatedUser = DEFAULT_OWNER_ID,
                        CreatedDate = DateTime.Now,
                        CreatedUser = DEFAULT_OWNER_ID,
                    };

                    db.Products.Add(newProduct);
                    db.SaveChanges();

                    // 保存映射关系
                    _channelMapper.CreateMap(new ChannelMap()
                    {
                        LocalId = newProduct.Id,
                        ChannnelValue = mapKey,
                        MapType = ChannelMapType.ProductCode
                    });

                    // 保存本地商品Id和渠道的对应的关系，用于如更新图片等功能的使用
                    _channelMapper.CreateMap(new ChannelMap()
                    {
                        LocalId = newProduct.Id,
                        ChannnelValue = channelProduct.ProductId,
                        MapType = ChannelMapType.ProductId
                    });

                    //if (category == null)
                    //{
                    //    CategoriesMap(newProduct.Id);
                    //}
                    return newProduct;
                }

                var proudctExt = db.Products.FirstOrDefault(p => p.Id == productCodeMap.LocalId);

                if (proudctExt == null)
                {
                    Log.ErrorFormat("商品productId:[{0}],productcode:[{1}],存在映射关系，商品数据不存在", channelProduct.ProductId, channelProduct.ProductCode);
                    return null;
                }

                //proudctExt.BarCode = string.Empty;                 
                proudctExt.Store_Id = section.StoreId ?? 0;
                proudctExt.Brand_Id = brand == null ? 0 : brand.Id;
                proudctExt.Tag_Id = tagId;
                proudctExt.SkuCode = channelProduct.ProductCode;
                proudctExt.Name = string.IsNullOrEmpty(channelProduct.ProductName) ? string.Format("{0}-{1}", brand.Name, channelProduct.ProductCode) : channelProduct.ProductName;
                proudctExt.UnitPrice = channelProduct.LabelPrice;
                proudctExt.Price = channelProduct.CurrentPrice;
                //proudctExt.Description = string.Empty;
                //proudctExt.RecommendedReason = string.Empty;
                proudctExt.UpdatedDate = DateTime.Now;
                proudctExt.UpdatedUser = DEFAULT_OWNER_ID;

                //if (category == null)
                //{
                //    CategoriesMap(proudctExt.Id);
                //}

                db.SaveChanges();
                return proudctExt;
            }
            
        }

        private int FetchTagId(ProductDto channelProduct)
        {
            using (var db = new YintaiHZhouContext())
            {
                var map =
                    db.OPC_CategoryMaps.FirstOrDefault(
                        x =>
                            x.ChannelCategoryCode == channelProduct.CategoryId1 ||
                            x.ChannelCategoryCode == channelProduct.CategoryId2 ||
                            x.ChannelCategoryCode == channelProduct.CategoryId3 ||
                            x.ChannelCategoryCode == channelProduct.CategoryId4);

                return map == null ? DefaultTagId : map.TagId;
            }
        }

        //private void CategoriesMap(int productId)
        //{
        //    using (var db = new YintaiHZhouContext())
        //    {
        //        var category = db.Categories.FirstOrDefault();
        //        var productMap = new ProductMap()
        //        {
        //            ProductId = productId,
        //            ChannelPId=productId,
        //            Channel="ERP",
        //            UpdateDate=DateTime.Now

        //        };
        //        db.ProductMaps.Add(productMap);

        //        db.SaveChanges();
        //    }
        //}
    }
}
