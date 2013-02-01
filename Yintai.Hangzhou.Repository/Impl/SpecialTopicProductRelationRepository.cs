using System.Collections.Generic;
using System.Linq;
using Yintai.Hangzhou.Data.Models;
using Yintai.Hangzhou.Model.Enums;
using Yintai.Hangzhou.Repository.Contract;

namespace Yintai.Hangzhou.Repository.Impl
{
    public class SpecialTopicProductRelationRepository : RepositoryBase<SpecialTopicProductRelationEntity, int>, ISpecialTopicProductRelationRepository
    {
        public override SpecialTopicProductRelationEntity GetItem(int key)
        {
            return base.Find(key);
        }

        public List<SpecialTopicProductRelationEntity> GetList(int specialTopicId)
        {
            return base.Get(v => v.SpecialTopic_Id == specialTopicId && v.Status == (int)DataStatus.Normal).ToList();
        }

        public List<SpecialTopicProductRelationEntity> GetList(List<int> ids)
        {
            return base.Get(v => ids.Any(s => s == v.SpecialTopic_Id) && v.Status == (int)DataStatus.Normal).ToList();
        }
    }
}
