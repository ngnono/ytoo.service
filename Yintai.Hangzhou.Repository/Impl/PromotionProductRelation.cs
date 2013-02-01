using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yintai.Hangzhou.Data.Models;
using Yintai.Hangzhou.Repository.Contract;

namespace Yintai.Hangzhou.Repository.Impl
{
    public class PromotionProductRelationRepository : RepositoryBase<Promotion2ProductEntity, int>, IPromotionProductRelationRepository
    {
        public override Promotion2ProductEntity GetItem(int key)
        {
            return base.Find(key);
        }

        public List<Promotion2ProductEntity> GetList(int promotionId)
        {
            throw new NotImplementedException();
        }

        public List<Promotion2ProductEntity> GetList4Product(List<int> productids)
        {
            throw new NotImplementedException();
        }
    }
}
