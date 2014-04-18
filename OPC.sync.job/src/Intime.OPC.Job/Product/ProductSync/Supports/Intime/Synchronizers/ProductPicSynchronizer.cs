using System;
using System.Linq;
using System.Threading;
using Common.Logging;
using Intime.OPC.Job.Product.ProductSync.Supports.Intime.Repository;
using Intime.OPC.Domain.Models;

namespace Intime.OPC.Job.Product.ProductSync.Supports.Intime.Synchronizers
{
    /// <summary>
    /// 商品同步
    /// </summary>
    public class ProductPicSynchronizer
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();
        private readonly IRemoteRepository _remoteRepository;
        private readonly string _lastUpdateDateTimeKey = string.Empty;
        private readonly IUpdateDateStore _updateDateStore;
        private const int PageSize = 20;
        private readonly IProductPicProcessor _productPicProcessor;

        public ProductPicSynchronizer(IRemoteRepository remoteRepository, IUpdateDateStore updateDateStore, IProductPicProcessor productPicProcessor)
        {
            _remoteRepository = remoteRepository;
            _updateDateStore = updateDateStore;
            _productPicProcessor = productPicProcessor;
            _lastUpdateDateTimeKey = GetType().FullName;
        }

        public void Sync()
        {
            var pageIndex = 1;
            var lastUpdateDateTime = _updateDateStore.GetLast(_lastUpdateDateTimeKey);

            while (true)
            {
                var products = _remoteRepository.GetProudctImages(pageIndex, PageSize, lastUpdateDateTime).ToList();

                if (products.Count == 0)
                {
                    Log.ErrorFormat("没有可同步的信息,pageIndex:{0},pageSize:{1},lastUpdateDatetime:{2}", pageIndex, PageSize, lastUpdateDateTime);
                    break;
                }

                Log.InfoFormat("开始处理第{0}页商品,获取商品{1}", pageIndex, products.Count);

                foreach (var product in products)
                {
                    try
                    {
                        _productPicProcessor.Sync(product.ProductId, product.ColorId, product.Url,product.Id,product.SeqNo,product.WriteTime);                      

                    }
                    catch (Exception ex)
                    {
                        // 这里异常处理防止接口出现问题，造成别的商品同步也会出现问题
                        Log.ErrorFormat("同步商品发生异常,proudctId:{0}", product.ProductId);
                    }
                }

                // 进行下一页
                pageIndex += 1;

                Thread.Sleep(500);
            }

        }
    }
}
