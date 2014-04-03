using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Intime.OPC.Domain;
using Intime.OPC.Domain.Models;
using Intime.OPC.Repository.Base;

namespace Intime.OPC.Repository.Support
{
    public class ShippingSaleRepository : BaseRepository<OPC_ShippingSale>, IShippingSaleRepository
    {
        public PageResult<OPC_ShippingSale> GetBySaleOrderNo(string saleNo, int pageIndex, int pageSize = 20)
        {
            return Select(t => t.SaleOrderNo == saleNo,t=>t.UpdateDate,false, pageIndex,pageSize);
        }

        public PageResult<OPC_ShippingSale> GetByShippingCode(string shippingCode, int pageIndex, int pageSize = 20)
        {
            return Select(t => t.ShippingCode == shippingCode,t=>t.UpdateDate,false, pageIndex, pageSize);
        }

        public PageResult<OPC_ShippingSale> Get(string shippingCode, DateTime startTime, DateTime endTime, int shippingStatus, int pageIndex, int pageSize = 20)
        {
            Expression<Func<OPC_ShippingSale, bool>> filterExpression =
                t => t.CreateDate >= startTime && t.CreateDate < endTime && t.ShippingStatus==shippingStatus;
            if (!string.IsNullOrWhiteSpace(shippingCode))
            {
                filterExpression.And(t => t.ShippingCode.Contains(shippingCode));
            }
            return Select(filterExpression,t=>t.UpdateDate,false, pageIndex, pageSize);
        }


        public PageResult<OPC_ShippingSale> GetShippingSale(string saleOrderNo, string expressNo, DateTime startGoodsOutDate, 
            DateTime endGoodsOutDate, int sectionId, int shippingStatus, 
            string customerPhone, int brandId, int pageIndex, int pageSize)
        {
            Expression<Func<OPC_ShippingSale, bool>> filterExpression =
                t => t.CreateDate >= startGoodsOutDate && t.CreateDate < endGoodsOutDate && t.ShippingStatus == shippingStatus;
            if (string.IsNullOrWhiteSpace(saleOrderNo))
            {
                filterExpression = filterExpression.And(t => t.SaleOrderNo.Contains( saleOrderNo));
            }

            if (string.IsNullOrWhiteSpace(expressNo))
            {
                filterExpression = filterExpression.And(t => t.ShippingCode.Contains(expressNo) );
            }

            if (sectionId>0)
            {
                //filterExpression = filterExpression.And(t => t(expressNo));
            }
            return Select(filterExpression, t => t.CreateDate, false, pageIndex, pageSize);

        }


        public PageResult<OPC_ShippingSale> GetShippingSale(string saleOrderNo, string expressNo, DateTime startGoodsOutDate, DateTime endGoodsOutDate, string outGoodsCode, int sectionId, int shippingStatus, string customerPhone, int brandId, int pageIndex, int pageSize)
        {
            Expression<Func<OPC_ShippingSale, bool>> filterExpression =
               t => t.CreateDate >= startGoodsOutDate && t.CreateDate < endGoodsOutDate && t.ShippingStatus == shippingStatus;
            if (string.IsNullOrWhiteSpace(saleOrderNo))
            {
                filterExpression = filterExpression.And(t => t.SaleOrderNo.Contains(saleOrderNo));
            }

            if (string.IsNullOrWhiteSpace(expressNo))
            {
                filterExpression = filterExpression.And(t => t.ShippingCode.Contains(expressNo));
            }

            if (sectionId > 0)
            {
                //filterExpression = filterExpression.And(t => t(expressNo));
            }
            return Select(filterExpression, t => t.CreateDate, false, pageIndex, pageSize);
        }
    }
}