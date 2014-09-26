using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Xml.Linq;

using com.intime.o2o.data.exchange.Tmall.Core;
using com.intime.o2o.data.exchange.Tmall.Core.Support;
using com.intime.o2o.data.exchange.Tmall.Product.Mappers;
using com.intime.o2o.data.exchange.Tmall.Product.Mappers.Support;
using com.intime.o2o.data.exchange.Tmall.Product.Models;
using Common.Logging;
using Top.Api.Request;
using Yintai.Hangzhou.Model.ES;
using Yintai.Hangzhou.Model.ESModel;
using System.Collections.Generic;

namespace com.intime.o2o.data.exchange.Tmall.Product.Services.Support
{
    /// <summary>
    /// 商品推送服务
    /// </summary>
    public class ProductPushService : IProductPushService
    {
        #region fileds

        private readonly ITopClientFactory _topClientFactory = new DefaultTopClientFactory();
        private readonly IBrandMapper _brandMapper = new DefaultBrandMapper();
        private readonly ICategoryMapper _categoryMapper = new DefaultCategoryMapper();
        private readonly IProductMapper _productMapper = new DefaultProductMapper();
        private readonly IItemMapper _itemMapper = new DefaultItemMapper();
        private readonly ISchemaMapper _schemaMapper = new DefaultSchemaMapper();
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        #endregion

        #region .ctor

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

        #endregion

        #region 产品相关接口

        /// <summary>
        /// 添加商品
        /// </summary>
        /// <param name="productSchema">商品实体</param>
        /// <param name="consumerKey">消费者Key</param>
        /// <returns>添加状态报告</returns>
        public ResultInfo<long> AddProduct(ESProduct productSchema, string consumerKey)
        {
            /**================================================
             * 品牌映射关系
             ==================================================*/
            var brandId = _brandMapper.ToChannel(productSchema.Brand.Id);

            if (brandId == null)
            {
                var errorMessage = string.Format("商品品牌没有映射，productId:{0},brandId:{1}", productSchema.Id,
                    productSchema.Brand.Id);
                Log.Error(errorMessage);
                return Error<long>(errorMessage, "-10001");
            }

            /**================================================
            * 分类的映射关系
            ==================================================*/
            var categoryId = _categoryMapper.ToChannel(productSchema.CategoryId);

            if (categoryId == null)
            {
                var errorMessage = string.Format("商品分类没有映射，productId:{0},brandId:{1}", productSchema.Id,
                    productSchema.CategoryId);
                Log.Error(errorMessage);
                return Error<long>(errorMessage, "-10002");
            }

            // 创建templatekey,{模版名称}+{分类}+{品牌}
            var templateKey = string.Format("tmall.schema.product.{0}.{1}", categoryId, brandId);

            if (!_schemaMapper.ExistsTemplate(templateKey))
            {
                var errorMessage = string.Format("商品上传数据模版不存在，tempalte:{0}", templateKey);
                Log.Error(errorMessage);
                return Error<long>(errorMessage, "-10003");
            }

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
                return Error<long>(string.Format("{0}[{1}]", result.ErrMsg, result.SubErrMsg), result.ErrCode);
            }

            // 解析XMl返回结果提取productId
            var root = XElement.Load(new StringReader(result.AddProductResult));

            var field = (from item in root.Elements("field")
                         where item.Attribute("id").Value == "product_id"
                         select item).FirstOrDefault();

            if (field == null)
            {
                var errorMessage = string.Format("解析productId出错，result:{0}", result.AddProductResult);
                Log.Error(errorMessage);
                return Error<long>(errorMessage, "-10004");
            }

            var productId = Convert.ToInt64(field.Value);

            // 保存上传商品和成功后的商品Id的关系
            _productMapper.Save(productSchema.Id, productId);

            return Ok(productId);
        }

