using Intime.OPC.Domain.Enums;
using Intime.OPC.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intime.OPC.Repository.Base;

namespace Intime.OPC.Repository.Support
{
    public class SaleRepository:BaseRepository<OPC_Sale> ,ISaleRepository
    {
        public bool UpdateSatus(OPC_Sale sale)
        {
            using (var db = new YintaiHZhouContext())
            {
                OPC_Sale saleEnt = db.OPC_Sale.Where(e => e.Id == sale.Id).FirstOrDefault();
                if (saleEnt != null)
                {
                    saleEnt.Status = sale.Status;
                    db.SaveChanges();
                    return true;

                }
                return false;
            }
        }

        public IList<OPC_Sale> Select()
        {
            using (var db = new YintaiHZhouContext())
            {
                var saleList = db.OPC_Sale.ToList();
                return saleList;
            }
        }

        public OPC_Sale GetBySaleNo(string saleNo)
        {
            using (var db = new YintaiHZhouContext())
            {
                return db.OPC_Sale.FirstOrDefault(t => t.SaleOrderNo == saleNo);
            }
        }

        public IList<OPC_SaleDetail> GetSaleOrderDetails(string saleOrderNo)
        {
            using (var db = new YintaiHZhouContext())
            {
                return db.OPC_SaleDetail.Where(t=>t.SaleOrderNo == saleOrderNo).ToList();
            }
        }

        public bool UpdateSatus(IEnumerable<string> saleNos, EnumSaleOrderStatus saleOrderStatus, int userID)
        {
            using (var db = new YintaiHZhouContext())
            {
                foreach (var saleNo in saleNos)
                {
                    var sale= db.OPC_Sale.FirstOrDefault(t => t.SaleOrderNo == saleNo);
                    if (sale !=null)
                    {
                        sale.UpdatedDate = DateTime.Now;
                        sale.UpdatedUser = userID;
                        sale.Status = (int) saleOrderStatus;
                        
                    }
                }
                try
                {
                    db.SaveChanges();
                    return true;
                }
                catch (Exception ex)
                {
                    //todo 增加错误日志
                    return false;
                }
                
            }
        }
    }
}
