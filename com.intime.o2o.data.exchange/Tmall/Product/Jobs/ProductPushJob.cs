using com.intime.o2o.data.exchange.Tmall.Product.Services;
using com.intime.o2o.data.exchange.Tmall.Product.Services.Support;
using Quartz;

namespace com.intime.o2o.data.exchange.Tmall.Product.Jobs
{
    /// <summary>
    /// 产品同步Job
    /// </summary>
    public class ProductPushJob : IJob
    {
        private readonly IExtractionService _extractionService = new ExtractionService();
        private readonly IProductPushService _productPushService = new ProductPushService();

        public void Execute(IJobExecutionContext context)
        {
            /**
             * 1. 获取待处理的商品列表
             * 2.遍历商品列表调用ProductPushService进行Push商品到Tmall
             * 3.更新处理后的状态
             */

            var proudctList = _extractionService.GetPendingProducts();

            foreach (var productSchema in proudctList)
            {
                _productPushService.AddProduct(productSchema, "intime");
            }
        }
    }
}
