﻿using Common.Logging;
using Intime.O2O.ApiClient;
using Intime.O2O.ApiClient.Request;
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

        private void DoQuery(Action<IQueryable<OPC_SaleRMA>> callback, int RMAStatus, NotificationStatus status)
        {
            using (var context = new YintaiHZhouContext())
            {
                var minx =
                    context.OPC_SaleRMA.Where(
                        t =>
                            t.UpdatedDate > _benchTime && t.Status == RMAStatus &&
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
            },0,NotificationStatus.Create);

            int cursor = 0;
            int size = 20;
            while (cursor < totalCount)
            {
                List<OPC_SaleRMA> oneTimeList = null;
                DoQuery(r => oneTimeList = r.OrderBy(t => t.RMANo).Skip(cursor).Take(size).ToList(),0, NotificationStatus.Create);
                foreach (var saleOrder in oneTimeList)
                {
                    NotifyCreate(saleOrder);
                }
                cursor += size;
            }

            totalCount = 0;
            cursor = 0;

            DoQuery(orders =>
            {
                totalCount = orders.Count();
            },1, NotificationStatus.Paid);

            while (cursor < totalCount)
            {
                List<OPC_SaleRMA> oneTimeList = null;
                DoQuery(r => oneTimeList = r.OrderBy(t => t.RMANo).Skip(cursor).Take(size).ToList(),1, NotificationStatus.Paid);
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

        public void NotifyPaid(OPC_SaleRMA saleOrder)
        {
            var apiClient = new DefaultApiClient();
            var rsp = apiClient.Post(new OrderNotifyRequest()
            {
                Data = new PaidRMANotificationEntity(saleOrder).CreateNotifiedEntity()
            });
            if (!rsp.Status)
            {
                Logger.Error(rsp.Data);
                Logger.Error(rsp.Message);
                return;
            }
            SaleRMANotified(saleOrder,NotificationStatus.Paid);
        }

        private void SaleRMANotified(OPC_SaleRMA saleRMA, NotificationStatus status)
        {
            using (var db = new YintaiHZhouContext())
            {
                var order = db.OPC_SaleRMA.FirstOrDefault(x => x.Id == saleRMA.Id);
                if (order == null)
                {
                    Logger.Error(string.Format("Invalid RMA ({0})",saleRMA.RMANo));
                    return;
                }

                if (order.Status != 1 && order.Status != 0)
                {
                    Logger.Error(string.Format("Invalid RMA status ({0})", saleRMA.RMANo));
                    return;
                }

                order.Status = 1;
                order.UpdatedDate = DateTime.Now;
                order.UpdatedUser = -10000;
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
