using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Intime.OPC.Domain;
using Intime.OPC.Domain.Dto;
using Intime.OPC.Domain.Exception;
using Intime.OPC.Domain.Models;
using Intime.OPC.Repository.Base;

namespace Intime.OPC.Repository.Support
{
    public class SaleRMARepository : BaseRepository<OPC_SaleRMA>, ISaleRMARepository
    {
        public int Count(string orderNo)
        {
            using (var db = new YintaiHZhouContext())
            {
                return db.OPC_SaleRMA.Count(t => t.OrderNo == orderNo);
            }
        }

        public PageResult<SaleRmaDto> GetAll(string orderNo, string payType, int? bandId, DateTime startTime, DateTime endTime, string telephone, int pageIndex, int pageSize)
        {
            CheckUser();
            using (var db = new YintaiHZhouContext())
            {
                var query = db.OPC_SaleRMA.Where(t =>true);
                var query2 = db.Orders.Where(t => t.CreateDate >= startTime && t.CreateDate < endTime && CurrentUser.StoreIDs.Contains(t.StoreId));
                if (!string.IsNullOrWhiteSpace(orderNo))
                {
                    query = query.Where(t => t.OrderNo.Contains(orderNo));
                    query2 = query2.Where(t => t.OrderNo.Contains(orderNo));
                }


                if (!string.IsNullOrWhiteSpace(payType))
                {
                    query2 = query2.Where(t => t.PaymentMethodCode==payType);
                }

                if (bandId.HasValue)
                {
                    query2 = query2.Where(t => t.BrandId==bandId.Value);
                }
               
                var q = from t in query2
                    join o in query on t.OrderNo equals o.OrderNo into cs
                    select new {SaleRMA = cs.FirstOrDefault(), Orders = t};
                q = q.OrderByDescending(t => t.Orders.OrderNo);
                var lst = q.ToPageResult(pageIndex, pageSize);
                var lstSaleRma = new List<SaleRmaDto>();
                foreach (var t in lst.Result)
                {
                    var o = new SaleRmaDto();
                    o.Id = t.Orders.Id;
                   
                    o.CustomerAddress = t.Orders.ShippingAddress;
                    o.CustomerName = t.Orders.ShippingContactPerson;
                    o.CustomerPhone = t.Orders.ShippingContactPhone;
                    
                    o.IfReceipt = t.Orders.NeedInvoice.HasValue?t.Orders.NeedInvoice.Value:false;
                    o.MustPayTotal =(double)( t.Orders.TotalAmount);
                    o.OrderNo = t.Orders.OrderNo;
                    o.PaymentMethodName = t.Orders.PaymentMethodName;
                    
                    o.ReceiptContent = t.Orders.InvoiceDetail;
                    o.ReceiptHead = t.Orders.InvoiceSubject;
                   
                    o.OrderSource = t.Orders.OrderSource;
                    o.OrderTransFee = t.Orders.ShippingFee;
                   
                    if (t.SaleRMA!=null)
                    {
                        o.BuyDate = t.SaleRMA.CreatedDate;
                        o.CustomFee = t.SaleRMA.CustomFee;
                        o.RealRMASumMoney = t.SaleRMA.RealRMASumMoney;
                        o.RecoverableSumMoney = t.SaleRMA.RecoverableSumMoney;
                        o.RealRMASumMoney = t.SaleRMA.RealRMASumMoney;
                        o.SaleOrderNo = t.SaleRMA.SaleOrderNo;
                        o.StoreFee = t.SaleRMA.StoreFee;
                        o.ServiceAgreeDate = t.SaleRMA.ServiceAgreeTime;
                        o.CustomerRemark = t.SaleRMA.Reason;
                    }
                    lstSaleRma.Add(o);
                }
                return new PageResult<SaleRmaDto>(lstSaleRma,lst.TotalCount);
            }
        }

