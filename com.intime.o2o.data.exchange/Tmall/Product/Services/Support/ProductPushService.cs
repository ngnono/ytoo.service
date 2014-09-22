﻿using System;
using System.Collections;
using System.IO;
using System.Xml.Linq;
using System.Xml.XPath;

using com.intime.o2o.data.exchange.Tmall.Core;
using com.intime.o2o.data.exchange.Tmall.Core.Support;
using com.intime.o2o.data.exchange.Tmall.Mappers.Support;
using com.intime.o2o.data.exchange.Tmall.Product.Mappers;
using com.intime.o2o.data.exchange.Tmall.Product.Mappers.Support;
using com.intime.o2o.data.exchange.Tmall.Product.Models;

using Top.Api.Request;
using Yintai.Hangzhou.Model.ES;
using Yintai.Hangzhou.Model.ESModel;

namespace com.intime.o2o.data.exchange.Tmall.Product.Services.Support
{
    /// <summary>
    /// 商品推送服务
    /// </summary>
    public class ProductPushService : IProductPushService
    {
        private readonly ITopClientFactory _topClientFactory = new DefaultTopClientFactory();
        private readonly IBrandMapper _brandMapper = new DefaultBrandMapper();
        private readonly ICategoryMapper _categoryMapper = new DefaultCategoryMapper();
        private readonly IProductMapper _productMapper = new DefaultProductMapper();
        private readonly IItemMapper _itemMapper = new DefaultItemMapper();
        private readonly ISchemaMapper _schemaMapper = new DefaultSchemaMapper();

        public ProductPushService()
        {

        }

        public ProductPushService(ITopClientFactory topClientFactory, IBrandMapper brandMapper, ICategoryMapper categoryMapper, IProductMapper productMapper, ISchemaMapper schemaMapper, IItemMapper itemMapper)
        {
            _topClientFactory = topClientFactory;
            _brandMapper = brandMapper;
            _categoryMapper = categoryMapper;
            _productMapper = productMapper;
            _schemaMapper = schemaMapper;
            _itemMapper = itemMapper;
        }

        /// <summary>
        /// 添加商品
        /// </summary>
        /// <param name="productSchema">商品实体</param>
        /// <param name="consumerKey">消费者Key</param>
        /// <returns>添加状态报告</returns>
        public ResultInfo<long> AddProduct(ESProduct productSchema, string consumerKey)
        {
            /**================================================
             * 获取品牌和分类的映射关系
             ==================================================*/
            var brandId = _brandMapper.ToChannel(productSchema.Brand.Id);
            var categoryId = _categoryMapper.ToChannel(productSchema.CategoryId);

            //创建templatekey,{模版名称}+{分类}+{品牌}
            var templateKey = string.Format("tmall.schema.product_{0}_{1}", categoryId, brandId);

            // 转化商品数据Schema
            var xmlData = CreateXmlData(templateKey, new Hashtable
            {
                { "product", productSchema },
                {"consumerKey",consumerKey}
            });

            // 根据门店获取Client
            var topClient = _topClientFactory.Get(consumerKey);

            // 请求Taobao接口
            var result = topClient.Execute(new TmallProductSchemaAddRequest()
            {
                BrandId = brandId,
                CategoryId = categoryId,
                XmlData = xmlData
            }, _topClientFactory.GetSessionKey(consumerKey));

            if (result.IsError)
            {
                return Error<long>(result.ErrCode, result.ErrMsg);
            }

            // 解析XMl返回结果提取productId
            var root = XElement.Load(new StringReader(result.AddProductResult));

            var productId = Convert.ToInt64(root.XPathSelectElement("field[id='product_id']").Value);

            // 保存上传商品和成功后的商品Id的关系
            _productMapper.Save(productSchema.Id, productId);

            return Ok(productId);
        }

        ResultInfo<bool> IProductPushService.UpdateProduct(ESProduct product, string consumerKey)
        {
            /**================================================
             * 获取商品映射关系
             ==================================================*/
            var productId = _productMapper.ToChannel(product.Id);

            // 转化商品数据Schema
            var xmlData = CreateXmlData("tmall.schema.product", new Hashtable { { "product", product } });

            // 根据门店获取Client
            var topClient = _topClientFactory.Get(consumerKey);

            // 请求Taobao接口
            var result = topClient.Execute(new TmallProductSchemaUpdateRequest()
            {
                ProductId = productId,
                XmlData = xmlData
            });

            if (result.IsError)
            {
                return Error<bool>(result.ErrCode, result.ErrMsg);
            }

            return Ok(true);
        }

        public ResultInfo<long> AddItem(ESStock item, ESProduct product, string consumerKey)
        {
            // 获取产品Id和分类Id
            var productId = _productMapper.ToChannel(item.ProductId);
            var categoryId = _categoryMapper.ToChannel(product.CategoryId);

            // 转化商品数据Schema
            var xmlData = CreateXmlData("tmall.schema.item", new Hashtable { { "product", product }, { "item", item } });

            // 根据门店获取Client
            var topClient = _topClientFactory.Get(consumerKey);

            // 请求Taobao接口
            var result = topClient.Execute(new TmallItemSchemaAddRequest()
            {
                ProductId = productId,
                CategoryId = categoryId,
                XmlData = xmlData
            });

            if (result.IsError)
            {
                return Error<long>(result.ErrCode, result.ErrMsg);
            }

            //TODO:提取ItemId逻辑添加

            return Ok(0L);
        }

        public ResultInfo<bool> UpdateItem(ESStock item, ESProduct product, string consumerKey)
        {
            // 获取数据映射关系
            var itemId = _itemMapper.ToChannel(item.Id);
            var categoryId = _categoryMapper.ToChannel(product.CategoryId);
            var productId = _productMapper.ToChannel(item.ProductId);

            //TODO:根据实际情况调整模版Key
            // 转化商品数据Schema
            var xmlData = CreateXmlData("tmall.schema.item", new Hashtable { { "product", product }, { "item", item } });

            // 根据门店获取Client
            var topClient = _topClientFactory.Get(consumerKey);

            // 请求Taobao接口
            var result = topClient.Execute(new TmallItemSchemaUpdateRequest()
            {
                ItemId = itemId,
                CategoryId = categoryId,
                ProductId = productId,
                XmlData = xmlData
            });

            if (result.IsError)
            {
                return Error<bool>(result.ErrCode, result.ErrMsg);
            }

            return Ok(true);
        }

        #region 帮助方法

        private string CreateXmlData(string schemaName, Hashtable proprties)
        {
            /**================================================
            * 添加公共方法
            ================================================*/
            proprties.Add("brandMapper", _brandMapper);
            proprties.Add("categoryMapper", _categoryMapper);
            proprties.Add("productMapper", _productMapper);
            proprties.Add("itemMapper", _itemMapper);

            // 转化商品数据Schema
            return _schemaMapper.Map(schemaName, proprties);
        }

        private ResultInfo<T> Error<T>(string errMsg, string errCode)
        {
            return new ResultInfo<T>()
            {
                ErrMsg = errMsg,
                ErrCode = errCode
            };
        }

        private ResultInfo<T> Ok<T>(T data)
        {
            return new ResultInfo<T>()
            {
                Data = data
            };
        }

        #endregion
    }
}
