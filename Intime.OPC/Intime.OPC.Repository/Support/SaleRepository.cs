// ***********************************************************************
// Assembly         : 01_Intime.OPC.Repository
// Author           : Liuyh
// Created          : 03-19-2014 20:11:42
//
// Last Modified By : Liuyh
// Last Modified On : 03-24-2014 01:29:37
// ***********************************************************************
// <copyright file="SaleRepository.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using AutoMapper;
using Intime.OPC.Domain;
using Intime.OPC.Domain.Dto;
using Intime.OPC.Domain.Enums;
using Intime.OPC.Domain.Models;
using Intime.OPC.Repository.Base;

namespace Intime.OPC.Repository.Support
{
    /// <summary>
    ///     Class SaleRepository.
    /// </summary>
    public class SaleRepository : BaseRepository<OPC_Sale>, ISaleRepository
    {
        #region ISaleRepository Members

        /// <summary>
        ///     Selects this instance.
        /// </summary>
        /// <returns>IList{OPC_Sale}.</returns>
        public IList<OPC_Sale> Select()
        {
            using (var db = new YintaiHZhouContext())
            {
                if (CurrentUser != null)
                {
                  //query = query.Where(t => t.SectionId.HasValue && sectionIds.Contains(t.SectionId.Value));
                  return   db.OPC_Sales.Where(t => t.SectionId.HasValue && CurrentUser.SectionID.Contains(t.SectionId.Value)).ToList();
                }
                List<OPC_Sale> saleList = db.OPC_Sales.ToList();
                return saleList;
            }
        }

        public bool UpdateSatus(string saleNo, EnumSaleOrderStatus saleOrderStatus, int userID)
        {
            using (var db = new YintaiHZhouContext())
            {
                OPC_Sale sale = db.OPC_Sales.FirstOrDefault(t => t.SaleOrderNo == saleNo);
                if (sale != null)
                {
                    sale.UpdatedDate = DateTime.Now;
                    sale.UpdatedUser = userID;
                    sale.Status = (int) saleOrderStatus;
                }

                db.SaveChanges();
                return true;
            }
        }

        /// <summary>
        ///     Gets the by sale no.
        /// </summary>
        /// <param name="saleNo">The sale no.</param>
        /// <returns>OPC_Sale.</returns>
        public OPC_Sale GetBySaleNo(string saleNo)
        {
            using (var db = new YintaiHZhouContext())
            {
                return db.OPC_Sales.FirstOrDefault(t => t.SaleOrderNo == saleNo);
            }
        }

