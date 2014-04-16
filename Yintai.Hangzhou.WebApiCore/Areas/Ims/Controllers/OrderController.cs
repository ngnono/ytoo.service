using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Yintai.Architecture.Common.Models;
using Yintai.Hangzhou.Contract.DTO.Request;
using Yintai.Hangzhou.Contract.DTO.Response;
using Yintai.Hangzhou.Data.Models;
using Yintai.Hangzhou.Model.Enums;
using Yintai.Hangzhou.WebSupport.Mvc;
using com.intime.fashion.common;
using com.intime.fashion.common.Extension;

namespace Yintai.Hangzhou.WebApiCore.Areas.Ims.Controllers
{
    public class OrderController:RestfulController
    {
        [RestfulAuthorize]
        public ActionResult My(PagerInfoRequest request,int authuid)
        {
            var itemLinq = Context.Set<OrderItemEntity>()
                          .GroupJoin(Context.Set<ResourceEntity>().Where(r => r.SourceType == (int)SourceType.Product && r.Status == (int)DataStatus.Normal && r.Type == (int)ResourceType.Image),
                                        o => new { Product = o.ProductId, Color = o.ColorValueId },
                                        i => new { Product = i.SourceId, Color = i.ColorId },
                                        (o, i) => new { OI = o, R = i.OrderByDescending(ir => ir.SortOrder).FirstOrDefault() });
            var linq = Context.Set<OrderEntity>().Where(o => o.CustomerId == authuid)
                       .GroupJoin(itemLinq, o => o.OrderNo, i => i.OI.OrderNo, (o, i) => new { Order = o,OI=i});

            int totalCount = linq.Count();
            int skipCount = request.Page > 0 ? (request.Page - 1) * request.Pagesize : 0;
            linq = linq.OrderByDescending(l => l.Order.CreateDate).Skip(skipCount).Take(request.Pagesize);
            var result = linq.ToList().Select(l => new 
            {
                order_no = l.Order.OrderNo,
                amount = l.Order.TotalAmount,
                create_date = l.Order.CreateDate,
                status = ((OrderStatus)l.Order.Status).ToFriendlyString(),
                status_i = l.Order.Status,
                shipping_name = l.Order.ShippingContactPerson,
                products = l.OI.ToList().Select(p=>new {
                    id = p.OI.ProductId,
                    name = p.OI.ProductName,
                    price = p.OI.ItemPrice,
                    image = p.R.Name.Image320Url()
                })
            });
            var response = new PagerInfoResponse<dynamic>(request.PagerRequest, totalCount)
            {
                Items = result.ToList<dynamic>()
            };

            return this.RenderSuccess<PagerInfoResponse<dynamic>>(c => c.Data = response);
            
        }
    }
}
