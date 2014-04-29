using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using AutoMapper;
using Intime.OPC.Domain;
using Intime.OPC.Domain.Dto;
using Intime.OPC.Domain.Dto.Financial;
using Intime.OPC.Domain.Enums;
using Intime.OPC.Domain.Models;
using Intime.OPC.Repository.Base;

namespace Intime.OPC.Repository.Support
{
    public class OrderItemRepository : BaseRepository<OrderItem>, IOrderItemRepository
    {
        #region IOrderItemRepository Members

        public PageResult<OrderItemDto> GetByOrderNo(string orderNo, int pageIndex, int pageSize)
        {
            using (var db = new YintaiHZhouContext())
            {
                IQueryable<OrderItem> query = db.OrderItems.Where(t => t.OrderNo == orderNo);

                IQueryable<OPC_RMADetail> query2 = db.OrderItems.Where(t => t.OrderNo == orderNo)
                    .Join(db.OPC_RMADetail, t => t.Id, o => o.OrderItemId, (t, o) => o);
                var filter = from q in query
                    join b in db.Brands on q.BrandId equals b.Id into cs
                    join rmaDetail in query2 on q.Id equals rmaDetail.OrderItemId into rma
                    select new {Order = q, Brand = cs.FirstOrDefault(), Rma = rma.FirstOrDefault()};
                filter = filter.OrderByDescending(t => t.Order.CreateDate);
                var list = filter.ToPageResult(pageIndex, pageSize);
                IList<OrderItemDto> lstDtos = new List<OrderItemDto>();
                foreach (var t in list.Result)
                {
                    OrderItemDto o = Mapper.Map<OrderItem, OrderItemDto>(t.Order);
                    o.BrandName = t.Brand == null ? "" : t.Brand.Name;
                    if (t.Rma != null)
                    {
                        o.NeedReturnCount = t.Rma.BackCount;
                        o.ReturnCount = t.Rma.BackCount;
                    }
                    lstDtos.Add(o);
                }
                return new PageResult<OrderItemDto>(lstDtos, list.TotalCount);
            }
        }

        public IList<OrderItem> GetByIDs(IEnumerable<int> ids)
        {
            return Select(t => ids.Contains(t.Id));
        }

        public SaleDetailStatListDto WebSiteStatSaleDetail(SearchStatRequest request)
        {
            using (var db = new YintaiHZhouContext())
            {
                var lstSections = CurrentUser.SectionID;
                var saleNos = db.OPC_Sale.Where(t => t.SectionId.HasValue && lstSections.Contains(t.SectionId.Value)).Select<OPC_Sale, string>(t => t.SaleOrderNo).Distinct().ToList();


                IQueryable<OPC_SaleDetail> query =
                    db.OPC_SaleDetail.Where(t => t.CreatedDate >= request.StartTime && t.CreatedDate < request.EndTime && saleNos.Contains(t.SaleOrderNo));
                IQueryable<OrderItem> queryOrder =
                    db.OrderItems.Where(t => t.CreateDate >= request.StartTime && t.CreateDate < request.EndTime);

                var query2 = query.Join(queryOrder, t => t.OrderItemId, o => o.Id,
                    (t, o) => new {OrderItem = o, SaleDetail = t});

                var filter = from q in query2
                    join o in db.Orders on q.OrderItem.OrderNo equals o.OrderNo into order
                    join b in db.Brands on q.OrderItem.BrandId equals b.Id into cs
                    join s in db.Stores on q.OrderItem.StoreId equals s.Id into store
                    select
                        new
                        {
                            Order = order.FirstOrDefault(),
                            q.OrderItem,
                            Brand = cs.FirstOrDefault(),
                            Stroe = store.FirstOrDefault(),
                            q.SaleDetail
                        };
                var lst = filter.ToList();

                var lstDto = new SaleDetailStatListDto();

                foreach (var o in lst)
                {
                    var dto = new SaleDetailStatDto();

                    dto.Brand = o.Brand == null ? "" : o.Brand.Name;
                    dto.Color = o.OrderItem.ColorValueName;
                    dto.LabelPrice = o.OrderItem.UnitPrice.HasValue ? o.OrderItem.UnitPrice.Value : 0;
                    dto.BuyDate = o.Order.CreateDate;
                    dto.OrderNo = o.Order.OrderNo;
                    dto.OrderSouce = o.Order.OrderSource;
                    dto.OrderTransFee = o.Order.ShippingFee;
                    dto.PaymentMethodName = o.Order.PaymentMethodName;
                    dto.SalePrice = o.OrderItem.ItemPrice;
                    dto.SaleTotalPrice = o.OrderItem.ExtendPrice;
                    dto.SectionCode = o.SaleDetail.SectionCode;
                    dto.SellCount = o.SaleDetail.SaleCount;
                    dto.Size = o.OrderItem.SizeValueName;
                    dto.StoreName = o.Stroe==null?"": o.Stroe.Name;
                    dto.StyleNo = o.OrderItem.StoreItemNo;

                    lstDto.Add(dto);
                }
                return lstDto;
            }
        }

