using com.intime.fashion.data.tmall.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Top.Api;
using Top.Api.Domain;
using Top.Api.Request;

namespace com.intime.fashion.data.sync.Tmall.Executor
{
    public class LogisticsExecutor : ExecutorBase
    {
        private ITopClient _client;
        public LogisticsExecutor(DateTime benchTime, int pageSize,ITopClient topClient)
            : base(benchTime, pageSize)
        {
            _client = topClient;
        }

        public override void Execute()
        {
            int totalCount = 0;
            int cursor = 0;
            int lastCursor = 0;
            DoQuery(orders => totalCount = orders.Count());

            while (cursor < totalCount)
            {
                List<OrderSync> oneTimeList = null;
                DoQuery(orders =>
                        oneTimeList = orders.Where(o => o.Id > lastCursor).OrderBy(o => o.Id).Take(_pageSize).ToList());
                BatchProcess(oneTimeList);
            }
        }

        private void BatchProcess(IEnumerable<OrderSync> oneTimeList)
        {
            var orderIds = oneTimeList.Select(x => x.ImsOrderNo);
            // todo 获取订单状态
        }

        private void Mark(IEnumerable<OrderSync> oneTimeList, IEnumerable<dynamic> result)
        {
            foreach (var orderSync in oneTimeList)
            {
                SyncOne(orderSync, result.FirstOrDefault(x => x.OrderNo == orderSync.ImsOrderNo));
            }
        }

        private void SyncOne(OrderSync orderSync, dynamic rsp)
        {
            foreach (var item in rsp.itemStatus)
            {
                int stockId = item.stockId;
                var request = new LogisticsOfflineSendRequest
                {
                    CompanyCode = GetCompanyCode(item),
                    OutSid = item.ShippingCode,
                    Tid = long.Parse(orderSync.TmallOrderId.ToString()),
                    SubTid = GetSubOrderId(item.stockId)
                };

                var response = _client.Execute(request);
                if (response.IsError)
                {
                    Logger.Error(response.ErrMsg);
                }
                else
                {
                    using (var db = DbContextHelper.GetJushitaContext())
                    {
                        //todo here log logistics
                        var subOrder =
                            db.Set<SubOrder>().FirstOrDefault(x => x.TmallOrderId == orderSync.TmallOrderId && x.ImsInventoryId == stockId);
                        if (subOrder != null && !subOrder.LogisticsSynced)
                        {
                            subOrder.LogisticsSynced = true;
                            subOrder.UpdateDate = DateTime.Now;
                            db.SaveChanges();
                        }
                    }
                }
            }

            SetOrderLogisticsSynced(orderSync);
        }

        /// <summary>
        /// 如果子订单全部发货，则订单状态置为发货状态，此处有个bug，如果子订单是多家门店发货，则可能会导致未必全部门店都发货而置订单为已发货状态。暂时不处理
        /// </summary>
        /// <param name="orderSync"></param>
        private void SetOrderLogisticsSynced(OrderSync orderSync)
        {
            using (var db = DbContextHelper.GetJushitaContext())
            {
                if (!db.Set<SubOrder>().Any(x => x.TmallOrderId == orderSync.TmallOrderId && !x.LogisticsSynced))
                {
                    var order = db.Set<OrderSync>().FirstOrDefault(x => x.TmallOrderId == orderSync.TmallOrderId);

                    if (order != null && !order.LogisticsSynced)
                    {
                        order.LogisticsSynced = true;
                        order.UpdateDate = DateTime.Now;
                        db.SaveChanges();
                    }
                }
            }
        }

        private string GetSubOrderId(int stockId)
        {
            using (var db = DbContextHelper.GetJushitaContext())
            {
                var subOrder = db.Set<SubOrder>().FirstOrDefault(x => x.ImsInventoryId == stockId);
                if (subOrder == null)
                {
                    throw new ArgumentException(string.Format("Invalid stockId ({0})",stockId));
                }
                return subOrder.TmallSubOrderId.ToString();
            }
        }

        private string GetCompanyCode(dynamic item)
        {
            using (var db = DbContextHelper.GetJushitaContext())
            {
                int expressId = item.expressId;
                var logistics = db.Set<ShipViaMapping>().FirstOrDefault(x => x.ImsShipViaId == expressId && x.Channel == "tmall");
                if (logistics == null)
                {
                    return item.ShipViaName;
                }
                return logistics.CompanyCode;
            }
        }

        protected void DoQuery(Action<IQueryable<OrderSync>> callback)
        {
            using (var context = DbContextHelper.GetJushitaContext())
            {
                var linq =
                    context.Set<OrderSync>().Where(x => x.CreateDate >= _benchDateTime && !x.LogisticsSynced);
                if (callback != null)
                    callback(linq);
            }
        }
    }
}
