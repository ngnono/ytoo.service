using Common.Logging;
using Intime.O2O.ApiClient;
using Intime.O2O.ApiClient.Request;
using Intime.OPC.Domain.Models;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Intime.OPC.Job.Order.OrderStatusSync
{
    [DisallowConcurrentExecution]
    public class OrderNotifyJob : IJob
    {
        private static readonly ILog Logger = LogManager.GetCurrentClassLogger();
        private DateTime _benchTime = DateTime.Now.AddMinutes(-20);

        private void DoQuery(Action<IQueryable<OPC_Sale>> callback, int orderStatus, NotificationStatus status)
        {
            using (var context = new YintaiHZhouContext())
            {
                var minx =
                    context.OPC_Sale.Where(
                        t =>
                            t.UpdatedDate > _benchTime && t.Status == orderStatus &&
                            !context.OPC_SaleOrderNotificationLogs.Any(
                                x => x.SaleOrderNo == t.SaleOrderNo && x.Status == (int) status));

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
                List<OPC_Sale> oneTimeList = null;
                DoQuery(r => oneTimeList = r.OrderBy(t => t.OrderNo).Skip(cursor).Take(size).ToList(),0, NotificationStatus.Create);
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
                List<OPC_Sale> oneTimeList = null;
                DoQuery(r => oneTimeList = r.OrderBy(t => t.OrderNo).Skip(cursor).Take(size).ToList(),1, NotificationStatus.Paid);
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


        public void NotifyCreate(OPC_Sale saleOrder)
        {
            var entity =new CreateOrderNotificationEntity (saleOrder).CreateNotifiedEntity();
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

            SaleOrderNotified(saleOrder, NotificationStatus.Create);
        }

        public void NotifyPaid(OPC_Sale saleOrder)
        {
            var apiClient = new DefaultApiClient();
            var rsp = apiClient.Post(new OrderNotifyRequest()
            {
                Data = new PaidOrderNotificationEntity(saleOrder).CreateNotifiedEntity()
            });
            if (!rsp.Status)
            {
                Logger.Error(rsp.Data);
                Logger.Error(rsp.Message);
                return;
            }
            SaleOrderNotified(saleOrder,NotificationStatus.Paid);
        }

        private void SaleOrderNotified(OPC_Sale saleOrder, NotificationStatus status)
        {
            using (var db = new YintaiHZhouContext())
            {
                var order = db.OPC_Sale.FirstOrDefault(x => x.Id == saleOrder.Id);
                if (order == null || order.Status != 0)
                {
                    Logger.Error(string.Format("Invalid order status ({0})",saleOrder.OrderNo));
                    return;
                }
                order.Status = 1;
                order.UpdatedDate = DateTime.Now;
                order.UpdatedUser = -10000;
                db.SaveChanges();

                db.OPC_SaleOrderNotificationLogs.Add(new OPC_SaleOrderNotificationLog
                {
                    CreateDate = DateTime.Now,
                    CreateUser = -10000,
                    SaleOrderNo = saleOrder.SaleOrderNo,
                    Status = (int)status,
                    Message = string.Empty,
                });
                db.SaveChanges();
            }
        }
    }

    public class OrderNotificationException : Exception
    {
        public OrderNotificationException(string message) : base(message)
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