        public PageResult<SaleRmaDto> GetAll(string orderNo, string saleOrderNo, string payType, string rmaNo, DateTime startTime, DateTime endTime,
            int? rmaStatus, int? storeId, string returnGoodsStatus,int pageIndex,int pageSize)
        {
            CheckUser();
            using (var db = new YintaiHZhouContext())
            {
                var query = db.OPC_SaleRMA.Where(t => t.CreatedDate >= startTime && t.CreatedDate < endTime && t.RMAStatus==returnGoodsStatus);
                var query2 = db.Orders.Where(t => t.CreateDate >= startTime && t.CreateDate < endTime);
                if (!string.IsNullOrWhiteSpace(orderNo))
                {
                    query = query.Where(t => t.OrderNo.Contains(orderNo));
                    query2 = query2.Where(t => t.OrderNo.Contains(orderNo));
                }

                if (!string.IsNullOrWhiteSpace(saleOrderNo))
                {
                    query = query.Where(t => t.SaleOrderNo.Contains(saleOrderNo));
                }

                if (!string.IsNullOrWhiteSpace(rmaNo))
                {
                    query = query.Where(t => t.RMANo.Contains(rmaNo));
                }
               

                if (!string.IsNullOrWhiteSpace(payType))
                {
                    query2 = query2.Where(t => t.PaymentMethodCode == payType);
                }
                if (rmaStatus.HasValue)
                {
                    query2 = query2.Where(t => t.Status == rmaStatus.Value);
                }

                if (storeId.HasValue)
                {
                    query2 = query2.Where(t => t.StoreId == storeId.Value);
                }

               // var lst = query.Join(query2, t => t.OrderNo, o => o.OrderNo, (t, o) => new { SaleRMA = t, Orders = o }).ToList();
                var q = from t in query2
                        join o in query on t.OrderNo equals o.OrderNo into cs
                        select new { SaleRMA = cs.FirstOrDefault(), Orders = t };
                q = q.OrderBy(t=>t.Orders.OrderNo);
                var lst = q.ToPageResult(pageIndex,pageSize);


                var lstSaleRma = new List<SaleRmaDto>();
                foreach (var t in lst.Result)
                {
                    var o = new SaleRmaDto();
                    o.Id = t.Orders.Id;

                    o.CustomerAddress = t.Orders.ShippingAddress;
                    o.CustomerName = t.Orders.ShippingContactPerson;
                    o.CustomerPhone = t.Orders.ShippingContactPhone;

                    o.IfReceipt = t.Orders.NeedInvoice.HasValue ? t.Orders.NeedInvoice.Value : false;
                    o.MustPayTotal = (double)(t.Orders.TotalAmount);
                    o.OrderNo = t.Orders.OrderNo;
                    o.PaymentMethodName = t.Orders.PaymentMethodName;

                    o.ReceiptContent = t.Orders.InvoiceDetail;
                    o.ReceiptHead = t.Orders.InvoiceSubject;

                    o.OrderSource = t.Orders.OrderSource;
                    o.OrderTransFee = t.Orders.ShippingFee;

                    if (t.SaleRMA != null)
                    {
                        o.BuyDate = t.SaleRMA.CreatedDate;
                        o.CustomFee = t.SaleRMA.CustomFee;
                        o.RealRMASumMoney = t.SaleRMA.RealRMASumMoney;
                        o.RecoverableSumMoney = t.SaleRMA.RecoverableSumMoney;
                        o.RealRMASumMoney = t.SaleRMA.RealRMASumMoney;
                        o.SaleOrderNo = t.SaleRMA.SaleOrderNo;
                        o.StoreFee = t.SaleRMA.StoreFee;
                        o.ServiceAgreeDate = t.SaleRMA.ServiceAgreeTime;
                        o.CustomerRemark = t.SaleRMA.Reason;
                    }

                    lstSaleRma.Add(o);
                }
                return new PageResult<SaleRmaDto>(lstSaleRma,lst.TotalCount);
            }
        }

        public OPC_SaleRMA GetByRmaNo(string rmaNo)
        {
            return  Select(t => t.RMANo == rmaNo).FirstOrDefault();
        }
    }
}