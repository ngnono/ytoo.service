﻿using System;
using System.Linq.Expressions;
using Intime.OPC.Domain.Dto;
using Intime.OPC.Domain.Dto.Request;
using Intime.OPC.Domain.Enums;
using Intime.OPC.Domain.Exception;
using Intime.OPC.Domain.Models;
using Intime.OPC.Service.Contract;
using System.Collections.Generic;
using System.Linq;

namespace Intime.OPC.Service.Impl
{
    public class SaleOrderService : ISaleOrderService
    {
        public IList<SaleDetailDto> GetSaleDetails(string saleOrderNo)
        {
            using (var db = new YintaiHZhouContext())
            {
                return
                    db.OPC_SaleDetail.Where(o => o.SaleOrderNo == saleOrderNo)
                        .Join(db.OrderItems, d => d.OrderItemId, i => i.Id, (o, i) => new { o, i })
                        .GroupJoin(db.Brands.Where(b => b.Status == 1), o => o.i.BrandId, b => b.Id, (o, b) => new
                        {
                            detail = o.o,
                            item = o.i,
                            brand = b.OrderByDescending(x => x.Status).FirstOrDefault()
                        })
                        .Join(db.Products, o => o.item.ProductId, p => p.Id, (o, p) => new SaleDetailDto
                        {
                            Id = o.detail.Id,
                            ProductNo = p.SkuCode,
                            SaleOrderNo = o.detail.SaleOrderNo,
                            StyleNo = p.SkuCode,
                            Size = o.item.SizeValueName,
                            Color = o.item.ColorValueName,
                            Brand = o.brand != null ? o.brand.Name : string.Empty,
                            SellPrice = o.item.ItemPrice,
                            Price = o.item.UnitPrice.HasValue ? o.item.UnitPrice.Value : (decimal)0.0,
                            SalePrice = o.item.ItemPrice,
                            SellCount = o.detail.SaleCount,
                            ReturnCount = o.detail.BackNumber.HasValue ? o.detail.BackNumber.Value : 0,
                            LabelPrice = o.item.UnitPrice.HasValue ? o.item.UnitPrice.Value : (decimal)0.0,
                            Remark = o.detail.Remark,
                            SectionCode = o.detail.SectionCode,
                            ProductName = o.item.ProductName
                        }).ToList();
            }
        }

        public IList<OPC_SaleComment> GetSaleComments(string saleOrderNo)
        {
            using (var db = new YintaiHZhouContext())
            {
                return db.OPC_SaleComment.Where(x => x.SaleOrderNo == saleOrderNo).ToList();
            }
        }

        public void CommentSaleOrder(string comment, string saleOrderNo, int uid)
        {
            using (var db = new YintaiHZhouContext())
            {
                db.OPC_SaleComment.Add(new OPC_SaleComment
                {
                    CreateUser = uid,
                    CreateDate = DateTime.Now,
                    Content = comment,
                    SaleOrderNo = saleOrderNo,
                    UpdateDate = DateTime.Now,
                    UpdateUser = uid,
                });
            }
        }

        public IList<OPC_Sale> GetSaleOrdersByPachageId(int packageId)
        {
            using (var db = new YintaiHZhouContext())
            {
                return db.OPC_Sale.Where(x => x.ShippingSaleId.HasValue && x.ShippingSaleId.Value == packageId).ToList();
            }
        }

        public IList<OPC_SaleComment> GetSaleComments(string saleOrderNo, int uid)
        {
            using (var db = new YintaiHZhouContext())
            {
                return db.OPC_SaleComment.Where(x => x.SaleOrderNo == saleOrderNo).ToList();
            }
        }