        /// <summary>
        ///     Gets the sale order details.
        /// </summary>
        /// <param name="saleOrderNo">The sale order no.</param>
        /// <returns>IList{OPC_SaleDetail}.</returns>
        public PageResult<SaleDetailDto> GetSaleOrderDetails(string saleOrderNo, int pageIndex, int pageSize)
        {
            //return Select2<OPC_SaleDetail, DateTime>(t => t.SaleOrderNo == saleOrderNo, t => t.CreatedDate, false,
            //    pageIndex, pageSize);
            using (var db = new YintaiHZhouContext())
            {
                var query =
                    db.OPC_SaleDetails.Where(t => t.SaleOrderNo == saleOrderNo);
                  
                        //.Join(db.OrderItems, t => t.OrderItemId, o => o.Id, (t, o) => new {Sale = t, OrderItem = o})
                        //.Join(db.Brands, t => t.OrderItem.BrandId, o => o.Id,
                        //    (t, o) => new {t.Sale, t.OrderItem, BrandName = o.Name});
                var qq = from q in db.OrderItems
                    join b in db.Brands on q.BrandId equals b.Id into bb
                    join o in db.Orders on q.OrderNo equals o.OrderNo into oo

                    select new {OrderItems=q,Brand=bb.FirstOrDefault(),Order=oo.FirstOrDefault()};



                var filter = from q in query
                    join o in qq on q.OrderItemId equals o.OrderItems.Id into oo
                    join p in db.OPC_Stocks on q.StockId equals p.Id into pp
                    
                    select new {OrderItem=oo.FirstOrDefault(),Sale=q,Stock=pp.FirstOrDefault()};


                var lst3 = filter.OrderByDescending(t => t.Sale.CreatedDate);
                var lst = lst3.ToPageResult(pageIndex, pageSize);
                var lstDto = new List<SaleDetailDto>();
                foreach (var t in lst.Result)
                {
                    SaleDetailDto o = Mapper.Map<OPC_SaleDetail, SaleDetailDto>(t.Sale);
                    if (t.OrderItem!=null)
                    {
                        
                        o.Color = t.OrderItem.OrderItems.ColorValueName;
                        o.Size = t.OrderItem.OrderItems.SizeValueName;
                        o.ProductNo = t.OrderItem.OrderItems.StoreSalesCode;
                        o.ProductName = t.OrderItem.OrderItems.ProductName;
                        if (t.OrderItem.Brand!=null)
                        {
                            o.Brand = t.OrderItem.Brand.Name;
                        }
                    }
                    //o.LabelPrice = (double)t.OrderItem.OrderItems.UnitPrice;
                    //o.Price = (double)t.OrderItem.OrderItems.ItemPrice;
                    //o.SalePrice = (double)t.OrderItem.OrderItems.ItemPrice;
                    //o.SellPrice = (double)t.OrderItem.OrderItems.ItemPrice;
                    o.SectionCode = t.Sale.SectionCode;
                    //o.StyleNo=t.Stock.ProductCode
                    o.StyleNo = t.OrderItem.OrderItems.StoreItemNo;
                    //if (t.Stock!=null)
                    //{
                    //    o.StyleNo = t.Stock.ProductCode;
                    //}
                    lstDto.Add(o);
                }
                return new PageResult<SaleDetailDto>(lstDto, lst.TotalCount);
            }
        }

        public PageResult<SaleDto> GetPickUped(string saleId, string orderNo, DateTime dtStart, DateTime dtEnd,
            int pageIndex, int pageSize, params int[] sectionIds)
        {
            return getSalesData(saleId, orderNo, dtStart, dtEnd, EnumSaleOrderStatus.PickUp, pageIndex, pageSize,
                sectionIds);
        }

        /// <summary>
        ///     获得已完成 打印销售单 的数据
        /// </summary>
        /// <param name="saleId">The sale identifier.</param>
        /// <param name="orderNo">The order no.</param>
        /// <param name="dtStart">The dt start.</param>
        /// <param name="dtEnd">The dt end.</param>
        /// <returns>IList{OPC_Sale}.</returns>
        public PageResult<SaleDto> GetPrintSale(string saleId, string orderNo, DateTime dtStart, DateTime dtEnd,
            int pageIndex, int pageSize, params int[] sectionIds)
        {
            //return getSalesData(saleId, orderNo, dtStart, dtEnd, EnumSaleOrderStatus.PrintSale, pageIndex, pageSize,
            //    sectionIds);
            var saleOrderStatus = EnumSaleOrderStatus.PrintSale.AsID();
            var saleOrderStatus1 = EnumSaleOrderStatus.ShoppingGuidePickUp.AsID();
            using (var db = new YintaiHZhouContext())
            {
                IQueryable<OPC_Sale> query = db.OPC_Sales.Where(t => (t.Status == saleOrderStatus || t.Status == saleOrderStatus1)
                                                                    && t.SellDate >= dtStart
                                                                    && t.SellDate < dtEnd);

                if (sectionIds != null)
                {
                    query = query.Where(t => sectionIds.Contains(t.SectionId.Value));
                }

                if (!string.IsNullOrWhiteSpace(orderNo))
                {
                    query = query.Where(t => t.OrderNo.Contains(orderNo));
                }

                if (!string.IsNullOrWhiteSpace(saleId))
                {
                    query = query.Where(t => t.SaleOrderNo.Contains(saleId));
                }


                var ll = from s in db.Sections
                         join store in db.Stores on s.StoreId equals store.Id into ss
                         select new { SectionID = s.Id, Section = s, Store = ss.FirstOrDefault() };

                var uu = from o in db.Orders
                         join u in db.Users on o.CustomerId equals u.Id into ss
                         select new { OrderNo = o.OrderNo, Order = o, User = ss.FirstOrDefault() };
                

                var ll2 = from s in query
                          join l in ll on s.SectionId equals l.SectionID into ss
                          join o in uu on s.OrderNo equals o.OrderNo into oo
                    join r in db.OrderTransactions on s.OrderNo equals r.OrderNo into rr
                
                
                          select new { Sale = s, Order = oo.FirstOrDefault(), Store = ss.FirstOrDefault(),OrderTrans=rr.FirstOrDefault() };



                ll2 = ll2.OrderByDescending(t => t.Sale.CreatedDate);
                var lst = ll2.ToPageResult(pageIndex, pageSize);
                var lstDto = new List<SaleDto>();
                foreach (var t in lst.Result)
                {
                    var o = AutoMapper.Mapper.Map<OPC_Sale, SaleDto>(t.Sale);
                    if (t.Store != null && t.Store.Store != null)
                    {
                        o.StoreName = t.Store.Store.Name;
                        o.StoreTelephone = t.Store.Store.Tel;
                        o.StoreAddress = t.Store.Store.Location;
                    }


                    if (t.Store != null && t.Store.Section != null)
                    {
                        o.SectionName = t.Store.Section.Name;
                    }
                    if (t.OrderTrans!=null)
                    {
                        o.TransNo = t.OrderTrans.TransNo;
                    }
                    o.ReceivePerson = t.Order.User.Nickname;
                    o.OrderSource = t.Order.Order.OrderSource;
                    o.InvoiceSubject = t.Order.Order.InvoiceSubject;
                    o.PayType = t.Order.Order.PaymentMethodName;
                    o.Invoice = t.Order.Order.InvoiceDetail;
                    lstDto.Add(o);
                }
                return new PageResult<SaleDto>(lstDto, lst.TotalCount);
            }
        }

