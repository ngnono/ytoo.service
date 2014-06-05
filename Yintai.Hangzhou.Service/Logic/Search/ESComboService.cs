using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Yintai.Architecture.Framework;
using Yintai.Architecture.Framework.ServiceLocation;
using Yintai.Hangzhou.Data.Models;
using Yintai.Hangzhou.Model.Enums;
using Yintai.Hangzhou.Model.ES;
using Yintai.Hangzhou.Model.ESModel;

namespace Yintai.Hangzhou.Service.Logic.Search
{
    class ESComboService : ESServiceBase
    {
        public override bool IndexSingle(int entityId)
        {

            var esCombo = combo2ESCombo(entityId);
            return SearchLogic.IndexSingle<ESCombo>(esCombo);
        }

        private ESCombo combo2ESCombo(int entityId)
        {
            var db = Context;
            var brandLinq = db.Set<IMS_Combo2ProductEntity>().Where(ic => ic.ComboId == entityId)
                            .Join(db.Set<ProductEntity>(), o => o.ProductId, i => i.Id, (o, i) => i)
                            .Join(db.Set<BrandEntity>(), o => o.Brand_Id, i => i.Id, (o, i) => i).ToList();
            return db.Set<IMS_ComboEntity>().Where(ic => ic.Id == entityId)
                    .Join(db.Set<IMS_AssociateItemsEntity>().Where(iai => iai.ItemType == (int)ComboType.Product), o => o.Id, i => i.ItemId, (o, i) => new { C = o, AI = i })
                    .Join(db.Set<IMS_AssociateEntity>(), o => o.AI.AssociateId, i => i.Id, (o, i) => new { C = o.C, A = i })
                    .GroupJoin(db.Set<ResourceEntity>().Where(r => r.Status == (int)DataStatus.Normal && r.SourceType == (int)SourceType.Combo),
                                o => o.C.Id,
                                i => i.SourceId,
                                (o, i) => new { C = o.C, A = o.A, R = i.OrderByDescending(ri => ri.SortOrder) })
                     .ToList()
                     .Select(l =>Mapper.Map<IMS_ComboEntity,ESCombo>(l.C,target=>{
                               target.Resources = l.R == null ? null : l.R.Select(r => Mapper.Map<ResourceEntity,ESResource>(r));
                               target.AssociateId = l.A.Id;
                               target.StoreId = l.A.StoreId;
                               target.Brands = brandLinq.Select(b =>Mapper.Map<BrandEntity,ESBrand>(b));
                            })).FirstOrDefault();
                        
        }
        private DbContext Context
        {
            get { return ServiceLocator.Current.Resolve<DbContext>(); }
        }
    }
}
