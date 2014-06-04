using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
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
            return  Context.Set<IMS_ComboEntity>().Where(ic => ic.Id == entityId)
                    .GroupJoin(Context.Set<ResourceEntity>().Where(r => r.Status == (int)DataStatus.Normal && r.SourceType == (int)SourceType.Combo),
                                o => o.Id,
                                i => i.SourceId,
                                (o, i) => new { C=o,R=i.OrderByDescending(ri=>ri.SortOrder)})
                     .ToList()
                     .Select(l=>new ESCombo()
            {
                Id = l.C.Id,
                CreateDate = l.C.CreateDate,
                CreateUser = l.C.CreateUser,
                Desc = l.C.Desc,
                ExpireDate = l.C.ExpireDate,
                OnlineDate = l.C.OnlineDate,
                Price = l.C.Price,
                Private2Name = l.C.Private2Name,
                ProductType = l.C.ProductType,
                Status = l.C.Status,
                UpdateDate = l.C.UpdateDate,
                UpdateUser = l.C.UpdateUser,
                UserId = l.C.UserId,
                Resources = l.R==null?null:l.R.Select(r=>new ESResource()
                                       {
                                           Domain = r.Domain,
                                           Name = r.Name,
                                           SortOrder = r.SortOrder,
                                           IsDefault = r.IsDefault,
                                           Type = r.Type,
                                           Width = r.Width,
                                           Height = r.Height
                                       })
            }).FirstOrDefault();
        }
        private DbContext Context
        {
            get { return ServiceLocator.Current.Resolve<DbContext>(); }
        }
    }
}
