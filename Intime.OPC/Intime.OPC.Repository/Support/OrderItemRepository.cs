using System.Collections.Generic;
using System.Linq;
using Intime.OPC.Domain;
using Intime.OPC.Domain.Dto;
using Intime.OPC.Domain.Models;
using Intime.OPC.Repository.Base;

namespace Intime.OPC.Repository.Support
{
    public class OrderItemRepository : BaseRepository<OrderItem>, IOrderItemRepository
    {
        public PageResult<OrderItemDto> GetByOrderNo(string orderNo, int pageIndex, int pageSize)
        {
            using (var db = new YintaiHZhouContext())
            {
                
                var query= db.OrderItems.Where(t => t.OrderNo == orderNo);

                var query2 = Queryable.Join(db.OrderItems.Where(t => t.OrderNo == orderNo), db.OPC_RMADetail, t => t.Id,
                    o => o.OrderItemId, (t, o) => o);
                var filter = from q in query
                    join b in db.Brands on q.BrandId equals b.Id into cs
                    join rmaDetail in query2 on q.Id equals rmaDetail.OrderItemId into rma
                           select new { Order=q,Brand=cs.FirstOrDefault(),Rma=rma.FirstOrDefault()};
                filter = filter.OrderByDescending(t => t.Order.CreateDate);
                var list = filter.ToPageResult(pageIndex, pageSize);
                IList<OrderItemDto>  lstDtos=new List<OrderItemDto>();
                foreach (var t in list.Result)
                {

                    var o = AutoMapper.Mapper.Map<OrderItem, OrderItemDto>(t.Order);
                    o.BrandName = t.Brand == null ? "" : t.Brand.Name;
                    if (t.Rma!=null)
                    {
                        o.NeedReturnCount = t.Rma.BackCount;
                        o.ReturnCount = t.Rma.BackCount;
                    }
                    lstDtos.Add(o);

                }
                return new PageResult<OrderItemDto>(lstDtos,list.TotalCount);
            }
        }

        public IList<OrderItem> GetByIDs(IEnumerable<int> ids)
        {
            return Select(t => ids.Contains(t.Id));
        }
    }
}