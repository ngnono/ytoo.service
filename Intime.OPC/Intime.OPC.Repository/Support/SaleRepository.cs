using Intime.OPC.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intime.OPC.Repository.Support
{
    public class SaleRepository:ISaleRepository
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
    }
}
