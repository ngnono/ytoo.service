using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Intime.OPC.Domain.Dto;
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

        public IList<SaleRmaDto> GetAll(string orderNo, string payType, int? bandId, DateTime startTime, DateTime endTime, string telephone)
        {

            using (var db = new YintaiHZhouContext())
            {
                var query = db.OPC_SaleRMA.Where(t => t.CreatedDate >= startTime && t.CreatedDate < endTime);
                var query2 = db.Orders.Where(t=>1==1);
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

                var lst = query.Join(query2, t => t.OrderNo, o => o.OrderNo, (t, o) => new { SaleRMA = t, Orders=o }).ToList();
                var lstSaleRma = new List<SaleRmaDto>();
                foreach (var t in lst)
                {
                    var o = new SaleRmaDto();
                    o.Id = t.SaleRMA.Id;
                    o.BuyDate = t.SaleRMA.CreatedDate;
                    o.CustomFee = t.SaleRMA.CustomFee;
                    o.CustomerAddress = t.Orders.ShippingAddress;
                    o.CustomerName = t.Orders.ShippingContactPerson;
                    o.CustomerPhone = t.Orders.ShippingContactPhone;
                    o.CustomerRemark = t.SaleRMA.Reason;
                    o.IfReceipt = t.Orders.NeedInvoice.HasValue?t.Orders.NeedInvoice.Value:false;
                    o.MustPayTotal =(double)( t.Orders.TotalAmount);
                    o.OrderNo = t.SaleRMA.OrderNo;
                    o.PaymentMethodName = t.Orders.PaymentMethodName;
                    o.RealRMASumMoney = t.SaleRMA.RealRMASumMoney;
                    o.ReceiptContent = t.Orders.InvoiceDetail;
                    o.ReceiptHead = t.Orders.InvoiceSubject;
                    o.SaleOrderNo = t.SaleRMA.SaleOrderNo;
                    o.StoreFee = t.SaleRMA.StoreFee;
                    o.ServiceAgreeDate = t.SaleRMA.ServiceAgreeTime;
                  
                    lstSaleRma.Add(o);
                }
                return lstSaleRma;
            }
        }

        public IList<SaleRmaDto> GetAll(string orderNo, string saleOrderNo, string payType, string rmaNo, DateTime startTime, DateTime endTime,
            int? rmaStatus, int? storeId, string returnGoodsStatus)
        {
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

                var lst = query.Join(query2, t => t.OrderNo, o => o.OrderNo, (t, o) => new { SaleRMA = t, Orders = o }).ToList();
                var lstSaleRma = new List<SaleRmaDto>();
                foreach (var t in lst)
                {
                    var o = new SaleRmaDto();
                    o.Id = t.SaleRMA.Id;
                    o.BuyDate = t.SaleRMA.CreatedDate;
                    o.CustomFee = t.SaleRMA.CustomFee;
                    o.CustomerAddress = t.Orders.ShippingAddress;
                    o.CustomerName = t.Orders.ShippingContactPerson;
                    o.CustomerPhone = t.Orders.ShippingContactPhone;
                    o.CustomerRemark = t.SaleRMA.Reason;
                    o.IfReceipt = t.Orders.NeedInvoice.HasValue ? t.Orders.NeedInvoice.Value : false;
                    o.MustPayTotal = (double)(t.Orders.TotalAmount);
                    o.OrderNo = t.SaleRMA.OrderNo;
                    o.PaymentMethodName = t.Orders.PaymentMethodName;
                    o.RealRMASumMoney = t.SaleRMA.RealRMASumMoney;
                    o.ReceiptContent = t.Orders.InvoiceDetail;
                    o.ReceiptHead = t.Orders.InvoiceSubject;
                    o.SaleOrderNo = t.SaleRMA.SaleOrderNo;
                    o.StoreFee = t.SaleRMA.StoreFee;

                    lstSaleRma.Add(o);
                }
                return lstSaleRma;
            }
        }

        public OPC_SaleRMA GetByRmaNo(string rmaNo)
        {
            return  Select(t => t.RMANo == rmaNo).FirstOrDefault();
        }
    }
}