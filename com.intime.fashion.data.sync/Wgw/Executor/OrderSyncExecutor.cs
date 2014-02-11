using System.Linq.Expressions;
using com.intime.fashion.data.sync.Wgw.Request.Builder;
using com.intime.fashion.data.sync.Wgw.Request.Order;
using com.intime.fashion.data.sync.Wgw.Response.Processor;
using Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using Yintai.Hangzhou.Data.Models;
using Yintai.Hangzhou.Model.Enums;

namespace com.intime.fashion.data.sync.Wgw.Executor
{
    public class OrderSyncExecutor:ExecutorBase
    {
        private int _succeedCount = 0;
        private int _failedCount = 0;
        private readonly string _beginTime;
        private readonly string _endTime;
        public OrderSyncExecutor(DateTime benchTime, ILog logger) : this(new SyncClient(), benchTime, logger)
        {
        }

        public OrderSyncExecutor(SyncClient client, DateTime benchTime, ILog logger) : base(client, benchTime, logger)
        {
            _beginTime = BeginTime(benchTime);
            _endTime = EndTime();
        }

        protected override int SucceedCount
        {
            get { return _succeedCount; }
        }

        protected override int FailedCount
        {
            get { return _failedCount; }
        }

        protected override void ExecuteCore(dynamic extraParameter = null)
        {
            int pageSize = extraParameter??20;
            this.SyncPaidOrders(pageSize);
            //this.SyncCancelledOrder(pageSize);
            this.SyncShippedOrder(pageSize);
        }

        private void DoQuery(Expression<Func<Map4Order, bool>> whereCondition, Action<IQueryable<Map4Order>> callback)
        {
            using (var context = DbContextHelper.GetDbContext())
            {
                int shipping = (int) OrderOpera.Shipping;
                int fromCustomer = (int) OrderOpera.FromCustomer;
                var linq =
                    context.OrderLogs.Where(l => l.Type == shipping && l.CreateDate >= BenchTime)
                        .Join(context.Map4Orders.Where(m => m.SyncStatus == fromCustomer), l => l.OrderNo,
                            m => m.OrderNo, (l, m) => m);
                if (whereCondition != null)
                    linq = linq.Where(whereCondition);
                if (callback != null)
                    callback(linq);
            }
        }

