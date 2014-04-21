using System.Collections.Generic;
using System.Linq;
using Intime.OPC.Domain.Models;

namespace Intime.OPC.Job.Order.OrderStatusSync
{
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
}