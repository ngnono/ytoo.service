using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Intime.OPC.Domain.Dto;
using Intime.OPC.Domain.Dto.Custom;
using Intime.OPC.Domain.Enums;
using Intime.OPC.Domain.Models;
using Intime.OPC.Repository.Base;

namespace Intime.OPC.Repository.Support
{
    public class RMARepository : BaseRepository<OPC_RMA>, IRMARepository
    {
        public IList<OPC_RMA> GetByReturnGoods(ReturnGoodsInfoGet request)
        {
            //todo 为实现
            return null;

            //using (var db = new YintaiHZhouContext())
            //{
                
            //}
        }

        public IList<RMADto> GetAll(string orderNo, string saleOrderNo, DateTime startTime, DateTime endTime, EnumRMAStatus rmaStatus, EnumReturnGoodsStatus returnGoodsStatus)
        {
            int status = rmaStatus.AsID();
            string des = returnGoodsStatus.GetDescription();
            using (var db=new YintaiHZhouContext())
            {
                var query = db.OPC_RMA.Where(t => t.CreatedDate >= startTime && t.CreatedDate < endTime);
                if (orderNo.IsNotNull())
                {
                    query = query.Where(t => t.OrderNo.Contains(orderNo));
                }
                if (saleOrderNo.IsNotNull())
                {
                    query = query.Where(t => t.SaleOrderNo.Contains(saleOrderNo));
                }


                var lst = query.Join(db.OPC_SaleRMA.Where(e => e.Status == status && e.RMAStatus == des), o => o.RMANo, t => t.RMANo, (t, o) => new { Rma = t, SaleRma = o })
                .Join(db.Stores, t => t.Rma.StoreId, o => o.Id, (t, o) => new { Rma = t.Rma, StoreName = o.Name, SaleRma = t.SaleRma })
                .Join(db.OPC_Sale, t => t.Rma.SaleOrderNo, o => o.SaleOrderNo, (t, o) => new { Rma = t.Rma,StoreName = t.StoreName, SaleRma=t.SaleRma,Sale=o })
                .Join(db.Orders, t => t.Rma.OrderNo, o => o.OrderNo, (t, o) => new { Rma = t.Rma, StoreName = t.StoreName, SaleRma = t.SaleRma, Sale = t.Sale,payTyp=o.PaymentMethodName }).ToList();

               var lstSaleRma = new List<RMADto>();
               foreach (var t in lst)
               {
                   var o = new RMADto();
                   o.Id = t.Rma.Id;
                   o.OrderNo = t.Rma.OrderNo;
                   o.SaleOrderNo = t.Rma.SaleOrderNo;
                   o.CashDate = t.Sale.CashDate;
                   o.CashNum = t.Sale.CashNum;
                   o.Count = t.Rma.Count;
                   o.CreatedDate = t.Rma.CreatedDate;
                   o.RMAAmount = t.Rma.RMAAmount;
                   o.RMANo = t.Rma.RMANo;
                   o.BackDate = t.SaleRma.BackDate;

                   //o.RMAReason = t.Rma.re;
                   o.RMAType = t.Rma.RMAType;
                   o.RefundAmount = t.Rma.RefundAmount;
                   o.RmaCashDate = t.Rma.RmaCashDate;
                   o.PayType = t.payTyp;
                   o.RmaCashStatusName = t.SaleRma.RMACashStatus;
                   EnumRMAStatus status2 = (EnumRMAStatus) (t.Rma.Status);
                   o.StatusName =status2.GetDescription();
                   o.SourceDesc = t.Rma.SourceDesc;
                   o.RmaStatusName = t.SaleRma.RMAStatus;
                   o.StoreName = t.StoreName;
                   o.专柜码 = "";

                   lstSaleRma.Add(o);
               }

                return lstSaleRma;
            }
        }
    }
}