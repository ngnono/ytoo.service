using Common.Logging;
using Intime.OPC.Domain.Enums;
using Intime.OPC.Domain.Models;
using Intime.OPC.Job.Order.DTO;
using Intime.OPC.Job.Order.Repository;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intime.OPC.Job.Order.OrderStatusSync
{
       [DisallowConcurrentExecution]
    public class RMAStatusSyncJob : IJob
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();
        private DateTime _benchTime = DateTime.Now.AddDays(-5);
        private readonly IOrderRemoteRepository _remoteRepository = new OrderRemoteRepository();

        private void DoQuery(Action<IQueryable<OPC_SaleRMA>> callback)
        {
            using (var context = new YintaiHZhouContext())
            {
                var linq = context.OPC_SaleRMA.Where(t => t.UpdatedDate > _benchTime && t.Status > 0 && t.Status == (int)EnumRMAStatus.ShipInStorage);
                if (callback != null)
                    callback(linq);
            }
        }

        #region IJob 成员

        public void Execute(IJobExecutionContext context)
        {
            var totalCount = 0;
#if !DEBUG
            JobDataMap data = context.JobDetail.JobDataMap;
            var isRebuild = data.ContainsKey("isRebuild") ? data.GetBoolean("isRebuild") : false;
            var interval = data.ContainsKey("intervalOfDays") ? data.GetInt("intervalOfDays") : 5;
             _benchTime = DateTime.Now.AddDays(-interval);
             if (isRebuild)
                 _benchTime = _benchTime.AddMonths(-2);
#endif
            DoQuery(saleRMA =>
            {
                totalCount = saleRMA.Count();
            });
            int cursor = 0;
            int size = 20;
            while (cursor < totalCount)
            {
                List<OPC_SaleRMA> oneTimeList = null;
                DoQuery(r => oneTimeList = r.OrderBy(t => t.RMANo).Skip(cursor).Take(size).ToList());
                foreach (var opc_saleRMA in oneTimeList)
                {
                    Process(opc_saleRMA);          // 同步状态到单品系统
                }
                cursor += size;
            }
        }
        private void Process(OPC_SaleRMA opc_SaleRMA)
        {
            OrderStatusResultDto saleStatus = null;
            try
            {
                saleStatus = _remoteRepository.GetRMAStatusById(opc_SaleRMA);
                ProcessSaleOrderStatus(opc_SaleRMA, saleStatus);
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }

        private void ProcessSaleOrderStatus(OPC_SaleRMA saleRMA, OrderStatusResultDto saleStatus)
        {
            var processor = SaleOrderStatusProcessorFactory.Create(int.Parse(saleStatus.Status));
            processor.Process(saleRMA.RMANo, saleStatus);
        }

        #endregion
    }
}
