using System;
using System.Linq;
using Intime.OPC.Domain.Enums;
using Intime.OPC.Domain.Models;
using Intime.OPC.Job.Order.DTO;
using Intime.OPC.Job.Product.ProductSync;

namespace Intime.OPC.Job.Order.OrderStatusSync
{
    public class CashedSaleOrderStatusProcessor : AbstractSaleOrderStatusProcessor
    {
        public CashedSaleOrderStatusProcessor(EnumSaleOrderStatus status) : base(status) { }

        /// <summary>
        /// 销售单收银状态从单品系统同步回来，目前未做销售单明细表商品销售码的回写。
        /// 信息部回传格式有问题，需要调整后再依据他们的结果调整。 wxh comment on 2014-04-20 17:45:00
        /// </summary>
        /// <param name="saleOrderNo"></param>
        /// <param name="statusResult"></param>
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