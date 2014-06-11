using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yintai.Architecture.Framework.ServiceLocation;
using Yintai.Hangzhou.Data.Models;
using Yintai.Hangzhou.Model.ESModel;

namespace com.intime.fashion.service.search
{
    class ESInventoryService : ESServiceBase
    {
        public override bool IndexSingle(int entityId)
        {

            var esStock = stock2ES(entityId);
            return SearchLogic.IndexSingle<ESStock>(esStock);
        }

        private ESStock stock2ES(int id)
        {
            var db = Context;
            var inventoryEntity = db.Set<InventoryEntity>().Where(i => i.Id == id)
                                .Join(db.Set<ProductEntity>(), o => o.ProductId, i => i.Id, (o, i) => new { I = o, P = i }).FirstOrDefault();
            if (inventoryEntity == null)
                return null;
            return new ESStock()
                    {
                        ProductId = inventoryEntity.I.ProductId,
                        Amount = inventoryEntity.I.Amount,
                        ColorValueId = inventoryEntity.I.PColorId,
                        SizeValueId = inventoryEntity.I.PSizeId,
                        Id = inventoryEntity.I.Id,
                        LabelPrice = inventoryEntity.P.UnitPrice.HasValue ? inventoryEntity.P.UnitPrice.Value : 999999,
                        Price = inventoryEntity.P.Price,
                        UpdateDate = DateTime.Now,
                    };
        }
        private DbContext Context
        {
            get { return ServiceLocator.Current.Resolve<DbContext>(); }
        }
    }
}
