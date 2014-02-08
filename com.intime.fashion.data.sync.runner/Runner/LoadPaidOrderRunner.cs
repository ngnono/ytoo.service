using com.intime.fashion.data.sync.Wgw;
using com.intime.fashion.data.sync.Wgw.Executor;
using System;
using System.Collections.Generic;
using System.Linq;
using Yintai.Hangzhou.Data.Models;
using Yintai.Hangzhou.Service.Logic;

namespace com.intime.fashion.data.sync.runner.Runner
{
    public class LoadPaidOrderRunner : Runner
    {
        protected override void Do()
        {
            var benchTime = DateTime.Now.AddDays(-30);
            ExecuteResult executeInfo = null;

            var productSyncExecutor = new OrderSyncExecutor(benchTime, Logger);
            executeInfo = productSyncExecutor.Execute();

            if (executeInfo.Status != ExecuteStatus.Succeed)
            {
                foreach (var msg in executeInfo.MessageList)
                {
                    Logger.Error(msg);
                }
            }
            else
            {
                this.Execute(benchTime);
                Console.WriteLine("同步订单,成功{0},失败{1},总数{2}", executeInfo.SucceedCount, executeInfo.FailedCount, executeInfo.TotalCount);
            }
        }

        private void Query(DateTime benchTime, Action<IQueryable<Map4Order>> callback)
        {
            using (var db = new YintaiHangzhouContext("YintaiHangzhouContext"))
            {
                var orders =
                    db.Set<Map4Order>()
                        .Where(
                            m =>
                                m.CreateDate >= benchTime && m.Channel == ConstValue.WGW_CHANNEL_NAME &&
                                !db.Set<Order2ExEntity>().Any(o => o.OrderNo == m.OrderNo));

                if (callback != null)
                    callback(orders);
            }
        }

        private void Execute(DateTime benchTime)
        {
            var totalCount = 0;
            Query(benchTime, orders => { totalCount = orders.Count(); });
            int cursor = 0;
            int successCount = 0;
            const int size = 10;
            int lastCursor = 0;

            while (cursor < totalCount)
            {
                List<Map4Order> oneTimeList = null;
                Query(benchTime, orders =>
                {
                    oneTimeList = orders.Where(o => o.Id > lastCursor).OrderBy(a => a.Id).Take(size).ToList();
                });
                foreach (var order in oneTimeList)
                {
                    try
                    {
                        if (OrderRule.SyncPaidOrder2Erp(order.OrderNo))
                        {
                            successCount++;
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex);
                    }
                }
                cursor += size;
                lastCursor = oneTimeList.Max(o => o.Id);
            }
        }
    }
}
