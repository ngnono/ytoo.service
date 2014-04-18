using System;
using System.Collections.Generic;
using System.Linq;
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

        /// <summary>
        /// 查询订单是否已经拆过单
        /// </summary>
        /// <param name="orderNo"></param>
        /// <returns></returns>
        public bool SearchSaleOrderByOrderNo(string orderNo)
        {
            using (var db = new YintaiHZhouContext())
            {
                var saleOrder = db.OPC_Sale.FirstOrDefault(x => x.OrderNo == orderNo);
                if (saleOrder!=null)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 保存拆单失败的订单
        /// </summary>
        /// <param name="orderNo"></param>
        /// <param name="ErrorReason"></param>
        /// <returns></returns>
        public bool SaveSplitErrorOder(string orderNo, string ErrorReason)
        {
            using (var db = new YintaiHZhouContext())
            { 
                //建完表，生成实体，实现保存

            }            
            return true;
        }
    }
}
