using System;
using System.Collections.Generic;
using System.Linq;
using Intime.OPC.Domain.Models;

namespace Intime.OPC.Repository.Support
{
    public class TransRepository : ITransRepository
    {
        #region ITransRepository Members

        public IList<OPC_Sale> Select(string startDate, string endDate, string orderNo, string saleOrderNo)
        {
            using (var db = new YintaiHZhouContext())
            {
                DateTime dateNow = DateTime.Now;
                DateTime dateStart = dateNow;
                DateTime dateEnd = dateNow;
                if (!string.IsNullOrEmpty(startDate))
                {
                    dateStart = Convert.ToDateTime(startDate);
                }
                if (!string.IsNullOrEmpty(endDate))
                {
                    dateEnd = Convert.ToDateTime(endDate);
                }

                IQueryable<OPC_Sale> saleList = db.OPC_Sale.Where(e => 1 == 1);
                if (dateStart != dateNow)
                {
                    saleList = saleList.Where(p => p.SellDate >= dateStart);
                }
                if (dateEnd != dateNow)
                {
                    saleList = saleList.Where(p => p.SellDate <= dateEnd);
                }
                if (!string.IsNullOrEmpty(orderNo))
                {
                    saleList = saleList.Where(p => p.OrderNo.Contains(orderNo));
                }
                if (!string.IsNullOrEmpty(saleOrderNo))
                {
                    saleList = saleList.Where(p => p.SaleOrderNo.Contains(saleOrderNo));
                }
                return saleList.ToList();
            }
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