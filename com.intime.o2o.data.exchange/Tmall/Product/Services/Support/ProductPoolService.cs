﻿using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using com.intime.fashion.service.search;
using com.intime.o2o.data.exchange.Tmall.Product.Models;
using Nest;
using Yintai.Hangzhou.Data.Models;
using Yintai.Hangzhou.Model.ES;
using Yintai.Hangzhou.Model.ESModel;

namespace com.intime.o2o.data.exchange.Tmall.Product.Services.Support
{
    /// <summary>
    /// 提取服务
    /// </summary>
    public class ProductPoolService : IProductPoolService
    {
        private const int ChannelId = 1008;
        /**
         * 说明：ProductPool status:
         * 
         * 100: 待处理产品
         * 200：已经成功添加的商品
         * 500：上传商品出错
         * 800：上传产品完成，并完成商品的添加
         * */
        #region 获取待处理的新产品

        public IEnumerable<int> GetPendingProductIds()
        {
            /**
            *   获取ProductPool表中的，待处理产品数据中默认商品的ProductId进行添加商品
             *  默认一次获取20条数据
            */
            using (var db = new YintaiHangzhouContext())
            {
                var list = db.ProductPools.Where(
                    p =>
                        p.Status == 100
                        && p.ChannelId == ChannelId
                        && p.IsDefault == true)
                    .OrderBy(p => p.Id)
                   .Skip(0)
                   .Take(10).ToList();

                return list.ConvertAll(p => p.ProductId);
            }
        }

        #endregion

        #region 获取单品相关方法

        public ESProduct GetProductByProductId(int productId)
        {

            return GetDefaultElasticClient().Search<ESProduct>(
                body =>
                    body.Filter(q =>
                        q.Term(p => p.Id, productId))
                        .Skip(0).
                        Size(1))
                    .Documents.FirstOrDefault();
        }

        public IEnumerable<string> GetAddedMergedProductCode()
        {
            /**
            *   获取已经成功上传产品的IDS
             *  默认一次获取20条数据
            */
            using (var db = new YintaiHangzhouContext())
            {
                var list = db.ProductPools.Where(
                    p =>
                        p.Status == 200
                        && p.ChannelId == ChannelId)
                    .OrderBy(p => p.ProductId)
                   .Skip(0)
                   .Take(10).ToList();

                return list.ConvertAll(p => p.MergedProductCode.ToString(CultureInfo.InvariantCulture));
            }
        }

        public IEnumerable<int> GetProductIdsByMergedProductCode(string code)
        {
            using (var db = new YintaiHangzhouContext())
            {
                var list = db.ProductPools.Where(
                    p =>
                        p.Status == 200
                        && p.MergedProductCode == code
                        && p.ChannelId == ChannelId)
                    .OrderBy(p => p.ProductId)
                   .Skip(0)
                   .Take(10).ToList();

                return list.ConvertAll(p => p.ProductId);
            }
        }

        /// <summary>
        /// 根据库存id,获取单品列表信息
        /// </summary>
        /// <param name="productId">商品Id</param>
        /// <returns></returns>
        public IEnumerable<ESStock> GetItemsByIdsByProductId(int productId)
        {
            return GetDefaultElasticClient().Search<ESStock>(
                body =>
                    body.Filter(q =>
                        q.Term(p => p.ProductId, productId))
                        .Skip(0).
                        Size(5000))
                    .Documents;
        }

        #endregion


        #region 更新同步商品状态及失败原因

        public void UpdateProductStatus(int productId, ProductPoolStatus status, string errorMessage)
        {
            using (var db = new YintaiHangzhouContext())
            {
                var extProductPool = db.ProductPools.FirstOrDefault(p => p.ProductId == productId);
                if (extProductPool != null)
                {
                    extProductPool.Status = (int)status;
                    extProductPool.ErrorMessage = errorMessage;
                }
                db.SaveChanges();
            }
        }

        #endregion

        #region 帮助方法

        /// <summary>
        /// 获取默认的Es搜索Client
        /// </summary>
        /// <returns>ES搜索客户端</returns>
        private IElasticClient GetDefaultElasticClient()
        {
            return SearchLogic.GetClient();
        }

        #endregion
    }
}