        /// <summary>
        ///     获得 未提货 的数据
        /// </summary>
        /// <param name="saleId">The sale identifier.</param>
        /// <param name="orderNo">The order no.</param>
        /// <param name="dtStart">The dt start.</param>
        /// <param name="dtEnd">The dt end.</param>
        /// <returns>IList{OPC_Sale}.</returns>
        public PageResult<SaleDto> GetNoPickUp(string saleId, string orderNo, DateTime dtStart, DateTime dtEnd,
            int pageIndex, int pageSize, params int[] sectionIds)
        {

            
            //return getSalesData(saleId, orderNo, dtStart, dtEnd, EnumSaleOrderStatus.NoPickUp, pageIndex, pageSize,
            //    sectionIds);
            int saleOrderStatus = EnumSaleOrderStatus.NotifyProduct.AsID();
            int cashStatus = EnumSaleOrderCashStatus.CashOver.AsID();
            using (var db = new YintaiHZhouContext())
            {
                IQueryable<OPC_Sale> query = db.OPC_Sales.Where(t => (t.Status == saleOrderStatus || t.Status == (int)EnumSaleOrderStatus.NoPickUp || t.Status == (int)EnumSaleOrderStatus.Fetched)
                                                                    && t.SellDate >= dtStart
                                                                    && t.SellDate < dtEnd);

                if (sectionIds != null)
                {
                    query = query.Where(t => t.SectionId.HasValue && sectionIds.Contains(t.SectionId.Value));
                }

                if (!string.IsNullOrWhiteSpace(orderNo))
                {
                    query = query.Where(t => t.OrderNo.Contains(orderNo));
                }

                if (!string.IsNullOrWhiteSpace(saleId))
                {
                    query = query.Where(t => t.SaleOrderNo.Contains(saleId));
                }

                var qq = from sale in query
                         join s in db.Sections on sale.SectionId equals s.Id into cs
                         join r in db.OrderTransactions on sale.OrderNo equals r.OrderNo into rr
                         select new { Sale = sale, Section = cs.FirstOrDefault(),OrderTrans=rr.FirstOrDefault() };

                var uu = from o in db.Orders
                         join u in db.Users on o.CustomerId equals u.Id into ss
                         select new { OrderNo = o.OrderNo, Order = o, User = ss.FirstOrDefault() };
                

                var filter = from q in qq
                             join s in db.Stores on q.Section.StoreId equals s.Id into mm
                    join o in uu on q.Sale.OrderNo equals o.OrderNo into oo
                             
                             select new { Sale = q.Sale,Section=q.Section, OrderTrans=q.OrderTrans, Order=oo.FirstOrDefault(), Store = mm.FirstOrDefault() };


                filter = filter.OrderByDescending(t => t.Sale.CreatedDate);
                var lst=filter.ToPageResult(pageIndex, pageSize);
                var lstDto = new List<SaleDto>();
                foreach (var s in lst.Result)
                {
                    var o = Mapper.Map<OPC_Sale, SaleDto>(s.Sale);
                    if (s.Store!=null)
                    {
                        o.StoreName = s.Store.Name;
                        o.StoreTelephone = s.Store.Tel;
                        o.StoreAddress = s.Store.Location;
                    }
                    if (s.Section != null)
                    {
                        o.SectionName = s.Section.Name;
                    }
                    if (s.OrderTrans!=null)
                    {
                        o.TransNo = s.OrderTrans.TransNo;
                    }
                    o.ReceivePerson = s.Order.User.Nickname;
                    o.OrderSource = s.Order.Order.OrderSource;
                    o.InvoiceSubject = s.Order.Order.InvoiceSubject;
                    o.PayType = s.Order.Order.PaymentMethodName;
                    o.Invoice = s.Order.Order.InvoiceDetail;
                    lstDto.Add(o);
                }
                return new PageResult<SaleDto>(lstDto,lst.TotalCount);
                 
            }
        }

