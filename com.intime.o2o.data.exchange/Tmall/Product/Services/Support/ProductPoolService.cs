using System.Collections.Generic;
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
        public IEnumerable<ESProduct> GetPendingProducts()
        {
            /**
             * 1. 获取待处理的商品Id列表
             * 2 .根据列表获取商品
             */


            var ids = GetProductIds();

            // 没有数据处理是返会空集合，如果去搜索查询返回所有数据
            if (ids.Count == 0)
            {
                return new List<ESProduct>();
            }


            return GetProductsByIds(ids);
        }

        public IEnumerable<ESStock> GetPendingItems()
        {
            // TODO:实现获取单品信息逻辑
            throw new System.NotImplementedException();
        }


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

        #region 帮助方法


        private IList<string> GetProductIds()
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
                        && p.ChannelId == 1008
                        && p.IsDefault == true)
                    .OrderBy(p => p.Id)
                   .Skip(0)
                   .Take(10).ToList();

                return list.ConvertAll(p => p.ProductId.ToString());
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
