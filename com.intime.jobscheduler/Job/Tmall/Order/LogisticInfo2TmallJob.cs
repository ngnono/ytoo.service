using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.intime.fashion.data.sync.Tmall;
using com.intime.fashion.data.sync.Tmall.Executor;
using com.intime.o2o.data.exchange.IT;
using Common.Logging;
using Quartz;
using Top.Api;

namespace com.intime.jobscheduler.Job.Tmall.Order
{
    public class LogisticInfo2TmallJob : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            JobDataMap data = context.JobDetail.JobDataMap;
            var pageSize = data.ContainsKey("pageSize") ? data.GetInt("pageSize") : 20;
            var interval = data.ContainsKey("intervalOfMins") ? data.GetInt("intervalOfMins") : 15;

            var benchTime = DateTime.Now.AddMinutes(-interval);
            ITopClient topClient = new DefaultTopClient(ConstValue.TOP_SERVICE_URL, ConstValue.TOP_APP_KEY,
                ConstValue.TOP_APP_SECRET);
            IApiClient imsClient = new DefaultApiClient(ConstValue.IMS_SERVICE_URL,ConstValue.IMS_APP_SECRET,ConstValue.IMS_APP_KEY);
            var logisticExecutor = new LogisticsExecutor(benchTime, pageSize,topClient,imsClient, ConstValue.TOP_SESSION_KEY);

            try
            {
                logisticExecutor.Execute();
            }
            catch (Exception e)
            {
                 LogManager.GetCurrentClassLogger().Error(e);
            }
        }
    }
}