        public ReturnGoodsStatListDto WebSiteStatReturnGoods(SearchStatRequest request)
        {
            using (var db = new YintaiHZhouContext())
            {
                var lstSections = CurrentUser.SectionID;
                var saleNos=   db.OPC_Sale.Where(t => t.SectionId.HasValue && lstSections.Contains(t.SectionId.Value)).Select<OPC_Sale,string>(t=>t.SaleOrderNo).Distinct().ToList();

                IQueryable<OPC_SaleDetail> query =
                    db.OPC_SaleDetail.Where(t => t.CreatedDate >= request.StartTime && t.CreatedDate < request.EndTime && saleNos.Contains(t.SaleOrderNo));

               
                var queryOrder =
                    db.OrderItems.Where(t => t.CreateDate >= request.StartTime && t.CreateDate < request.EndTime)
                        .Join(db.OPC_RMADetail, t => t.Id, o => o.OrderItemId,
                            (t, o) => new {OrderItem = t, RmaDetail = o});

                var query2 = query.Join(queryOrder, t => t.OrderItemId, o => o.OrderItem.Id,
                    (t, o) => new {o.OrderItem, SaleDetail = t, o.RmaDetail});

                var filter = from q in query2
                    join o in db.Orders on q.OrderItem.OrderNo equals o.OrderNo into order
                    join b in db.Brands on q.OrderItem.BrandId equals b.Id into cs
                    join s in db.Stores on q.OrderItem.StoreId equals s.Id into store
                    join r in db.OPC_SaleRMA on q.RmaDetail.RMANo equals r.RMANo into saleRma
                    select
                        new
                        {
                            SaleRma = saleRma.FirstOrDefault(),
                            q.RmaDetail,
                            Order = order.FirstOrDefault(),
                            q.OrderItem,
                            Brand = cs.FirstOrDefault(),
                            Stroe = store.FirstOrDefault(),
                            q.SaleDetail
                        };
                var lst = filter.ToList();

                var lstDto = new ReturnGoodsStatListDto();

                foreach (var o in lst)
                {
                    var dto = new ReturnGoodsStatDto();

                    dto.Brand = o.Brand == null ? "" : o.Brand.Name;
                    dto.Color = o.OrderItem.ColorValueName;
                    dto.LabelPrice = o.OrderItem.UnitPrice.HasValue ? o.OrderItem.UnitPrice.Value : 0;
                    dto.BuyDate = o.Order.CreateDate;
                    dto.OrderNo = o.Order.OrderNo;
                    dto.OrderSouce = o.Order.OrderSource;
                    dto.OrderTransFee = o.Order.ShippingFee;
                    dto.PaymentMethodName = o.Order.PaymentMethodName;
                    dto.SalePrice = o.OrderItem.ItemPrice;
                    dto.ApplyRmaDate = o.RmaDetail.CreatedDate;
                    dto.RMANo = o.RmaDetail.RMANo;
                    dto.ReturnGoodsCount = o.RmaDetail.BackCount;
                    dto.OrderTransFee = o.Order.ShippingFee;
                    dto.RmaDate = o.SaleRma.BackDate;
                    dto.SectionCode = o.SaleDetail.SectionCode;
                    dto.RmaPrice = o.RmaDetail.Price;
                    dto.Size = o.OrderItem.SizeValueName;
                    dto.StoreName = o.Stroe.Name;
                    dto.StyleNo = o.OrderItem.StoreItemNo;

                    lstDto.Add(dto);
                }
                return lstDto;
            }
        }

