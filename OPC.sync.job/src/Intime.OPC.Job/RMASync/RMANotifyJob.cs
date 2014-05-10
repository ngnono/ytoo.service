using Common.Logging;
using Intime.O2O.ApiClient;
using Intime.O2O.ApiClient.Request;
using Intime.OPC.Domain.Enums;
using Intime.OPC.Domain.Models;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Intime.OPC.Job.RMASync
{
    [DisallowConcurrentExecution]
    public class RMANotifyJob : IJob
    {
        private static readonly ILog Logger = LogManager.GetCurrentClassLogger();
        private DateTime _benchTime = DateTime.Now.AddMinutes(-20);

        private void DoQuery(Action<IQueryable<OPC_SaleRMA>> callback,  NotificationStatus status)
        {
            using (var context = new YintaiHZhouContext())
            {
                var minx =
                    context.OPC_SaleRMA.Where(
                        t =>
                            t.UpdatedDate > _benchTime && (t.Status == (int)EnumRMAStatus.ShipInStorage || t.Status == (int)EnumRMAStatus.PayVerify) &&
                            !context.OPC_RMANotificationLogs.Any(
                                x => x.RMANo == t.RMANo && x.Status == (int) status));

                if (callback != null)
                    callback(minx);
            }
        }


        public void Execute(IJobExecutionContext context)
        {
#if !DEBUG
            JobDataMap data = context.JobDetail.JobDataMap;
            var isRebuild = data.ContainsKey("isRebuild") && data.GetBoolean("isRebuild");
            var interval = data.ContainsKey("intervalOfMins") ? data.GetInt("intervalOfMins") : 2 ;
            _benchTime = DateTime.Now.AddMinutes(-interval);
            if (isRebuild)
                _benchTime = _benchTime.AddMonths(-2);
#endif

            var totalCount = 0;
            DoQuery(skus =>
            {
                totalCount = skus.Count();
            }, NotificationStatus.Create);

            int cursor = 0;
            int size = 20;
            while (cursor < totalCount)
            {
                List<OPC_SaleRMA> oneTimeList = null;
                DoQuery(r => oneTimeList = r.OrderBy(t => t.RMANo).Skip(cursor).Take(size).ToList(), NotificationStatus.Create);
                foreach (var saleRMA in oneTimeList)
                {
                    if (saleRMA.Status == (int)EnumRMAStatus.ShipInStorage)
                        NotifyCreate(saleRMA);
                    else
                        NotifyPaid(saleRMA);
                }
                cursor += size;
            }

            /*
            totalCount = 0;
            cursor = 0;

            DoQuery(orders =>
            {
                totalCount = orders.Count();
            },  NotificationStatus.Paid);

            while (cursor < totalCount)
            {
                List<OPC_SaleRMA> oneTimeList = null;
                DoQuery(r => oneTimeList = r.OrderBy(t => t.RMANo).Skip(cursor).Take(size).ToList(), NotificationStatus.Paid);
                foreach (var saleOrder in oneTimeList)
                {
                    try
                    {
                        NotifyPaid(saleOrder);
                    }
                    catch (OrderNotificationException ex)
                    {
                        Logger.Error(ex);
                    }
                }
                cursor += size;
            }
             */

        }


        public void NotifyCreate(OPC_SaleRMA saleRMA)
        {
            var entity =new CreateRMANotificationEntity (saleRMA).CreateNotifiedEntity();
            var apiClient = new DefaultApiClient();
            var rsp = apiClient.Post(new OrderNotifyRequest()
            {
                Data = entity
            });
            if (!rsp.Status)
            {
                Logger.Error(rsp.Data);
                Logger.Error(rsp.Message);
                return;
            }

            SaleRMANotified(saleRMA, NotificationStatus.Create);
        }

        public void NotifyPaid(OPC_SaleRMA saleRMA)
        {
            var apiClient = new DefaultApiClient();
            var rsp = apiClient.Post(new OrderNotifyRequest()
            {
                Data = new PaidRMANotificationEntity(saleRMA).CreateNotifiedEntity()
            });
            if (!rsp.Status)
            {
                Logger.Error(rsp.Data);
                Logger.Error(rsp.Message);
                return;
            }
            SaleRMANotified(saleRMA,NotificationStatus.Paid);

        }

        private void SaleRMANotified(OPC_SaleRMA saleRMA, NotificationStatus status)
        {
            using (var db = new YintaiHZhouContext())
            {
                var saleRma = db.OPC_SaleRMA.FirstOrDefault(x => x.Id == saleRMA.Id);
                if (saleRma == null)
                {
                    Logger.Error(string.Format("Invalid RMA ({0})",saleRMA.RMANo));
                    return;
                }

                if (saleRma.Status != 1 && saleRma.Status != 0)
                {
                    Logger.Error(string.Format("Invalid RMA status ({0})", saleRMA.RMANo));
                    return;
                }

                saleRma.Status = (int)EnumRMAStatus.NotifyProduct;    //xiugai 状态值
                saleRma.UpdatedDate = DateTime.Now;
                saleRma.UpdatedUser = -10000;
                db.SaveChanges();

                var opc_RMA = db.OPC_RMA.FirstOrDefault(x => x.RMANo == saleRMA.RMANo);
                opc_RMA.Status = (int)EnumRMAStatus.NotifyProduct;
                opc_RMA.UpdatedDate = DateTime.Now;
                opc_RMA.UpdatedUser = -10000;
                db.SaveChanges();

                db.OPC_RMANotificationLogs.Add(new OPC_RMANotificationLog
                {
                    CreateDate = DateTime.Now,
                    CreateUser = -10000,
                    RMANo = saleRMA.RMANo,
                    Status = (int)status,
                    Message = string.Empty,
                });
                db.SaveChanges();
            }
        }
    }

    public class OrderNotificationException : Exception
    {
        public OrderNotificationException(string message)
            : base(message)
        {
        }
    }

    public enum NotificationStatus
    {
        /// <summary>
        /// 创建订单
        /// </summary>
        Create = 1,


        /// <summary>
        /// 支付
        /// </summary>
        Paid = 3,


    }

    public enum NotificationType
    {
        /// <summary>
        /// 销售单
        /// </summary>
        Create = 0,

        /// <summary>
        /// 退货单
        /// </summary>
        RMA = 1,
    }

}
