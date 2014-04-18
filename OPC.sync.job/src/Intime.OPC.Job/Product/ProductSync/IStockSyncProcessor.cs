using Intime.OPC.Domain.Models;

namespace Intime.OPC.Job.Product.ProductSync
{
    /// <summary>
    /// OPC库存同步处理器
    /// </summary>
    public interface IStockSyncProcessor
    {
        /// <summary>
        /// 库存同步
        /// </summary>
        /// <param name="skuId">本地商品唯一编码</param>
        /// <param name="channelSectionId">渠道专柜id</param>
        ///  <param name="channelStoreNo">渠道门店id</param>
        /// <param name="channelCount">渠道库存数量</param>
        /// <param name="channelPrice">渠道价格</param>
        /// <param name="sourceStockId">渠道库存Id</param>
        /// <param name="商品销售码">MIS销售码</param>
        /// <param name="ProductName">商品名称</param>
        /// <param name="SectionCode">专柜码</param>
        /// <param name="StoreCode">门店码</param>
        /// <returns>转换后的本地库存信息</returns>
        OPC_Stock Sync(int skuId, string channelSectionId, string channelStoreNo, int channelCount, decimal channelPrice, string sourceStockId, string productSaleCode,string productName,string sectionCode,string storeCode);
    }
}
