using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Intime.OPC.Domain.Models;
using Intime.OPC.Repository.Base;

namespace Intime.OPC.Repository.Support
{
    public class ShippingSaleRepository : BaseRepository<OPC_ShippingSale>, IShippingSaleRepository
    {
        public IList<OPC_ShippingSale> GetBySaleOrderNo(string saleNo)
        {
            return Select(t => t.SaleOrderNo == saleNo).ToList();
        }

        public IList<OPC_ShippingSale> GetByShippingCode(string shippingCode)
        {
            return Select(t => t.ShippingCode == shippingCode).ToList();
        }

        public IList<OPC_ShippingSale> Get(string shippingCode, DateTime startTime, DateTime endTime)
        {
            Expression<Func<OPC_ShippingSale, bool>> filterExpression =
                t => t.CreateDate >= startTime && t.CreateDate < endTime;
            if (!string.IsNullOrWhiteSpace(shippingCode))
            {
                filterExpression.And(t => t.ShippingCode.Contains(shippingCode));
            }
            return Select(filterExpression).ToList();
        }
    }
}