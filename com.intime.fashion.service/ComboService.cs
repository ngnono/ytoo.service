using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yintai.Hangzhou.Data.Models;

namespace com.intime.fashion.service
{
    public partial class ComboService:BusinessServiceBase
    {
        public void RefreshPrice(IMS_ComboEntity combo)
        {
            if (combo == null)
                return;
            var products = _db.Set<IMS_Combo2ProductEntity>()
                .Where(icp => icp.ComboId == combo.Id)
                .Join(_db.Set<ProductEntity>(), o => o.ProductId, i => i.Id, (o, i) => i);
            var totalPrice = products.Sum(p => (decimal?)p.Price);
            combo.Price = totalPrice??0m;
            combo.UpdateDate = DateTime.Now;
            _comboRepo.Update(combo);
        }
        public void RefreshPrice(int productId)
        {
            foreach (var combo in _db.Set<IMS_Combo2ProductEntity>().Where(icp => icp.ProductId == productId)
                                    .Join(_db.Set<IMS_ComboEntity>(),o=>o.ComboId,i=>i.Id,(o,i)=>i))
            {
                RefreshPrice(combo);
            }
        }
    }
}
