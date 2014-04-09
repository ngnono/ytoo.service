using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Intime.OPC.Domain.Dto.Custom;
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

        public IList<OPC_RMA> GetAll(string orderNo, string saleOrderNo, DateTime startTime, DateTime endTime)
        {
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
                return query.ToList();
            }
        }
    }
}