        /// <summary>
        /// 同步已发货状态至微购物
        /// </summary>
        /// <param name="pageSize"></param>
        private void SyncShippedOrder(int pageSize)
        {
            int totalCount = 0;
            int cursor = 0;
            int lastCursor = 0;
            DoQuery(null,orders=>totalCount = orders.Count());

            while (cursor < totalCount)
            {
                List<Map4Order> oneTimeList = null;
                DoQuery(null,orders=>oneTimeList = orders.Where(o=>o.Id > lastCursor).OrderBy(o=>o.Id).Take(pageSize).ToList());
                foreach (var map4Order in oneTimeList)
                {
                    try
                    {
                        var builder = RequestParamsBuilderFactory.CreateBuilder(new MarkShippingRequest());
                        var rsp = Client.Execute<dynamic>(builder.BuildParameters(map4Order.ChannelOrderCode));
                        if (rsp.errorCode != 0)
                        {
                            _failedCount += 1;
                            Logger.Error(string.Format("标记订单发货失败,Error Message:{0}", rsp.errorMessage));
                        }
                        else
                        {
                            using (var db = DbContextHelper.GetDbContext())
                            {
                                var mapping = db.Map4Orders.FirstOrDefault(m => m.Id == map4Order.Id);
                                if(mapping == null) continue;
                                mapping.SyncStatus = (int)OrderOpera.Shipping;
                                mapping.UpdateDate = DateTime.Now;
                                db.SaveChanges();
                                _succeedCount += 1;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex.Message);
                        _failedCount += 1;
                    }
                }
                cursor += pageSize;
                lastCursor = oneTimeList.Max(o => o.Id);
            }
        }

        private void SyncCancelledOrder(int pageSize)
        {
            var dict = BuildQueryDict(OrderStatusConst.STATE_WG_CANCLE, pageSize.ToString());
            var request = new QueryOrderListRequest(dict);
            SyncCancelledOrdersFromWgw(request);
        }

        private Dictionary<string, string> BuildQueryDict(string orderStatus,string size)
        {
            return new Dictionary<string, string>
            {
                {"pageSize", size},
                {"pageindex", "1"},
                {"timeBegin",_beginTime},
                {"timeEnd", _endTime},
                {"historyDeal", "0"},
                {"timeType", "PAY"},
                {"listItem", "0"},
                {"orderDesc", "0"},
                {"dealState", orderStatus}
            };
        }

        private void SyncPaidOrders(int size)
        {
#if DEBUG
            var dict = BuildQueryDict(OrderStatusConst.STATE_WG_COMPLEX_WAIT_PAY,size.ToString());
#else
            var dict = BuildQueryDict(OrderStatusConst.STATE_WG_PAY_OK,size.ToString());
#endif
            var request = new QueryOrderListRequest(dict);

            SyncPaidOrdersFromWgw(request);
        }

        /// <summary>
        /// 微购物上取消的订单状态同步下来
        /// </summary>
        /// <param name="request"></param>
        private void SyncCancelledOrdersFromWgw(QueryOrderListRequest request)
        {
            var result = Client.Execute<dynamic>(request);

            if (result.errorCode == 0)
            {
                foreach (var order in result.dealList.dealListVo)
                {
                    var processor = ProcessorFactory.CreateProcessor<CancelOrderProcessor>();
                    if (!processor.Process(order, null))
                    {
                        _failedCount += 1;
                        Logger.Error(processor.ErrorMessage);
                    }
                    else
                    {
                        _succeedCount += 1;
                    }
                }
                var maxPage = result.dealList.maxPageNo;
                var currentPage = result.dealList.currPage;
                while (currentPage < maxPage)
                {
                    request.Put("pageindex", currentPage += 1);
                    request.Remove("sign");
                    SyncCancelledOrdersFromWgw(request);
                }
            }
            Logger.Error(result.errorMessage);
            

        }

        /// <summary>
        /// 同步最新支付的订单
        /// </summary>
        /// <param name="request"></param>
        private void SyncPaidOrdersFromWgw(ISyncRequest request)
        {
            var result = Client.Execute<dynamic>(request);

            if (result.errorCode == 0)
            {
                foreach (var order in result.dealList.dealListVo)
                {
                    var detailRequest = new QueryOrderDetailRequest(order.dealCode.ToString());
                    var detail = Client.Execute<dynamic>(detailRequest);
                    var processor = ProcessorFactory.CreateProcessor<OrderProcessor>();
                    if (!processor.Process(order, detail))
                    {
                        _failedCount += 1;
                        Logger.Error(string.Format("获取订单详情失败{0},Error Message{1}", order.dealCode, processor.ErrorMessage));
                    }
                    else
                    {
                        _succeedCount += 1;
                    }
                }
                var maxPage = result.dealList.maxPageNo;
                var currentPage = result.dealList.currPage;
                while (currentPage < maxPage)
                {
                    request.Put("pageindex", currentPage += 1);
                    request.Remove("sign");
                    SyncPaidOrdersFromWgw(request);
                }
            }
            else
            {
                Logger.Error(string.Format("获取订单列表失败:{0}", result.errorMessage));
            }
        }

        private string BeginTime(DateTime dateTime)
        {
            return TimeStampStr(dateTime);
        }

        private string EndTime()
        {
            return TimeStampStr(DateTime.Now);
        }

        private string TimeStampStr(DateTime dateTime)
        {
            TimeSpan t = dateTime.ToUniversalTime() - new DateTime(1970, 1, 1);
            return ((Int64)t.TotalSeconds).ToString();
        }
    }
}
