using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using com.intime.fashion.service.search;

using Nest;
using Yintai.Hangzhou.Data.Models;
using Yintai.Hangzhou.Model.ES;
using Yintai.Hangzhou.Model.ESModel;

namespace com.intime.o2o.data.exchange.Tmall.Product.Services.Support
{
    /// <summary>
    /// 提取服务
    /// </summary>
    public class ExtractionService : IExtractionService
    {
        public IEnumerable<ESProduct> GetPendingProducts()
        {
            /**
             * 1. 获取待处理的商品Id列表
             * 2 .根据列表获取商品
             */

            var ids = GetProductIds();

            return GetProductsByIds(ids);
        }

        public IEnumerable<ESStock> GetPendingItems()
        {
            // TODO:实现获取单品信息逻辑
            throw new System.NotImplementedException();
        }

        #region 帮助方法

        private IEnumerable<string> GetProductIds()
        {
            //TODO:修改逻辑进行商品获取逻辑
            using (var db = new YintaiHangzhouContext())
            {
                return db.ProductPool.Where(
                    p =>
                        p.Status == 100)
                    .Select(p => p.ProductId.ToString(CultureInfo.InvariantCulture))
                    .ToList();
            }
        }

        /// <summary>
        /// 获取默认的Es搜索Client
        /// </summary>
        /// <returns>ES搜索客户端</returns>
        private IElasticClient GetDefaultElasticClient()
        {
            return SearchLogic.GetClient();
        }

        /// <summary>
        /// 根据商品列表获取商品信息
        /// </summary>
        /// <param name="ids">商品id列表</param>
        /// <returns>商品基本信息列表</returns>
        private IEnumerable<ESProduct> GetProductsByIds(IEnumerable<string> ids)
        {
            return GetDefaultElasticClient().Search<ESProduct>(
                body =>
                    body.Filter(q =>
                        q.Terms(p => p.Id, ids))
                        .Skip(0)
                        .Size(5000))
                    .Documents;
        }

        /// <summary>
        /// 根据库存id,获取单品列表信息
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        private IEnumerable<ESStock> GetItemsByIds(IEnumerable<string> ids)
        {
            return GetDefaultElasticClient().Search<ESStock>(
                body =>
                    body.Filter(q =>
                        q.Terms(p => p.Id, ids))
                        .Skip(0).
                        Size(5000))
                    .Documents;
        }

        #endregion
    }
}
