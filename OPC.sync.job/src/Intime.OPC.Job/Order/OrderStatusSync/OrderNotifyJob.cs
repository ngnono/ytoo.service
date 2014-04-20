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
        private DateTime _benchTime = DateTime.Now.AddMinutes(-1600);

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
            var interval = data.ContainsKey("intervalOfMins") ? data.GetInt("intervalOfMins") : 5 * 60;
            _benchTime = DateTime.Now.AddMinutes(-interval);
            if (!isRebuild)
                _benchTime = data.GetDateTime("benchtime");
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

    public abstract class AbstractOrderNotificationEntity
    {
        protected OPC_Sale _saleOrder;
        protected AbstractOrderNotificationEntity(OPC_Sale saleOrder)
        {
            this._saleOrder = saleOrder;
        }

        public dynamic CreateNotifiedEntity()
        {
            using (var db = new YintaiHZhouContext())
            {
                var id = _saleOrder.SaleOrderNo;
                var status = (int)Status;

                var trans =
                    db.OrderTransactions.Where(t => t.OrderNo == _saleOrder.OrderNo)
                        .Join(db.PaymentMethods, t => t.PaymentCode, p => p.Code, (t, p) => new { trans = t, payment = p })
                        .FirstOrDefault();
                var order = db.Orders.FirstOrDefault(o => o.OrderNo == _saleOrder.OrderNo);
                var storeno = string.Empty;
                if (order == null)
                {
                    throw new OrderNotificationException(string.Format("Not exists order! order No ({0})", _saleOrder.OrderNo));
                }

                if (trans == null)
                {
                    throw new OrderNotificationException(string.Format("Order has no payment information ! order no ({0})", _saleOrder.OrderNo));
                }

                var detail = new List<dynamic>();
                var payment = new List<dynamic>();
                int idx = 1;

                foreach (var de in db.OPC_SaleDetail.Where(x => x.SaleOrderNo == _saleOrder.SaleOrderNo).Join(db.OPC_Stock, x => x.StockId, s => s.Id, (x, s) => new { detail = x, stock = s }))
                {
                    storeno = de.stock.StoreCode;
                    payment.Add(new
                    {
                        id = _saleOrder.SaleOrderNo,
                        type = PaymentType,
                        typeid = trans.payment.Code,
                        typename = trans.payment.Name,
                        no = string.Empty,
                        amount = (de.detail.Price * de.detail.SaleCount).ToString(),
                        rowno = idx,
                        memo = string.Empty,
                        storeno,
                    });
                    detail.Add(new
                    {
                        id,
                        productid = de.stock.SourceStockId,
                        productname = de.stock.ProductName,
                        price = de.detail.Price.ToString(),
                        discount = "0.0",
                        vipdiscount = 0,
                        quantity = de.detail.SaleCount,
                        total = (de.detail.Price * de.detail.SaleCount).ToString(),
                        rowno = idx,
                        counter = de.stock.SectionCode,
                        memo = de.detail.Remark,
                        storeno = de.stock.StoreCode
                    });
                    idx += 1;
                }
                dynamic head = new
                {
                    id,
                    mainid = _saleOrder.OrderNo,
                    flag = 0,
                    createtime = order.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                    paytime = trans.trans.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                    type = Type,
                    status = Status,
                    quantity = _saleOrder.SalesCount,
                    discount = "0.0",
                    total = _saleOrder.SalesAmount.ToString(),
                    vipno = string.Empty,
                    vipmemo = string.Empty,
                    storeno,
                    comcount = idx - 1,
                    paycount = 1,
                    oldid = string.Empty,
                    operid = string.Empty,
                    opername = string.Empty,
                    opertime = string.Empty
                };
                return new
                {
                    id,
                    status,
                    head,
                    detail,
                    payment,
                };
            }
        }

        public abstract NotificationStatus Status { get; }

        public abstract NotificationType Type { get; }

        public abstract string PaymentType { get; }
    }

    public class CreateOrderNotificationEntity:AbstractOrderNotificationEntity
    {
        public CreateOrderNotificationEntity(OPC_Sale saleOrder) : base(saleOrder)
        {
        }

        public override NotificationStatus Status
        {
            get { return NotificationStatus.Create; }
        }

        public override NotificationType Type
        {
            get { return NotificationType.Create; }
        }

        public override string PaymentType
        {
            get { return "C0"; }
        }
    }

    public class PaidOrderNotificationEntity : AbstractOrderNotificationEntity
    {
        public PaidOrderNotificationEntity(OPC_Sale saleOrder) : base(saleOrder)
        {
        }

        public override NotificationStatus Status
        {
            get { return NotificationStatus.Paid; }
        }

        public override NotificationType Type
        {
            get { return NotificationType.Create; }
        }

        public override string PaymentType
        {
            get { return "C0"; }
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
