using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intime.OPC.Domain.Models;
using Intime.OPC.Repository.Base;

namespace Intime.OPC.Repository.Support
{
    public class SaleDetailRepository:BaseRepository<OPC_SaleDetail>, ISaleDetailRepository
    {
        public IList<OPC_SaleDetail> GetBySaleOrderNo(string saleOrderNo)
        {
            return  Select(t => t.SaleOrderNo == saleOrderNo).ToList();
        }

        public IList<OPC_SaleDetail> GetByOrderNo(string orderNo)
        {
            using (var db = new YintaiHZhouContext())
            {
                return db.OPC_Sale.Where(t => t.OrderNo == orderNo).Join(db.OPC_SaleDetail, t => t.SaleOrderNo,
                    o => o.SaleOrderNo, (t, o) => o).ToList();
            }
        }
    }
}
