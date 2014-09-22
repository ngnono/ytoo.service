using System.Collections.Generic;
using com.intime.o2o.data.exchange.Tmall.Product.Models;
using Yintai.Hangzhou.Model.ES;
using Yintai.Hangzhou.Model.ESModel;

namespace com.intime.o2o.data.exchange.Tmall.Product.Services
{
    /// <summary>
    /// 提取服务
    /// </summary>
    public interface IProductPoolService
    {
        /// <summary>
        /// 获取待处理的产品列表
        /// </summary>
        /// <returns></returns>
        IEnumerable<ESProduct> GetPendingProducts();

        /// <summary>
        /// 获取带待处理的商品列表
        /// </summary>
        /// <returns></returns>
        IEnumerable<ESStock> GetPendingItems();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="status"></param>
        void UpdateProductStatus(int productId, ProductPoolStatus status);
    }
}
