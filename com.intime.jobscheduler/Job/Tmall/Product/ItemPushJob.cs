using com.intime.o2o.data.exchange.Tmall.Product.Services;
using com.intime.o2o.data.exchange.Tmall.Product.Services.Support;
using Common.Logging;
using Quartz;
using Yintai.Architecture.Framework.Extension;

namespace com.intime.jobscheduler.Job.Tmall.Product
{
    /// <summary>
    /// 商品同步Job
    /// </summary>
    [DisallowConcurrentExecution]
    public class ItemPushJob : IJob
    {
        private readonly IProductPoolService _productPoolService = new ProductPoolService();
        private readonly IProductPushService _productPushService = new ProductPushService();
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        public void Execute(IJobExecutionContext context)
        {
            Log.Info("开始商品同步任务");
            var mergedCodeList = _productPoolService.GetAddedMergedProductCode();

            foreach (var mergeCode in mergedCodeList)
            {
                var productIds = _productPoolService.GetProductIdsByMergedProductCode(mergeCode);

                foreach (var productId in productIds)
                {
                    PushItem(productId);
                }
            }

            Log.Info("结束商品同步任务");
        }

        private void PushItem(int productId)
        {
            var product = _productPoolService.GetProductByProductId(productId);

            if (product == null)
                Log.ErrorFormat("ES获取产品信息失败,productId:{0}", productId);

            var items = _productPoolService.GetItemsByIdsByProductId(productId);

            var result = _productPushService.AddItem(items, product, "intime");

            if (result.IsError)
            {
                Log.ErrorFormat("ES同步商品信息失败,productId:{0}，items:{1},errorMsg:{2}", productId, items.ToJson(), result.ErrMsg);
            }
        }
    }
}
