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
        private DateTime _benchTime = DateTime.Now.AddMinutes(-600);

        private void DoQuery(Action<IQueryable<OPC_Sale>> callback)
        {
            using (var context = new YintaiHZhouContext())
            {
                var minx = context.OPC_Sale.Where(t => t.UpdatedDate > _benchTime && t.Status == 0);

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
            });

            int cursor = 0;
            int size = 20;
            while (cursor < totalCount)
            {
                List<OPC_Sale> oneTimeList = null;
                DoQuery(r => oneTimeList = r.OrderBy(t => t.OrderNo).Skip(cursor).Take(size).ToList());
                foreach (var saleOrder in oneTimeList)
                {
                    Notify(saleOrder);
                }
                cursor += size;
            }
        }


        public void Notify(OPC_Sale saleOrder)
        {
            var entity = CreateNotifyEntity(saleOrder);
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

            SaleOrderNotified(saleOrder);
        }

        private void SaleOrderNotified(OPC_Sale saleOrder)
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
            }
        }

        private dynamic CreateNotifyEntity(OPC_Sale saleOrder)
        {
            using (var db = new YintaiHZhouContext())
            {
                var id = saleOrder.SaleOrderNo;
                int status = GetStatus();

                var trans =
                    db.OrderTransactions.Where(t => t.OrderNo == saleOrder.OrderNo)
                        .Join(db.PaymentMethods, t => t.PaymentCode, p => p.Code, (t, p) => new {trans = t, payment = p})
                        .FirstOrDefault();
                var order = db.Orders.FirstOrDefault(o => o.OrderNo == saleOrder.OrderNo);
                var storeno = string.Empty;
                if (order == null)
                {
                    Logger.Error(string.Format("Not exists order! order No ({0})", saleOrder.OrderNo));
                    return null;
                }

                if (trans == null)
                {
                    Logger.Error(string.Format("Order has no payment information ! order no ({0})", saleOrder.OrderNo));
                    return null;
                }

                dynamic head = new
                {
                    id,
                    mainid = saleOrder.OrderNo,
                    flag = 0,
                    createtime = order.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                    paytime = trans.trans.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                    type = 1,
                    status = GetStatus(),
                    quantity = saleOrder.SalesCount,
                    discount = "0.0",
                    total = saleOrder.SalesAmount.ToString(),
                    vipno = string.Empty,
                    vipmemo = string.Empty,
                    storeno,
                    comcount = saleOrder.SalesCount,
                    paycount = 1,
                    oldid = string.Empty,
                    operid = string.Empty,
                    opername = string.Empty,
                    opertime = string.Empty
                };

                var detail = new List<dynamic>();
                var payment = new List<dynamic>();

                foreach (var de in db.OPC_SaleDetail.Where(x => x.SaleOrderNo == saleOrder.SaleOrderNo).Join(db.OPC_Stock, x => x.StockId, s => s.Id, (x, s) => new { detail = x, stock = s }))
                {
                    int idx = 1;
                    payment.Add(new
                    {
                        id = saleOrder.SaleOrderNo,
                        type = GetPaymentType(),
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
                        storeno=de.stock.StoreCode
                    });
                    idx += 1;
                }

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

        private string GetPaymentType()
        {
            return "C0";
        }

        /// <summary>
        /// 找李麒确认
        /// </summary>
        /// <returns></returns>
        private int GetStatus()
        {
            return 1;
        }
    }
}
