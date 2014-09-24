using com.intime.o2o.data.exchange.Tmall.Product.Models;
using com.intime.o2o.data.exchange.Tmall.Product.Services;
using com.intime.o2o.data.exchange.Tmall.Product.Services.Support;
using Common.Logging;
using Quartz;

namespace com.intime.jobscheduler.Job.Tmall.Product
{
    /// <summary>
    /// 产品同步Job
    /// </summary>
    [DisallowConcurrentExecution]
    public class ProductPushJob : IJob
    {
        private readonly IProductPoolService _productPoolService = new ProductPoolService();
        private readonly IProductPushService _productPushService = new ProductPushService();
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        public void Execute(IJobExecutionContext context)
        {
            /**
             * 1. 获取待处理的商品列表
             * 2.遍历商品列表调用ProductPushService进行Push商品到Tmall
             * 3.更新处理后的状态
             */
            Log.Info("开始同步商品到天猫");

            var proudctList = _productPoolService.GetPendingProducts();

            foreach (var productSchema in proudctList)
            {
                Log.Info(string.Format("开始同步产品id:{0}到天猫", productSchema.Id));

                var result = _productPushService.AddProduct(productSchema, "intime");

                // 更新商品更新状态
                _productPoolService.UpdateProductStatus(productSchema.Id, result.IsError ? ProductPoolStatus.Error : ProductPoolStatus.Ok, result.ErrMsg);

                Log.Info(string.Format("完成同步产品id:{0}到天猫", productSchema.Id));
            }
        }
    }
}
