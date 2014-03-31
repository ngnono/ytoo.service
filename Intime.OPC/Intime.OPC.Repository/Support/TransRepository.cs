using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Intime.OPC.Domain;
using Intime.OPC.Domain.Models;
using Intime.OPC.Repository.Base;

namespace Intime.OPC.Repository.Support
{
    public class TransRepository : BaseRepository<OPC_Sale>, ITransRepository
    {
        #region ITransRepository Members

        public PageResult<OPC_Sale> Select(DateTime startDate, DateTime endDate, string orderNo, string saleOrderNo, int pageIndex, int pageSize = 20)
        {

                DateTime dateNow = DateTime.Now;

                Expression<Func<OPC_Sale, bool>> filterExpression =
               t => t.SellDate >= startDate && t.SellDate < endDate;

                
                if (startDate != dateNow)
                {
                    filterExpression = filterExpression.And(p => p.SellDate >= startDate);
                }
                if (endDate != dateNow)
                {
                    filterExpression = filterExpression.And(p => p.SellDate <= endDate);
                }
                if (!string.IsNullOrEmpty(orderNo))
                {
                    filterExpression = filterExpression.And(p => p.OrderNo.Contains(orderNo));
                }
                if (!string.IsNullOrEmpty(saleOrderNo))
                {
                    filterExpression = filterExpression.And(p => p.SaleOrderNo.Contains(saleOrderNo));
                }
                return Select(filterExpression, pageIndex, pageSize);

        }

        public bool Finish(Dictionary<string, string> sale)
        {
            using (var db = new YintaiHZhouContext())
            {
                int intid = int.Parse(sale["id"]);
                int intstatus = int.Parse(sale["status"]);

                OPC_Sale user = db.OPC_Sale.Where(e => e.Id == intid).FirstOrDefault();
                if (user != null)
                {
                    user.Status = intstatus;
                    db.SaveChanges();
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        ///     查询销售单明细信息
        /// </summary>
        /// <param name="saleIDs">销售单ID串</param>
        /// <returns></returns>
        public IList<OPC_SaleDetail> SelectSaleDetail(IEnumerable<string> saleNos)
        {
            using (var db = new YintaiHZhouContext())
            {
                return db.OPC_SaleDetail.Where(t => saleNos.Contains(t.SaleOrderNo)).ToList();
            }
        }

        #endregion
    }
}