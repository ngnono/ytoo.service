using Common.Logging;
using Intime.OPC.Job.Product.ProductSync.Supports.Intime.Repository;
using Intime.OPC.Job.Product.ProductSync.Supports.Intime.Synchronizers;
using Quartz;

namespace Intime.OPC.Job.Product.ProductSync.Supports.Intime.Jobs
{
    /// <summary>
    /// 同步所有信息Job
    /// </summary>
    [DisallowConcurrentExecution]
    public class AllSyncJob : IJob
    {
        private readonly AllSynchronizer _allSynchronizer;
        private readonly ProductPicSynchronizer _productPicSynchronizer;
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        public AllSyncJob()
        {
            _allSynchronizer = new AllSynchronizer(new RemoteRepository(), new UpdateDateStore());
        }

        public void Execute(IJobExecutionContext context)
        {
            Log.Info("开始同步");

            _allSynchronizer.Sync();

            Log.Info("完成同步");
        }
    }
}
