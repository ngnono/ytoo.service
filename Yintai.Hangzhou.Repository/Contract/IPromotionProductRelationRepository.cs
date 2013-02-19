using System.Collections.Generic;
using Yintai.Hangzhou.Data.Models;

namespace Yintai.Hangzhou.Repository.Contract
{
    public interface IPromotionProductRelationRepository : IRepository<Promotion2ProductEntity, int>
    {
        List<Promotion2ProductEntity> GetList(int promotionId);

        List<Promotion2ProductEntity> GetList4Product(List<int> productids);

        bool Exists(int promotionid, int productid);

        void DeletedByProduct(int productId);
    }
}
