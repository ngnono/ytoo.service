using Common.Logging;
using Intime.OPC.Job.Product.ProductSync.Supports.Intime.Mapper;
using Intime.OPC.Job.Product.ProductSync.Supports.Intime.Processors;
using Intime.OPC.Job.Product.ProductSync.Supports.Intime.Repository;
using Intime.OPC.Job.Product.ProductSync.Supports.Intime.Synchronizers;
using Quartz;

namespace Intime.OPC.Job.Product.ProductSync.Supports.Intime.Jobs
{
     [DisallowConcurrentExecution]
    public class ProductPicSyncJob : IJob
    {
        private readonly ProductPicSynchronizer _productPicSynchronizer;
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        public ProductPicSyncJob()
        {
            _productPicSynchronizer = new ProductPicSynchronizer(new RemoteRepository(), new UpdateDateStore(), new ProductPicProcessor(new ChannelMapper()));
        }

        public void Execute(IJobExecutionContext context)
        {
            Log.Info("开始同步");

            _productPicSynchronizer.Sync();

            Log.Info("完成同步");
        }
    }
}
