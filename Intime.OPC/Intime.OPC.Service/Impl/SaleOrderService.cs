using System;
using Intime.OPC.Domain.Dto;
using Intime.OPC.Domain.Exception;
using Intime.OPC.Domain.Models;
using Intime.OPC.Service.Contract;
using System.Collections.Generic;
using System.Linq;
using YintaiHZhouContext = Intime.OPC.Repository.YintaiHZhouContext;

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
                        .Join(db.OrderItems, d => d.OrderItemId, i => i.Id, (o, i) => new {o, i})
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

        public void CommentSaleOrder(string comment,string saleOrderNo, int uid)
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
                    UpdateUser =  uid,
                });
            }
        }

        public IList<OPC_Sale> GetSaleOrdersByPachageId(int packageId)
        {
            using (var db = new Domain.Models.YintaiHZhouContext())
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
    }
}
