using com.intime.fashion.data.tmall.Models;
using com.intime.o2o.data.exchange.Ims.Domain;
using com.intime.o2o.data.exchange.Ims.Request;
using com.intime.o2o.data.exchange.IT;
using System;
using System.Collections.Generic;
using System.Linq;
using Top.Api;
using Top.Api.Request;

namespace com.intime.fashion.data.sync.Tmall.Executor
{
    public class LogisticsExecutor : ExecutorBase
    {
        private ITopClient _topClient;
        private IApiClient _imsApiClient;
        public LogisticsExecutor(DateTime benchTime, int pageSize, ITopClient topClient, IApiClient apiClient)
            : base(benchTime, pageSize)
        {
            _topClient = topClient;
            _imsApiClient = apiClient;
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
                try
                {
                    Batch(oneTimeList);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                }
                lastCursor = oneTimeList.Max(x => x.Id);
                cursor += _pageSize;
            }
        }

        /// <summary>
        /// 批处理订单号，通过订单状态接口查询订单状态及物流信息
        /// </summary>
        /// <param name="oneTimeList"></param>
        private void Batch(IEnumerable<OrderSync> oneTimeList)
        {
            var orderIds = oneTimeList.Select(x => x.ImsOrderNo);
            // todo 获取订单状态
            var request = new QueryOrderStatusRequest() { Data = orderIds };
            var response = _imsApiClient.Post(request);
            if (!response.Data.Any())
            {
                return;
            }
            if (response.Status)
            {
                SyncLogistics(oneTimeList, response.Data);
            }
            else
            {
                // log an error here!
                Logger.ErrorFormat("Query order status error. orderno is ({0})", string.Join(",", oneTimeList.Select(x => x.ImsOrderNo)));
            }
        }

        /// <summary>
        /// 同步物流信息
        /// </summary>
        /// <param name="oneTimeList"></param>
        /// <param name="logisticStatus"></param>
        private void SyncLogistics(IEnumerable<OrderSync> oneTimeList, IEnumerable<LogisticStatus> logisticStatus)
        {
            foreach (var orderSync in oneTimeList)
            {
                try
                {
                    SyncOne(orderSync, logisticStatus.FirstOrDefault(x => x.OrderNo == orderSync.ImsOrderNo));
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                }
            }
        }

        /// <summary>
        /// 使用top接口回传子订单物流信息
        /// </summary>
        /// <param name="orderSync"></param>
        /// <param name="logistic"></param>
        private void SyncOne(OrderSync orderSync, LogisticStatus logistic)
        {

            foreach (var express in logistic.Logistics.GroupBy(x => string.Format("{0}/{1}{2}", x.StoreId, x.LogisticCode, x.CompanyId)))
            {
                var ship = express.FirstOrDefault();
                if(ship == null) continue;
                var request = new LogisticsOfflineSendRequest
                {
                    CompanyCode = GetCompanyCode(express.FirstOrDefault()),
                    OutSid =ship.LogisticCode,
                    Tid = long.Parse(orderSync.TmallOrderId.ToString()),
                    SubTid = GetSubOrderIdStr(express),
                    CancelId = GetCancelId(ship.StoreId)
                };

                var rsp = _topClient.Execute(request);
                if (rsp.IsError)
                {
                    Logger.Error(rsp);
                }
                else
                {
                    MarkSubOrders(orderSync, express.AsEnumerable());
                }
            }
            
            SetOrderLogisticsSynced(orderSync);
        }

        /// <summary>
        /// 获取门店对应的发货地址ID
        /// </summary>
        /// <param name="storeId"></param>
        /// <returns></returns>
        private long? GetCancelId(int storeId)
        {
            using (var db = DbContextHelper.GetJushitaContext())
            {
                var map = db.Set<LogisticsAddressMapping>().FirstOrDefault(x => x.StoreId == storeId);
                if (map == null)
                {
                    return null;
                }
                return map.TmallAddressId;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="orderSync"></param>
        /// <param name="express"></param>
        private void MarkSubOrders(OrderSync orderSync, IEnumerable<Logistic> express)
        {
            using (var db = DbContextHelper.GetJushitaContext())
            {
                foreach (var logistic in express)
                {
                    int stockId = logistic.StockId;
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

        /// <summary>
        /// 获取同一个快递包裹里所有的子订单号
        /// </summary>
        /// <param name="stockId"></param>
        /// <returns></returns>
        private string GetSubOrderIdStr(IEnumerable<Logistic> logistics)
        {
            var stockIds = logistics.Select(x => x.StockId).ToList();
            using (var db = DbContextHelper.GetJushitaContext())
            {
                return string.Join(",", db.Set<SubOrder>().Where(x => stockIds.Contains(x.ImsInventoryId)).Select(x=>x.TmallSubOrderId));                
            }
        }

        /// <summary>
        /// 天猫规则，如果是天猫签约物流公司，则传递签约公司code，否则传物流公司名称及相应运单号
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private string GetCompanyCode(Logistic item)
        {
            using (var db = DbContextHelper.GetJushitaContext())
            {
                int expressId = item.CompanyId;
                var logistics = db.Set<ShipViaMapping>().FirstOrDefault(x => x.ImsShipViaId == expressId && x.Channel == "tmall");
                if (logistics == null)
                {
                    return item.CompanyName;
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