        /// <summary>
        ///     Updates the satus.
        /// </summary>
        /// <param name="saleNos">The sale nos.</param>
        /// <param name="saleOrderStatus">The sale order status.</param>
        /// <param name="userID">The user identifier.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool UpdateSatus(IEnumerable<string> saleNos, EnumSaleOrderStatus saleOrderStatus, int userID)
        {
            //todo 更新状态为实现
            throw new Exception("UpdateSatus 未实现");
            using (var db = new YintaiHZhouContext())
            {
                foreach (string saleNo in saleNos)
                {
                    OPC_Sale sale = db.OPC_Sales.FirstOrDefault(t => t.SaleOrderNo == saleNo);
                    if (sale != null)
                    {
                        sale.UpdatedDate = DateTime.Now;
                        sale.UpdatedUser = userID;
                        sale.Status = (int) saleOrderStatus;
                    }
                }
                IQueryable<OPC_Sale> lst = db.OPC_Sales.Where(t => saleNos.Contains(t.SaleOrderNo));

                IEnumerable<OPC_Sale> lst2 = saleNos.Join(db.OPC_Sales, t => t, o => o.SaleOrderNo, (t, o) => o);

                db.SaveChanges();
                return true;
            }
        }

        /// <summary>
        ///     Gets the print express.
        /// </summary>
        /// <param name="saleOrderNo">The sale order no.</param>
        /// <param name="orderNo">The order no.</param>
        /// <param name="dtStart">The dt start.</param>
        /// <param name="dtEnd">The dt end.</param>
        /// <returns>IList{OPC_Sale}.</returns>
        public PageResult<SaleDto> GetPrintExpress(string saleOrderNo, string orderNo, DateTime dtStart, DateTime dtEnd,
            int pageIndex, int pageSize, params int[] sectionIds)
        {
            return getSalesData(saleOrderNo, orderNo, dtStart, dtEnd, EnumSaleOrderStatus.PrintExpress, pageIndex,
                pageSize, sectionIds);
        }

