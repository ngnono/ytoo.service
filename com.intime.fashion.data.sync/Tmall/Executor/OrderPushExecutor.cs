﻿using System.Transactions;
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
                        Logger.Error(ex);
                    }
                    ProcessSyncResult(result);
                }

                cursor += _pageSize;
                lastCursor = oneTimeList.Max(o => o.tid);
            }
        }

       /// <summary>
       /// 处理成功后要记录在订单映射表
       /// </summary>
       /// <param name="result"></param>
        private void ProcessSyncResult(SyncResult result)
        {
            using (var db = DbContextHelper.GetJushitaContext())
            {
                if (result.Succeed)
                {
                    using (var trans = new TransactionScope())
                    {
                        if (!db.Set<OrderSync>().Any(x => x.TmallOrderId == result.TmalOrderId))
                        {
                            //天猫订单和ims订单映射关系
                            db.Set<OrderSync>().Add(new OrderSync()
                            {
                                CreateDate = DateTime.Now,
                                TmallOrderId = result.TmalOrderId,
                                ImsOrderNo = result.TargetOrderNo,
                                Type = (int)OrderType.Order,
                                UpdateDate = DateTime.Now,
                                LogisticsSynced = false
                            });

                            // 子订单映射到ims库存，以便物流信息回传使用
                            foreach (var subOrder in result.TmallOrder.orders.order)
                            {
                                db.Set<SubOrder>().Add(new SubOrder()
                                {
                                    CreateDate = DateTime.Now,
                                    IsForceSynced = false,
                                    LogisticsSynced = false,
                                    Store = null,
                                    TmallOrderId = result.TmalOrderId,
                                    TmallSubOrderId = subOrder.oid,
                                    UpdateDate = DateTime.Now,
                                    ImsInventoryId = subOrder.outer_sku_id
                                });
                            }
                            db.SaveChanges();
                            trans.Complete();
                        }
                    }
                }
                else
                {
                    var failedLog = db.Set<OrderSynchErrorLog>()
                        .FirstOrDefault(x => x.TmallOrderId == result.TmalOrderId);
                    if (failedLog != null)
                    {
                        failedLog.FailedCount += 1;
                        db.SaveChanges();
                    }
                    else
                    {
                        failedLog = new OrderSynchErrorLog()
                        {
                            CreateDate = DateTime.Now,
                            FailedCount = 1,
                            Reason = result.Exception != null ? result.Exception.Message : result.FailedReason,
                            Status = 1,
                            TmallOrderId = result.TmalOrderId,
                            Type = (int)OrderType.Order
                        };
                        db.Set<OrderSynchErrorLog>().Add(failedLog);
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
            var tmallOrder = JsonConvert.DeserializeObject<dynamic>(trade.jdp_response);
            dynamic imsOrder = new
            {
                sonumber = tmallOrder.tid,
                transno = tmallOrder.tid,
                totalAmount = tmallOrder.total_fee,
                receivedAmount = tmallOrder.payment,
                paymentMehtodCode = "C019",//todo 支付编码未确定
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

            //todo 支付编码未确定
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

            return SyncResult.SucceedResult(null, tmallOrder.tid, tmallOrder);
        }

        public override OrderType OrderType
        {
            get { return OrderType.Order; }
        }
    }
}
