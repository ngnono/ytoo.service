using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;
using Intime.OPC.Domain.Enums;
using Intime.OPC.Domain.Models;
using Quartz;

namespace Intime.OPC.Job.RMASync
{
    public class RMA2YintaiJob : IJob
    {
        private static readonly ILog Logger = LogManager.GetCurrentClassLogger();
        private DateTime _benchTime = DateTime.Now.AddMinutes(-20);
        public void Execute(IJobExecutionContext context)
        {
#if !DEBUG
            JobDataMap data = context.JobDetail.JobDataMap;
            var isRebuild = data.ContainsKey("isRebuild") && data.GetBoolean("isRebuild");
            var interval = data.ContainsKey("intervalOfMins") ? data.GetInt("intervalOfMins") : 2;
            _benchTime = DateTime.Now.AddMinutes(-interval);
            if (isRebuild)
                _benchTime = _benchTime.AddMonths(-2);
#endif

            var totalCount = 0;
            DoQuery(skus =>
            {
                totalCount = skus.Count();
            }, NotificationStatus.Sync2Yintai);

            int cursor = 0;
            int size = 20;
            while (cursor < totalCount)
            {
                List<OPC_SaleRMA> oneTimeList = null;
                DoQuery(r => oneTimeList = r.OrderBy(t => t.RMANo).Skip(cursor).Take(size).ToList(),
                    NotificationStatus.Create);
                foreach (var saleRMA in oneTimeList)
                {
                    Notify2Yintai(saleRMA);
                }
                cursor += size;
            }
        }

        private void Notify2Yintai(OPC_SaleRMA rma)
        {
            using (var context = new YintaiHZhouContext())
            {
                //rma.OrderNo
            }
            throw new NotImplementedException();
        }

        private void DoQuery(Action<IQueryable<OPC_SaleRMA>> callback, NotificationStatus status)
        {
            using (var context = new YintaiHZhouContext())
            {
                var minx =
                    context.OPC_SaleRMA.Where(
                        t => context.Map4Order.Any(m => m.OrderNo == t.OrderNo && m.Channel == "yintai") &&
                            t.UpdatedDate > _benchTime &&
                            (t.Status == (int)EnumRMAStatus.ShipInStorage || t.Status == (int)EnumRMAStatus.PayVerify || t.Status == (int)EnumRMAStatus.ShipVerifyNotPass) &&
                            !context.OPC_RMANotificationLogs.Any(
                                x => x.RMANo == t.RMANo && x.Status == (int)status));

                if (callback != null)
                    callback(minx);
            }
        }
    }
}
