using Common.Logging;
using Intime.O2O.ApiClient.Yintai;
using Intime.OPC.Domain.Enums;
using Intime.OPC.Domain.Models;
using Newtonsoft.Json;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;

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
                List<OPC_RMA> oneTimeList = null;
                DoQuery(r => oneTimeList = r.OrderBy(t => t.RMANo).Skip(cursor).Take(size).ToList(),
                    NotificationStatus.Create);
                foreach (var saleRMA in oneTimeList)
                {
                    try
                    {
                        Notify2Yintai(saleRMA);
                    }
                    catch (Exception ex)
                    {
                        Logger.Error("推送退货信息失败,异常： {0}", ex);
                    }
                }
                cursor += size;
            }
        }

        private void Notify2Yintai(OPC_RMA rma)
        {
            using (var context = new YintaiHZhouContext())
            {
                var linq =
                    context.OPC_RMADetail.Where(r => r.RMANo == rma.RMANo).Select(r => new
                    {
                        ItemCode = r.StockId,
                        RMAQuantity = r.BackCount,
                        Reason = rma.RMAReason
                    });

                dynamic data = new
                {
                    OPCSONumber = rma.OrderNo,
                    Status = rma.Status == (int)EnumRMAStatus.PayVerify ? 700 : 300,
                    RMAType = 1,
                    OperaterFrom = 1,
                    OPCRMATrancaction = linq
                };
                var rmaInfos = new Dictionary<string, string>
                {
                    {"Data", JsonConvert.SerializeObject(data)},
                };
                Logger.ErrorFormat("推送退货订单 {0} 商品明细 {1}");
                var client = new YintaiApiClient();
                var rsp = client.Post(rmaInfos, "Yintai.OpenApi.RMA. AddRMA");
                if (rsp.IsSuccessful)
                {
                    context.OPC_RMANotificationLogs.Add(new OPC_RMANotificationLog
                    {
                        CreateDate = DateTime.Now,
                        CreateUser = -10000,
                        RMANo = rma.SaleOrderNo,
                        Status = (int)NotificationStatus.Sync2Yintai,
                        Message = string.Empty
                    });
                    Logger.ErrorFormat("通知银泰网RMA订单成功,RMANO {0}", rma.RMANo);
                }
                else
                {
                    Logger.ErrorFormat("推送退货单至RMA失败：RMANO:{0},银泰网返回错误:{1}", rma.RMAReason, rsp.Description);
                }
            }
        }

        private void DoQuery(Action<IQueryable<OPC_RMA>> callback, NotificationStatus status)
        {
            using (var context = new YintaiHZhouContext())
            {
                var rmas =
                    context.OPC_RMA.Where(
                        t => context.Map4Order.Any(m => m.OrderNo == t.OrderNo && m.Channel == "yintai") &&
                            t.UpdatedDate > _benchTime &&
                            (t.Status == (int)EnumRMAStatus.PayVerify || t.Status == (int)EnumRMAStatus.ShipVerifyNotPass) &&
                            !context.OPC_RMANotificationLogs.Any(
                                x => x.RMANo == t.RMANo && x.Status == (int)status));

                if (callback != null)
                    callback(rmas);
            }
        }
    }
}
