using com.intime.fashion.data.sync.Wgw.Executor;
using System;

namespace com.intime.fashion.data.sync.runner.Runner
{
    public class MapBrandRunner:Runner
    {
        protected override void Do()
        {
            DateTime benchTime = DateTime.Now.AddYears(-1);
            var productSyncExecutor = new BrandSyncExecutor(benchTime, Logger);
            var executeInfo = productSyncExecutor.Execute();
            if (executeInfo.Status != ExecuteStatus.Succeed)
            {
                foreach (var msg in executeInfo.MessageList)
                {
                    Console.WriteLine(msg);
                }
            }
            else
            {
                Console.WriteLine("成功映射商品数量{0}", executeInfo.SucceedCount);
            }
        }
    }
}