        /// <summary>
        ///     Gets the print invoice.
        /// </summary>
        /// <param name="saleOrderNo">The sale order no.</param>
        /// <param name="orderNo">The order no.</param>
        /// <param name="dtStart">The dt start.</param>
        /// <param name="dtEnd">The dt end.</param>
        /// <returns>IList{OPC_Sale}.</returns>
        public PageResult<SaleDto> GetPrintInvoice(string saleOrderNo, string orderNo, DateTime dtStart, DateTime dtEnd,
            int pageIndex, int pageSize, params int[] sectionIds)
        {
            return getSalesData(saleOrderNo, orderNo, dtStart, dtEnd, EnumSaleOrderStatus.PrintExpress, pageIndex,
                pageSize, sectionIds);
        }

        public PageResult<SaleDto> GetShipInStorage(string saleOrderNo, string orderNo, DateTime dtStart,
            DateTime dtEnd, int pageIndex, int pageSize, params int[] sectionIds)
        {
            return getSalesData(saleOrderNo, orderNo, dtStart, dtEnd, EnumSaleOrderStatus.ShipInStorage, pageIndex,
                pageSize, sectionIds);
        }

        public IList<OPC_Sale> GetByOrderNo(string orderID, int sectinID)
        {
            if (sectinID > -1)
            {
                return Select(t => t.OrderNo == orderID && t.SectionId == sectinID);
            }
            return Select(t => t.OrderNo == orderID);
        }

        public IList<SaleDto> GetByOrderNo2(string orderID)
        {
            using (var db = new YintaiHZhouContext())
            {
                var query = db.OPC_Sales.Where(t => t.OrderNo == orderID);
                if (CurrentUser!=null)
                {
                    query = query.Where(t => t.SectionId.HasValue && CurrentUser.SectionID.Contains(t.SectionId.Value));
                }

                var qq = from sale in query
                         join s in db.Sections on sale.SectionId equals s.Id into cs
                         join r in db.OrderTransactions on sale.OrderNo equals r.OrderNo into rr
                         select new { Sale = sale, Section = cs.FirstOrDefault(), OrderTrans = rr.FirstOrDefault() };

                //var qq = from sale in query
                //    join s in db.Sections on sale.SectionId equals s.Id into cs
                //    select new {Sale = sale, Section = cs.FirstOrDefault()};

                var uu = from o in db.Orders
                         join u in db.Users on o.CustomerId equals u.Id into ss
                         select new { OrderNo = o.OrderNo, Order = o, User = ss.FirstOrDefault() };
                

                var filter = from q in qq
                    join s in db.Stores on q.Section.StoreId equals s.Id into mm
                      join o in uu on q.Sale.OrderNo equals o.OrderNo into oo
                             select new { Sale = q.Sale, Section = q.Section, OrderTrans = q.OrderTrans, User = oo.FirstOrDefault().User, Order = oo.FirstOrDefault().Order, Store = mm.FirstOrDefault() };
                             //select new { Sale = q.Sale,Section=q.Section, Order=oo.FirstOrDefault().Order, User=oo.FirstOrDefault().User, Store = mm.FirstOrDefault() };

     

                var lst = filter.ToList();
                var lstDto = new List<SaleDto>();
                foreach (var s in lst)
                {
                    var o=Mapper.Map<OPC_Sale, SaleDto>(s.Sale);
                    if (s.Store != null)
                    {
                        o.StoreName = s.Store.Name;
                        o.StoreName = s.Store.Name;
                        o.StoreTelephone = s.Store.Tel;
                        o.StoreAddress = s.Store.Location;
                    }
                    if (s.Section != null)
                    {
                        o.SectionName = s.Section.Name;
                    }

                    if (s.OrderTrans != null)
                    {
                        o.TransNo = s.OrderTrans.TransNo;
                    }

                    o.ReceivePerson = s.User.Nickname;
                    o.OrderSource = s.Order.OrderSource;
                    o.InvoiceSubject = s.Order.InvoiceSubject;
                    o.PayType = s.Order.PaymentMethodName;
                    o.Invoice = s.Order.InvoiceDetail;
                    lstDto.Add(o);
                }
                return lstDto;
            }
        }

        public PageResult<SaleDto> GetShipped(string saleOrderNo, string orderNo, DateTime dtStart, DateTime dtEnd,
            int pageIndex, int pageSize, params int[] sectionIds)
        {
            return getSalesData(saleOrderNo, orderNo, dtStart, dtEnd, EnumSaleOrderStatus.Shipped, pageIndex, pageSize,
                sectionIds);
        }

