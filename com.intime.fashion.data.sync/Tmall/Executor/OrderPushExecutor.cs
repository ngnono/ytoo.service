using com.intime.fashion.data.tmall.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace com.intime.fashion.data.sync.Tmall.Executor
{
    public class OrderPushExecutor : OrderExecutorBase
    {
        public OrderPushExecutor(DateTime benchTime, int pageSize)
            : base(benchTime, pageSize)
        {
        }

        public OrderPushExecutor()
            : this(DateTime.Now.AddMinutes(-10), 50)
        {
        }

        public override void Execute()
        {
            int totalCount = 0;
            int cursor = 0;
            Decimal lastCursor = 0;
            DoQuery(null, orders => totalCount = orders.Count());

            while (cursor < totalCount)
            {
                List<JDP_TB_TRADE> oneTimeList = null;
                DoQuery(null,
                    orders =>
                        oneTimeList = orders.Where(o => o.tid > lastCursor).OrderBy(o => o.tid).Take(_pageSize).ToList());

                foreach (var trade in oneTimeList)
                {
                    SyncResult result = null;
                    try
                    {
                        result = SyncOne(trade);
                    }
                    catch (Exception ex)
                    {
                        result = SyncResult.FailedResult(ex, trade);
                    }
                    ProcessSyncResult(result);
                }

                cursor += _pageSize;
                lastCursor = oneTimeList.Max(o => o.tid);
            }
        }

        private void ProcessSyncResult(SyncResult result)
        {
            using (var db = DbContextHelper.GetJushitaContext())
            {
                if (result.Succeed)
                {
                    if (!db.Set<OrderPushHistory>().Any(x => x.TmallOrderId == result.TmalOrderId))
                    {
                        db.Set<OrderPushHistory>().Add(new OrderPushHistory()
                        {
                            CreateDate = DateTime.Now,
                            TmallOrderId = result.TmalOrderId,
                            ImsOrderNo = result.TargetOrderNo,
                            Type = (int)OrderType.Order,
                            UpdateDate = DateTime.Now
                        });
                        db.SaveChanges();
                    }
                }
                else
                {
                    var failedLog = db.Set<OrderPushErrorLog>()
                        .FirstOrDefault(x => x.TmallOrderId == result.TmalOrderId);
                    if (failedLog != null)
                    {
                        failedLog.FailedCount += 1;
                        db.SaveChanges();
                    }
                    else
                    {
                        failedLog = new OrderPushErrorLog()
                        {
                            CreateDate = DateTime.Now,
                            FailedCount = 1,
                            Reason = result.Exception != null ? result.Exception.Message : result.FailedReason,
                            Status = 1,
                            TmallOrderId = result.TmalOrderId,
                            Type = (int)OrderType.Order
                        };
                        db.Set<OrderPushErrorLog>().Add(failedLog);
                        db.SaveChanges();
                    }
                }
            }
        }

        private SyncResult SyncOne(JDP_TB_TRADE trade)
        {
            if (string.IsNullOrEmpty(trade.jdp_response))
            {
                throw new ArgumentException("Trade response is null");
            }
            var  tmallOrder = JsonConvert.DeserializeObject<dynamic>(trade.jdp_response);
            dynamic imsOrder = new
            {
                sonumber = tmallOrder.tid,
                transno = tmallOrder.tid,
                totalAmount = tmallOrder.total_fee,
                receivedAmount = tmallOrder.payment,
                paymentMehtodCode = "C019",//
                shipingFee = tmallOrder.post_fee,
                memo = tmallOrder.buyer_message,
                paid = true,
                invoice = new
                {
                    sbuject = tmallOrder.invoice_name,
                    detail = tmallOrder.invoice_type,
                    amount = tmallOrder.payment
                },
                contact = new
                {
                    addr = string.Format("{0}{1}{2}", tmallOrder.receiver_state, tmallOrder.receiver_city, tmallOrder.receiver_address),
                    person = tmallOrder.receiver_name,
                    phone = string.IsNullOrEmpty(tmallOrder.receiver_mobile) ? tmallOrder.receiver_mobile : tmallOrder.receiver_phone,
                    zip = tmallOrder.receiver_zip,
                },
                payment = new List<dynamic>(),
                products = new List<dynamic>(),
            };

            imsOrder.payment.Add(new
            {
                code = "",
                name = "",
                amount = tmallOrder.payment,
            });

            foreach (var order in tmallOrder.orders.order)
            {
                imsOrder.products.Add(new
                {
                    stockId = order.outer_sku_id,
                    quantity = order.num,
                    itemPrice = order.price,
                    extendPrice = order.price
                });
            }

            return SyncResult.SucceedResult(null, decimal.MaxValue);
        }

        public override OrderType OrderType
        {
            get { return OrderType.Order; }
        }
    }
}
