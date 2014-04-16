using System;
using System.Collections.Generic;
using Intime.OPC.Domain.Models;

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
                        SalesAmount = saleOrderModel.SalesAmount,
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

                    db.SaveChanges();

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

                return true;
            }
        }
    }
}
