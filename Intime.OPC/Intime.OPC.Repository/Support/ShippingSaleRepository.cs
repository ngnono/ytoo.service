using System;
using System.Linq;
using System.Linq.Expressions;
using Intime.OPC.Domain;
using Intime.OPC.Domain.Exception;
using Intime.OPC.Domain.Models;
using Intime.OPC.Repository.Base;

namespace Intime.OPC.Repository.Support
{
    public class ShippingSaleRepository : BaseRepository<OPC_ShippingSale>, IShippingSaleRepository
    {
        #region IShippingSaleRepository Members

        public OPC_ShippingSale GetBySaleOrderNo(string saleOrderNo)
        {
            using (var db = new YintaiHZhouContext())
            {
                OPC_Sale sale = db.OPC_Sale.FirstOrDefault(t => t.SaleOrderNo == saleOrderNo);
                if (sale == null)
                {
                    throw new SaleOrderNotExistsException(saleOrderNo);
                }
                return db.ShippingSales.FirstOrDefault(t => t.ShippingCode == sale.ShippingCode);
            }
        }

        public PageResult<OPC_ShippingSale> GetByShippingCode(string shippingCode, int pageIndex, int pageSize = 20)
        {
            return Select(t => t.ShippingCode == shippingCode, t => t.UpdateDate, false, pageIndex, pageSize);
        }

        public PageResult<OPC_ShippingSale> Get(string shippingCode, DateTime startTime, DateTime endTime,
            int shippingStatus, int pageIndex, int pageSize = 20)
        {
            Expression<Func<OPC_ShippingSale, bool>> filterExpression =
                t => t.CreateDate >= startTime && t.CreateDate < endTime && t.ShippingStatus == shippingStatus;
            if (!string.IsNullOrWhiteSpace(shippingCode))
            {
                filterExpression.And(t => t.ShippingCode.Contains(shippingCode));
            }
            return Select(filterExpression, t => t.UpdateDate, false, pageIndex, pageSize);
        }

        public PageResult<OPC_ShippingSale> GetShippingSale(string saleOrderNo, string expressNo,
            DateTime startGoodsOutDate, DateTime endGoodsOutDate, string outGoodsCode, int sectionId, int shippingStatus,
            string customerPhone, int brandId, int pageIndex, int pageSize)
        {
            //Expression<Func<OPC_ShippingSale, bool>> filterExpression =
            //   t => t.CreateDate >= startGoodsOutDate && t.CreateDate < endGoodsOutDate ;
            ////todo 增加订单查询条件
            //if (shippingStatus>-1)
            //{
            //    filterExpression = filterExpression.And(t => t.ShippingStatus == shippingStatus);
            //}

            //if (!string.IsNullOrWhiteSpace(expressNo))
            //{
            //    filterExpression = filterExpression.And(t => t.ShippingCode.Contains(expressNo));
            //}

            //return Select(filterExpression, t => t.CreateDate, false, pageIndex, pageSize);

            using (var db = new YintaiHZhouContext())
            {
                IQueryable<OPC_ShippingSale> query =
                    db.ShippingSales.Where(t => t.CreateDate >= startGoodsOutDate && t.CreateDate < endGoodsOutDate);
                if (shippingStatus > -1)
                {
                    query = query.Where(t => t.ShippingStatus == shippingStatus);
                }

                if (!string.IsNullOrWhiteSpace(expressNo))
                {
                    query = query.Where(t => t.ShippingCode.Contains(expressNo));
                }
                query = query.OrderBy(t => t.CreateDate);
                return query.ToPageResult(pageIndex, pageSize);
            }
        }

        #endregion

        public PageResult<OPC_ShippingSale> GetShippingSale(string saleOrderNo, string expressNo,
            DateTime startGoodsOutDate,
            DateTime endGoodsOutDate, int sectionId, int shippingStatus,
            string customerPhone, int brandId, int pageIndex, int pageSize)
        {
            Expression<Func<OPC_ShippingSale, bool>> filterExpression =
                t =>
                    t.CreateDate >= startGoodsOutDate && t.CreateDate < endGoodsOutDate &&
                    t.ShippingStatus == shippingStatus;

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