        public IList<SaleDto> QuerySaleOrders(SaleOrderQueryRequest request, int uid)
        {
            using (var db = new YintaiHZhouContext())
            {
                var user =
                    db.OPC_AuthUser.Where(x => x.Id == uid)
                        .Join(db.OPC_OrgInfo, u => u.OrgId, o => o.OrgID, (u, o) => new { user = u, org = o })
                        .FirstOrDefault();
                if (user == null)
                {
                    throw new OpcExceptioin("未授权的用户");
                }

                Expression<Func<OPC_Sale, bool>> whereCondition =
                    sale => sale.CreatedDate > request.StartDate && sale.CreatedDate < request.EndDate;

                if (!string.IsNullOrEmpty(request.SaleOrderNo))
                {
                    whereCondition = whereCondition.And(sale => sale.SaleOrderNo == request.SaleOrderNo);
                }

                if (!string.IsNullOrEmpty(request.OrderNo))
                {
                    whereCondition = whereCondition.And(sale => sale.OrderNo == request.OrderNo);
                }

                if (!user.user.IsSystem)
                {
                    return
                        db.OPC_Sale.Where(whereCondition)
                            .Join(db.Sections.Where(s => s.StoreId == user.org.StoreOrSectionID)
                            .Join(db.Stores,s=>s.StoreId,x=>x.Id,(section,store)=>new{section,store}), o => o.SectionId, s => s.section.Id, (o, s) => new{o,store=s})
                            .Join(db.OrderTransactions, o => o.o.OrderNo, ot => ot.OrderNo, (o, ot) => new { o, ot })                       
                            .Join(db.PaymentMethods, ot => ot.ot.PaymentCode, pm => pm.Code, (ot, pm) => new SaleDto()
                            {
                                Id = ot.o.o.Id,
                                OrderNo = ot.o.o.OrderNo,
                                SaleOrderNo = ot.o.o.SaleOrderNo,
                                SalesType = ot.o.o.SalesType,
                                ShipViaId = ot.o.o.ShipViaId,
                                Status = ot.o.o.Status,
                                ShippingCode = ot.o.o.ShippingCode,
                                ShippingFee = ot.o.o.ShippingFee.HasValue ? ot.o.o.ShippingFee.Value : (decimal)0,
                                ShippingStatus = ot.o.o.ShippingStatus,
                                ShippingStatusName = ot.o.o.ShippingStatus.HasValue ? ((EnumSaleOrderStatus)ot.o.o.ShippingStatus.Value).GetDescription() : string.Empty,
                                ShippingRemark = ot.o.o.ShippingRemark,
                                SellDate = ot.ot.CreateDate,
                                IfTrans = ot.o.o.IfTrans.HasValue && ot.o.o.IfTrans.Value ? "是" : "否",
                                TransStatus = ot.o.o.TransStatus.HasValue && ot.o.o.TransStatus.Value == 1 ? "调拨" : "",
                                SalesAmount = ot.o.o.SalesAmount,
                                SalesCount = ot.o.o.SalesCount,
                                CashStatus = ot.o.o.CashStatus,
                                CashNum = ot.o.o.CashNum,
                                CashDate = ot.o.o.CashDate,
                                SectionId = ot.o.o.SectionId,
                                PrintTimes = ot.o.o.PrintTimes,
                                Remark = ot.o.o.Remark,
                                RemarkDate = ot.o.o.RemarkDate,
                                //CreatedDate = ot.o.CreatedDate,
                                //CreatedUser = ot.o.CreatedUser,
                                //UpdatedDate = ot.o.UpdatedDate.Value,
                                //UpdatedUser = ot.o.UpdatedUser.Value,
                                StatusName = ((EnumSaleOrderStatus)ot.o.o.Status).GetDescription(),
                                CashStatusName = ot.o.o.CashStatus.HasValue ? ((EnumCashStatus)ot.o.o.CashStatus).GetDescription():string.Empty,
                                StoreName = ot.o.store.store.Name,
                                //InvoiceSubject = ot.o.o.
                                SectionName = ot.o.store.section.Name,
                                TransNo = ot.ot.TransNo,
                            }).ToList();
                }
                throw new NotImplementedException();
                //return db.OPC_Sale.Where(whereCondition).ToList();
            }
        }
    }
}