using System;
using System.Collections.Generic;
using System.Linq;
using Intime.OPC.Domain.Models;
using System.Transactions;

namespace Intime.OPC.Job.Trade.SplitOrder.Supports
{
    /// <summary>
    /// 默认销售单存储实现
    /// </summary>
    public class DefaultSaleOrderStore : ISaleOrderStore
    {
        public bool Save(IEnumerable<Models.SaleOrderModel> saleOrders)
        {
            using (var db = new YintaiHZhouContext())
            {
                using (var ts = new TransactionScope())
                {
                    // 遍历保存销售单
                    foreach (var saleOrderModel in saleOrders)
                    {
                        // TODO：检查订单是否已经被拆分，防止两个程序启动写入双份
                        var saleOrder = new OPC_Sale()
                        {
                            OrderNo = saleOrderModel.OrderNo,
                            SaleOrderNo = saleOrderModel.SaleOrderNo,
                            SalesType = 0,
                            Status = saleOrderModel.Status,
                            SellDate = saleOrderModel.SellDate,
                            SalesAmount = saleOrderModel.SalesAmount, //是明细累加出的总金额
                            SalesCount = saleOrderModel.SalesCount,
                            SectionId = saleOrderModel.SectionId,
                            CreatedDate = saleOrderModel.CreatedDate,
                            CreatedUser = saleOrderModel.CreatedUser,
                            UpdatedDate = saleOrderModel.CreatedDate,
                            UpdatedUser = saleOrderModel.CreatedUser,
                            CashDate = SplitOrderUtils.GetDefaultDateTime(),
                            RemarkDate = SplitOrderUtils.GetDefaultDateTime()
                        };
                        db.OPC_Sale.Add(saleOrder);

                        // 保存销售单详情
                        foreach (var saleDetail in saleOrderModel.Items)
                        {
                            saleDetail.UpdatedDate = DateTime.UtcNow;
                            saleDetail.Remark = String.Empty;
                            saleDetail.UpdatedUser = SystemDefine.SysUserId;
                            db.OPC_SaleDetail.Add(saleDetail);
                        }
                    }
                    db.SaveChanges();
                    ts.Complete();
                }
                return true;
            }
        }

        /// <summary>
        /// 保存拆单失败的订单
        /// </summary>
        /// <param name="orderNo"></param>
        /// <param name="ErrorReason"></param>
        /// <returns></returns>
        public bool SaveSplitOrderLog(string orderNo, string reason,int status)
        {
            using (var db = new YintaiHZhouContext())
            { 
                var opc_OrderSplitLog = new OPC_OrderSplitLog
                {
                    OrderNo=orderNo,
                    Reason = reason,
                    Status = status,
                    CreateDate=DateTime.Now
                };
                db.OPC_OrderSplitLog.Add(opc_OrderSplitLog);
                db.SaveChanges();

            }            
            return true;
        }
    }
}
