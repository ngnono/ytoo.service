using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using com.intime.fashion.data.sync.Tmall;
using com.intime.fashion.data.sync.Tmall.Executor;
using com.intime.o2o.data.exchange.IT;
using Common.Logging;
using Quartz;

namespace com.intime.jobscheduler.Job.Tmall.Order
{
    public class OrderPush2ImsJob : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            JobDataMap data = context.JobDetail.JobDataMap;
            var pageSize = data.ContainsKey("pageSize") ? data.GetInt("pageSize") : 20;
            var interval = data.ContainsKey("intervalOfMins")?data.GetInt("intervalOfMins"):15;

            var benchTime = DateTime.Now.AddMinutes(-interval);

            IApiClient imsClient = new DefaultApiClient(ConstValue.IMS_SERVICE_URL, ConstValue.IMS_APP_SECRET, ConstValue.IMS_APP_KEY);

            var orderSyncExecutor = new OrderSyncExecutor(benchTime, pageSize, imsClient);

            try
            {
                orderSyncExecutor.Execute();
            }
            catch (Exception e)
            {
                LogManager.GetCurrentClassLogger().Error(e);
            }
        }
    }
}
