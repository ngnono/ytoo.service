using Common.Logging;
using Intime.OPC.Domain.Enums;
using Intime.OPC.Domain.Models;
using Intime.OPC.Job.Order.Models;
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
    public class SaleOrderStatusSyncJob : IJob
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();
        private DateTime benchTime = DateTime.Now.AddDays(-5);
        private readonly IOrderRemoteRepository _remoteRepository = new OrderRemoteRepository();

        private void DoQuery(Action<IQueryable<Intime.OPC.Domain.Models.OPC_Sale>> callback)
        {
            using (var context = new YintaiHZhouContext())
            {
                var linq = context.OPC_Sale.Where(t => t.UpdatedDate > benchTime && t.Status > 0 && t.Status < (int)EnumSaleOrderStatus.SaleCompletion);
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
            var isRebuild = data.ContainsKey("isRebuild") ? data.GetBoolean("isRebuild") : false;
            var interval = data.ContainsKey("intervalOfSecs") ? data.GetInt("intervalOfSecs") : 5 * 60;
             _benchTime = DateTime.Now.AddMinutes(-interval);

            if (!isRebuild)
                benchTime = data.GetDateTime("benchtime");
#endif
            DoQuery(saleOrders =>
            {
                totalCount = saleOrders.Count();
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
            OrderStatusResultDto saleStatus = null;
            try
            {
                saleStatus = _remoteRepository.GetOrderStatusById(opc_Sale);
                ProcessSaleOrderStatus(opc_Sale, saleStatus);
            }
            catch (Exception e)
            {
                Log.Error(e);
                return;
            }
        }

        private void ProcessSaleOrderStatus(OPC_Sale saleOrder, OrderStatusResultDto saleStatus)
        {
            var processor = SaleOrderStatusProcessorFactory.Create(int.Parse(saleStatus.Status));
            processor.Process(saleOrder.SaleOrderNo, saleStatus);
        }

        #endregion
    }

    public class SaleOrderStatusProcessorFactory
    {
        public static AbstractSaleOrderStatusProcessor Create(int status)
        {
            switch (status)
            {
                case 31:
                    return new CashedSaleOrderStatusProcessor(EnumSaleOrderStatus.None);
                case 32:
                    return new ShoppingGuidePickUpSaleOrderStatusProcessor(EnumSaleOrderStatus.ShoppingGuidePickUp);
                default: return new NoneOperationStatusProcessor(EnumSaleOrderStatus.None);
            }
        }
    }

    public abstract class AbstractSaleOrderStatusProcessor
    {
        protected EnumSaleOrderStatus _status;
        protected AbstractSaleOrderStatusProcessor(EnumSaleOrderStatus status)
        {
            this._status = status;
        }
        public abstract void Process(string saleOrderNo, OrderStatusResultDto statusResult);
    }

    public class ShoppingGuidePickUpSaleOrderStatusProcessor : AbstractSaleOrderStatusProcessor
    {
        public ShoppingGuidePickUpSaleOrderStatusProcessor(EnumSaleOrderStatus status) : base(status) { }
        public override void Process(string saleOrderNo, OrderStatusResultDto statusResult)
        {
            using (var db = new YintaiHZhouContext())
            {

                var saleOrder = db.OPC_Sale.FirstOrDefault(o => o.SaleOrderNo == saleOrderNo);
                if (saleOrder.Status != (int)EnumSaleOrderStatus.PrintSale) return;
                saleOrder.Status = (int)_status;
                saleOrder.UpdatedDate = DateTime.Now;
                saleOrder.UpdatedUser = -100;
                db.SaveChanges();
            }
        }
    }

    public class CashedSaleOrderStatusProcessor : AbstractSaleOrderStatusProcessor
    {
        public CashedSaleOrderStatusProcessor(EnumSaleOrderStatus status) : base(status) { }
        public override void Process(string saleOrderNo, OrderStatusResultDto statusResult)
        {
            using (var db = new YintaiHZhouContext())
            {
                var saleOrder = db.OPC_Sale.FirstOrDefault(t => t.SaleOrderNo == saleOrderNo);
                saleOrder.CashStatus = (int)EnumCashStatus.Cashed;
                saleOrder.UpdatedDate = DateTime.Now;
                saleOrder.UpdatedUser = SystemDefine.SystemUser;
                saleOrder.CashNum = statusResult.PosSeqNo;
                saleOrder.CashDate = statusResult.PosTime;
                saleOrder.UpdatedUser = -100;
                db.SaveChanges();
            }
        }
    }
}
