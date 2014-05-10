﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Common.Logging;
using Intime.OPC.Domain.Enums;
using Intime.OPC.Domain.Models;
using Intime.OPC.Job.Order.DTO;
using Intime.OPC.Job.Product.ProductSync;

namespace Intime.OPC.Job.Order.OrderStatusSync
{
    public class CashedSaleOrderStatusProcessor : AbstractSaleOrderStatusProcessor
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();
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

                if (string.IsNullOrEmpty(statusResult.Products_SaleCodes))
                {
                    Log.Error("没有销售码信息");
                    return;
                }

                Log.Error("**************************************");
                Log.ErrorFormat(statusResult.Products_SaleCodes);
                Log.Error("**************************************");

                var slices = ParseProductIdAndPosCode(statusResult.Products_SaleCodes);

                foreach (var slice in slices)
                {
                    var productId = slice.Key;
                    var detail =
                        db.OPC_SaleDetail.Where(x => x.SaleOrderNo == saleOrderNo)
                            .Join(db.OPC_Stock.Where(s => s.SourceStockId == productId), d => d.StockId, s => s.Id,
                                (o, s) => o)
                            .FirstOrDefault();
                    if (detail != null)
                    {
                        detail.SectionCode = slice.Value;
                        db.SaveChanges();
                    }
                }
            }
        }

        /// <summary>
        /// 解析信息部给的结构
        /// </summary>
        /// <param name="strPosSeq">信息部给的结构 : productid|comcode,productid|comcode</param>
        /// <returns></returns>
        private IEnumerable<KeyValuePair<string, string>> ParseProductIdAndPosCode(string strPosSeq)
        {
            var slices = strPosSeq.Split(',');
            return from slice in slices select slice.Split('|') into kv where kv.Length == 2 select new KeyValuePair<string, string>(kv[0],kv[1]);
        }
    }
}