        public ResultInfo<bool> UpdateProduct(ESProduct productSchema, string consumerKey)
        {
            /**================================================
             * 获取商品Id映射关系
             ==================================================*/
            var productId = _productMapper.ToChannel(productSchema.Id);

            if (productId == null)
            {
                var errorMessage = string.Format("商品Id没有映射，或没有上传商品，productId:{0}", productSchema.Id);
                Log.Error(errorMessage);
                return Error<bool>(errorMessage, "-10005");
            }

            /**================================================
             * 品牌映射关系
             ==================================================*/
            var brandId = _brandMapper.ToChannel(productSchema.Brand.Id);

            if (brandId == null)
            {
                var errorMessage = string.Format("商品品牌没有映射，productId:{0},brandId:{1}", productSchema.Id,
                    productSchema.Brand.Id);
                Log.Error(errorMessage);
                return Error<bool>(errorMessage, "-10001");
            }

            /**================================================
            * 分类的映射关系
            ==================================================*/
            var categoryId = _categoryMapper.ToChannel(productSchema.CategoryId);

            if (categoryId == null)
            {
                var errorMessage = string.Format("商品分类没有映射，productId:{0},cateory:{1}", productSchema.Id,
                    productSchema.CategoryId);
                Log.Error(errorMessage);
                return Error<bool>(errorMessage, "-10002");
            }

            // 创建templatekey,{模版名称}+{分类}+{品牌}
            var templateKey = string.Format("tmall.schema.product.{0}.{1}", categoryId, brandId);

            if (!_schemaMapper.ExistsTemplate(templateKey))
            {
                var errorMessage = string.Format("商品上传数据模版不存在，tempalte:{0}", templateKey);
                Log.Error(errorMessage);
                return Error<bool>(errorMessage, "-10003");
            }

            // 转化商品数据Schema
            var xmlData = CreateXmlData("tmall.schema.product", new Hashtable { { "product", productSchema } });

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
                return Error<bool>(string.Format("{0}[{1}]", result.ErrMsg, result.SubErrMsg), result.ErrCode);
            }

            return Ok(true);
        }

        #endregion

        #region 商品相关接口

        public ResultInfo<long> AddItem(IEnumerable<ESStock> items, ESProduct product, string consumerKey)
        {
            if (product == null)
            {
                var errorMsg = string.Format("产品为空,请检查参数");
                Log.Error(errorMsg);
                return Error<long>(errorMsg, "408");
            }

            if (items == null)
            {
                var errorMsg = string.Format("单品列表为空,productId:{0}", product.Id);
                Log.Error(errorMsg);
                return Error<long>(errorMsg, "408");
            }

            // 获取产品Id和分类Id
            var productId = _productMapper.ToChannel(product.Id);
            var categoryId = _categoryMapper.ToChannel(product.CategoryId);

            // 创建templatekey,{模版名称}+{分类}+{品牌}
            var templateKey = string.Format("tmall.schema.item.{0}", categoryId);

            var esStocks = items as IList<ESStock> ?? items.ToList();
            var colors = esStocks.Select(p => p.ColorDesc).Distinct().ToList();
            var sizes = esStocks.Select(p => p.SizeDesc).Distinct().ToList();

            //商品总数量
            var total = esStocks.Sum(p => p.Amount);

            // 价格
            var minPrice = esStocks.Min(p => p.Price);

            // 转化商品数据Schema
            var xmlData = CreateXmlData(templateKey, new Hashtable
            {
                { "product", product },
                { "items", items }, 
                { "consumerKey", consumerKey },
                { "colors", colors },
                 { "sizes", sizes },
                 { "total", total },
                 { "minPrice", minPrice },
                 {"tools",new Tools()}
            });

            // 根据门店获取Client
            var topClient = _topClientFactory.Get(consumerKey);

            // 请求Taobao接口
            var result = topClient.Execute(new TmallItemSchemaAddRequest()
            {
                ProductId = productId,
                CategoryId = categoryId,
                XmlData = xmlData
            }, _topClientFactory.GetSessionKey("intime"));

            if (result.IsError)
            {
                return Error<long>(string.Format("{0}[{1}]", result.ErrMsg, result.SubErrMsg), result.ErrCode);
            }

            return Ok(Convert.ToInt64(result.AddItemResult));
        }

        public ResultInfo<bool> UpdateItem(ESStock item, ESProduct product, string consumerKey)
        {
            throw new NotImplementedException();
        }

        #endregion

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