        public CashierList WebSiteCashier(SearchCashierRequest request)
        {
            using (var db = new YintaiHZhouContext())
            {
                int cahStatus = EnumCashStatus.CashOver.AsID();

                var lstSections = CurrentUser.SectionID;
 
                var querySale =
                    db.OPC_Sale.Where(
                        t =>
                            t.CreatedDate >= request.StartTime && t.CreatedDate < request.EndTime &&
                            t.CashStatus == cahStatus && t.SectionId.HasValue && lstSections.Contains(t.SectionId.Value))
                        .Join(db.OPC_SaleDetail, t => t.SaleOrderNo, o => o.SaleOrderNo,
                            (t, o) => new {Sale = t, SaleDetail = o});

                IQueryable<OrderItem> queryOrder =
                    db.OrderItems.Where(t => t.CreateDate >= request.StartTime && t.CreateDate < request.EndTime);

                var query2 = querySale.Join(queryOrder, t => t.SaleDetail.OrderItemId, o => o.Id,
                    (t, o) => new {OrderItem = o, t.SaleDetail, t.Sale});

                var filter = from q in query2
                    join o in db.Orders on q.OrderItem.OrderNo equals o.OrderNo into order
                    join b in db.Brands on q.OrderItem.BrandId equals b.Id into cs
                    join s in db.Stores on q.OrderItem.StoreId equals s.Id into store
                    join r in db.OPC_RMADetail on q.OrderItem.Id equals r.OrderItemId into rmaDetails
                    select
                        new
                        {
                            RmaDetails = rmaDetails,
                            q.Sale,
                            Order = order.FirstOrDefault(),
                            q.OrderItem,
                            Brand = cs.FirstOrDefault(),
                            Stroe = store.FirstOrDefault(),
                            q.SaleDetail
                        };
                var lst = filter.ToList();

                var lstDto = new CashierList();

                foreach (var o in lst)
                {
                    if (o.RmaDetails == null)
                    {
                        var dto = new WebSiteCashierSearchDto();

                        dto.Brand = o.Brand == null ? "" : o.Brand.Name;
                        dto.Color = o.OrderItem.ColorValueName;
                        dto.LabelPrice = o.OrderItem.UnitPrice.HasValue ? o.OrderItem.UnitPrice.Value : 0;
                        dto.BuyDate = o.Order.CreateDate;
                        dto.OrderNo = o.Order.OrderNo;
                        dto.OrderSouce = o.Order.OrderSource;
                        dto.CashNum = o.Sale.CashNum;
                        dto.PaymentMethodName = o.Order.PaymentMethodName;
                        dto.SalePrice = o.OrderItem.ItemPrice;
                        dto.Count = o.OrderItem.Quantity;
                        dto.SectionCode = o.SaleDetail.SectionCode;

                        dto.Size = o.OrderItem.SizeValueName;
                        dto.StoreName = o.Stroe.Name;
                        dto.StyleNo = o.OrderItem.StoreItemNo;

                        lstDto.Add(dto);
                    }
                    else
                    {
                        foreach (OPC_RMADetail rma in o.RmaDetails)
                        {
                            var dto = new WebSiteCashierSearchDto();

                            dto.Brand = o.Brand == null ? "" : o.Brand.Name;
                            dto.Color = o.OrderItem.ColorValueName;
                            dto.LabelPrice = o.OrderItem.UnitPrice.HasValue ? o.OrderItem.UnitPrice.Value : 0;
                            dto.BuyDate = o.Order.CreateDate;
                            dto.OrderNo = o.Order.OrderNo;
                            dto.OrderSouce = o.Order.OrderSource;
                            dto.CashNum = o.Sale.CashNum;
                            dto.PaymentMethodName = o.Order.PaymentMethodName;
                            dto.SalePrice = o.OrderItem.ItemPrice;
                            dto.Count = o.OrderItem.Quantity;
                            dto.SectionCode = o.SaleDetail.SectionCode;

                            dto.Size = o.OrderItem.SizeValueName;
                            dto.StoreName = o.Stroe.Name;
                            dto.StyleNo = o.OrderItem.StoreItemNo;
                            dto.RmaCashNum = rma.RMANo;

                            lstDto.Add(dto);
                        }
                    }
                }
                return lstDto;
            }
        }

        public PageResult<OrderItemDto> GetOrderItemsAutoBack(string orderNo, int pageIndex, int pageSize)
        {
            using (var db = new YintaiHZhouContext())
            {
                IQueryable<OrderItem> query = db.OrderItems.Where(t => t.OrderNo == orderNo);

                var filter = from q in query
                    join b in db.Brands on q.BrandId equals b.Id into cs
                    select new {Order = q, Brand = cs.FirstOrDefault()};
                filter = filter.OrderByDescending(t => t.Order.CreateDate);
                var list = filter.ToPageResult(pageIndex, pageSize);
                IList<OrderItemDto> lstDtos = new List<OrderItemDto>();
                foreach (var t in list.Result)
                {
                    OrderItemDto o = Mapper.Map<OrderItem, OrderItemDto>(t.Order);
                    o.BrandName = t.Brand == null ? "" : t.Brand.Name;

                    o.NeedReturnCount = t.Order.Quantity;
                    o.ReturnCount = t.Order.Quantity;

                    lstDtos.Add(o);
                }
                return new PageResult<OrderItemDto>(lstDtos, list.TotalCount);
            }
        }

        public void SetSaleOrderVoid(string saleOrderNo)
        {
            using (var db = new YintaiHZhouContext())
            {
                var sale = db.OPC_Sale.Where(t => t.SaleOrderNo == saleOrderNo).FirstOrDefault();
                if (sale == null)
                {
                    throw new Exception("销售单不存在，销售单号:"+saleOrderNo);
                }
                sale.Status = EnumSaleOrderStatus.Void.AsID();
                
                db.OPC_Sale.AddOrUpdate(sale);
            
                db.SaveChanges();
            }
        }

        #endregion
    }
}