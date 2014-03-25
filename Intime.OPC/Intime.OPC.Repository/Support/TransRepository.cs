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

        /// <summary>
        ///     对订单、销售单、销售明细进行备注
        /// </summary>
        /// <param name="id"></param>
        /// <param name="remark"></param>
        /// <param name="strType"></param>
        /// <returns></returns>
        public ResultMsg InputRemark(string strID, string remark, string strType)
        {
            using (var db = new YintaiHZhouContext())
            {
                int intid = int.Parse(strID);
                var msg = new ResultMsg();
                try
                {
                    //订单
                    if (strType == "1")
                    {
                    }
                        //销售单
                    else if (strType == "2")
                    {
                        OPC_SaleComment entity4Add = db.OPC_SaleComment.Create();
                        entity4Add.Content = remark;
                        entity4Add.CreateDate = DateTime.Now;
                        entity4Add.CreateUser = 1;
                        entity4Add.UpdateDate = DateTime.Now;
                        entity4Add.UpdateUser = 1;
                        db.OPC_SaleComment.Add(entity4Add);
                    }
                        //销售单备注
                    else if (strType == "3")
                    {
                        OPC_SaleComment entity4Add = db.OPC_SaleComment.Create();
                        entity4Add.Content = remark;
                        entity4Add.CreateDate = DateTime.Now;
                        entity4Add.CreateUser = 1;
                        entity4Add.UpdateDate = DateTime.Now;
                        entity4Add.UpdateUser = 1;
                        db.OPC_SaleComment.Add(entity4Add);
                    }
                    msg.IsSuccess = true;
                    msg.Msg = "";
                }
                catch
                {
                    msg.IsSuccess = false;
                    msg.Msg = "";
                }
                return msg;
            }
        }

        #endregion
    }
}