        public IList<OPC_Sale> GetByShippingCode(string shippingCode)
        {
            var va = (int) (EnumSaleOrderStatus.Void);
            return Select(t => t.ShippingCode == shippingCode && t.Status > va);
        }

        #endregion

        /// <summary>
        ///     Gets the data.
        /// </summary>
        /// <param name="saleId">The sale identifier.</param>
        /// <param name="orderNo">The order no.</param>
        /// <param name="dtStart">The dt start.</param>
        /// <param name="dtEnd">The dt end.</param>
        /// <param name="saleOrderStatus">The sale order status.</param>
        /// <returns>IList{OPC_Sale}.</returns>
        private PageResult<SaleDto> getSalesData(string saleId, string orderNo, DateTime dtStart, DateTime dtEnd,
            EnumSaleOrderStatus saleOrderStatus, int pageIndex, int pageSize, params int[] sectionIds)
        {
            using (var db = new YintaiHZhouContext())
            {
                IQueryable<OPC_Sale> query = db.OPC_Sales.Where(t => t.Status == (int) saleOrderStatus
                                                                    && t.SellDate >= dtStart
                                                                    && t.SellDate < dtEnd);
                //if (CurrentUser!=null)
                //{
                //    query = query.Where(t => t.SectionId.HasValue && CurrentUser.SectionID.Contains(t.SectionId.Value));
                //}

                if (sectionIds != null)
                {
                    query = query.Where(t => sectionIds.Contains(t.SectionId.Value));
                }

                if (!string.IsNullOrWhiteSpace(orderNo))
                {
                    query = query.Where(t => t.OrderNo.Contains(orderNo));
                }

                if (!string.IsNullOrWhiteSpace(saleId))
                {
                    query = query.Where(t => t.SaleOrderNo.Contains(saleId));
                }
               
                
                var ll = from s in db.Sections
                    join store in db.Stores on s.StoreId equals store.Id into ss
                    select new {SectionID=s.Id, Section = s, Store = ss.FirstOrDefault()};

                var uu = from o in db.Orders
                    join u in db.Users on o.CustomerId equals u.Id into ss
                    select new { OrderNo=o.OrderNo, Order=o,User=ss.FirstOrDefault()};
                
                

                var ll2 = from s in query
                    join l in ll on s.SectionId equals l.SectionID into ss
                    join o in uu on s.OrderNo equals o.OrderNo into oo
                    join r in db.OrderTransactions on s.OrderNo equals r.OrderNo into rr

                    select new {Sale = s, Order = oo.FirstOrDefault().Order, User=oo.FirstOrDefault().User, Store = ss.FirstOrDefault(),OrderTrans=rr.FirstOrDefault()};



                ll2 = ll2.OrderByDescending(t => t.Sale.CreatedDate);
                var lst = ll2.ToPageResult(pageIndex, pageSize);
                var lstDto = new List<SaleDto>();
                foreach (var t in lst.Result)
                {
                    var o = AutoMapper.Mapper.Map<OPC_Sale, SaleDto>(t.Sale);
                    if (t.Store!=null && t.Store.Store!=null)
                    {
                        o.StoreName = t.Store.Store.Name;
                        o.StoreTelephone = t.Store.Store.Tel;
                        o.StoreAddress = t.Store.Store.Location;

                    }

                    if (t.OrderTrans != null)
                    {
                        o.TransNo = t.OrderTrans.TransNo;
                    }

                    if (t.Store != null && t.Store.Section!=null)
                    {
                        o.SectionName = t.Store.Section.Name;
                    }
                    o.ReceivePerson = t.User.Nickname;
                    o.OrderSource = t.Order.OrderSource;
                    o.InvoiceSubject = t.Order.InvoiceSubject;
                    o.PayType = t.Order.PaymentMethodName;
                    o.Invoice = t.Order.InvoiceDetail;
                    
                    lstDto.Add(o);
                }
                return new PageResult<SaleDto>(lstDto,lst.TotalCount);
            }
        }
    }
}