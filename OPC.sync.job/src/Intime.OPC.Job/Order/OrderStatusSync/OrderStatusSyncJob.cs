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
        private DateTime _benchTime = DateTime.Now.AddMinutes(-20);

        private void DoQuery(Action<IQueryable<OPC_Sale>> callback)
        {
            using (var context = new YintaiHZhouContext())
            {
                //
                var minx = context.OPC_Sale.Where(t => t.UpdatedDate > _benchTime && t.Status > 2 );//.Min(t=>t.Status);
                
                if (callback != null)
                    callback(minx);
            }
        }

        #region IJob 成员

        public void Execute(IJobExecutionContext context)
        {
            var totalCount = 0;
#if !DEBUG
            JobDataMap data = context.JobDetail.JobDataMap;
            var isRebuild = data.ContainsKey("isRebuild") && data.GetBoolean("isRebuild");
            var interval = data.ContainsKey("intervalOfSecs") ? data.GetInt("intervalOfSecs") : 5 * 60;
             _benchTime = DateTime.Now.AddMinutes(-interval);

            if (!isRebuild)
                _benchTime = data.GetDateTime("benchtime");
#endif
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
                foreach (var opcSale in oneTimeList)
                {
                    SetOrderStatusBySaleOrder(opcSale);
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
        private void SetOrderStatusBySaleOrder(OPC_Sale opcSale)
        {
            using (var db = new YintaiHZhouContext())
            {
                var p = db.Orders.FirstOrDefault(t => t.OrderNo == opcSale.OrderNo);
                switch(opcSale.Status)
                {
                    case 0:
                        p.Status = 0;
                        break;
                    case 5:
                        break;
                    case 10:
                        p.Status = 10;
                        break;
                    case 15:
                        break;
                    case 20:
                        p.Status = 20;
                        break;
                    case 25:
                        p.Status = 25;
                        break;
                    case 30:
                        p.Status = 30;
                        break;
                    case 35:
                        p.Status = 350;
                        break;
                    case 40:
                        p.Status = 40;
                        break;
                    default:
                        p.Status = opcSale.Status;
                        break;

                }

                p.UpdateDate = DateTime.Now;
                p.UpdateUser = SystemDefine.SystemUser;
                db.SaveChanges();
                Log.InfoFormat("完成订单状态更新,orderNo:{0},status:{1}", p.OrderNo, SystemDefine.OrderFinishSplitStatusCode);

            }
        }

        #endregion
    }
}
