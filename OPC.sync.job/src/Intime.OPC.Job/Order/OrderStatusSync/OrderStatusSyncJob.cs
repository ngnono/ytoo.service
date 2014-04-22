using Common.Logging;
using Intime.OPC.Domain.Models;
using Intime.OPC.Job.Product.ProductSync;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Intime.OPC.Job.Order.OrderStatusSync
{
    /// <summary>
    /// 根据销售单状态反向写入订单状态
    /// </summary>

    [DisallowConcurrentExecution]
    public class OrderStatusSyncJob : IJob
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();
        private DateTime _benchTime = DateTime.Now.AddDays(-1);
        
        public void DoQuery(Action<IQueryable<Domain.Models.Order>> callback)
        {
            using (var context = new YintaiHZhouContext())
            {
                var linq =
                    context.Orders.Where(
                        o => o.Status != 0 && o.Status != -10 && o.Status != 10 && o.Status != 19 && o.Status != 18 && o.CreateDate >= _benchTime);
                if (callback != null)
                    callback(linq);
            }
        }

        #region IJob 成员

        public void Execute(IJobExecutionContext context)
        {
            var totalCount = 0;
#if !DEBUG
            JobDataMap data = context.JobDetail.JobDataMap;
            var isRebuild = data.ContainsKey("isRebuild") && data.GetBoolean("isRebuild");
            var interval = data.ContainsKey("intervalOfMins") ? data.GetInt("intervalOfMins") : 5 * 60;
             _benchTime = DateTime.Now.AddMinutes(-interval);

            if (isRebuild)
                _benchTime = _benchTime.AddMonths(-3);
#endif
            DoQuery(skus =>
            {
                totalCount = skus.Count();
            });
            int cursor = 0;
            int size = 20;
            while (cursor < totalCount)
            {
                List<Domain.Models.Order> oneTimeList = null;
                DoQuery(r => oneTimeList = r.OrderBy(t => t.OrderNo).Skip(cursor).Take(size).ToList());
                foreach (var order in oneTimeList)
                {
                    SetOrderStatus(order);
                }
                cursor += size;
            }
        }

        /// <summary>
        /// 根据销售单状态反向写入订单状态，由同一订单下所有发货单状态来决策出订单状态
        /// 订单如果发货了，则需要同步发货状态及发货包裹,插入发货日志
        /// 目前实现有很大的问题 wxh备注 2014-04-20 0:17:43
        /// </summary>
        /// <param name="opcSale"></param>
        private void SetOrderStatus(Domain.Models.Order order)
        {
            using (var db = new YintaiHZhouContext())
            {
                var p = db.Orders.FirstOrDefault(t => t.OrderNo == order.OrderNo);
                var saleOrders = db.OPC_Sale.Where(sa => sa.OrderNo == p.OrderNo).ToList();
                var status = CheckStatus(saleOrders);
                if (status == order.Status || status == NoStatus)
                    return;
                p.UpdateDate = DateTime.Now;
                p.UpdateUser = SystemDefine.SystemUser;
                db.SaveChanges();
                Log.InfoFormat("完成订单状态更新,orderNo:{0},status:{1}", p.OrderNo, SystemDefine.OrderFinishSplitStatusCode);
            }
        }

        private const int NoStatus = -10000;
        private const int Shipped = 15;
        private const int PreparePack = 14;
        private const int OrderPrinted = 13;
        private const int AgentConfirmed = 11;

        private int CheckStatus(IEnumerable<OPC_Sale> saleOrders)
        {
            if (saleOrders == null || !saleOrders.Any())
            {
                return NoStatus;
            }
            //已发货
            if (saleOrders.Any(x => x.Status == 40))
            {
                return Shipped;
            }
            if (saleOrders.Any(x => x.Status == 35))
            {
                return PreparePack;
            }

            if (saleOrders.Any(x => x.Status == 30))
            {
                return OrderPrinted;
            }
            if (saleOrders.Any(x => x.Status == 25 || x.Status == 20 || x.Status == 15))
            {
                return OrderPrinted;
            }

            if (saleOrders.Any(x => x.Status == 2 || x.Status == 21))
            {
                return AgentConfirmed;
            }
            return NoStatus;
        }

        #endregion
    }
}
