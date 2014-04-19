using Common.Logging;
using Intime.OPC.Domain.Enums;
using Intime.OPC.Domain.Models;
using Intime.OPC.Job.Order.Repository;
using Intime.OPC.Job.Product.ProductSync;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intime.OPC.Job.Order.OrderStatusSync
{

    [DisallowConcurrentExecution]
    public class OrderStatusSyncJob : IJob
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();
        private DateTime benchTime = DateTime.Now.AddMinutes(-3600);
        private readonly IOrderRemoteRepository _remoteRepository = new OrderRemoteRepository();

        private void DoQuery(Action<IQueryable<Intime.OPC.Domain.Models.OPC_Sale>> callback)
        {
            using (var context = new YintaiHZhouContext())
            {
                var minx = context.OPC_Sale.Where(t => t.UpdatedDate > benchTime && t.Status > 0 && t.Status < (int)EnumSaleOrderStatus.SaleCompletion);
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
            var isRebuild = data.ContainsKey("isRebuild") ? data.GetBoolean("isRebuild") : false;
            var interval = data.ContainsKey("intervalOfSecs") ? data.GetInt("intervalOfSecs") : 5 * 60;
             _benchTime = DateTime.Now.AddMinutes(-interval);

            if (!isRebuild)
                benchTime = data.GetDateTime("benchtime");
#endif
            DoQuery(skus =>
            {
                totalCount = skus.Count();
            });
            int cursor = 0;
            int size = 20;
            while (cursor < totalCount)
            {
                List<Intime.OPC.Domain.Models.OPC_Sale> oneTimeList = null;
                DoQuery(r => oneTimeList = r.OrderBy(t => t.OrderNo).Skip(cursor).Take(size).ToList());
                foreach (var opc_sale in oneTimeList)
                {
                    Process(opc_sale);          // 同步状态到单品系统
                }
                cursor += size;
            }
        }
        private void Process(Domain.Models.OPC_Sale opc_Sale)
        {
            var dataResult = _remoteRepository.GetOrderStatusById(opc_Sale);
            if (dataResult == null)
            {
                Log.InfoFormat("订单信息查询失败,orderNo:{0},status:{1}", opc_Sale.SaleOrderNo, opc_Sale.Status);
                return;
            }
            //订单状态为
            int status = (int)EnumSaleOrderStatus.NotifyProduct;
            int cashStatus = (int)EnumCashStatus.NoCash;
            switch (dataResult.status)
            {
                case "1":        //导购确认提货
                    status=(int)EnumSaleOrderStatus.ShoppingGuidePickUp;
                    break;
                case  "2":      // 完成收银的
                    cashStatus = (int)EnumCashStatus.CashOver;
                    break;
                default:
                    status = (int)EnumSaleOrderStatus.NotifyProduct;
                    break;
            }

            //判断是否需要更新状态
            if (status == (int)EnumSaleOrderStatus.NotifyProduct && cashStatus == (int)EnumCashStatus.NoCash)
            {
                Log.InfoFormat("单品系统状态没有更新,orderNo:{0},status:{1}", opc_Sale.SaleOrderNo, opc_Sale.Status);
                return;
            }
                
            using (var db = new YintaiHZhouContext())
            {
                var saleOrder = db.OPC_Sale.FirstOrDefault(a => a.SaleOrderNo == opc_Sale.SaleOrderNo);

                if (status >(int)EnumSaleOrderStatus.NotifyProduct)
                    saleOrder.Status = status;

                if (cashStatus == (int)EnumCashStatus.CashOver)
                {
                    saleOrder.CashStatus = (int)EnumCashStatus.CashOver;
                    saleOrder.CashDate = DateTime.Now;
                    saleOrder.CashNum = dataResult.posseqno;
                }
                saleOrder.UpdatedDate = DateTime.Now;
                saleOrder.UpdatedUser = -100;
                db.SaveChanges();
            }
            Log.InfoFormat("完成订单状态更新,orderNo:{0},status:{1}", opc_Sale.OrderNo, opc_Sale.Status );

        }
        #endregion
    }
}
