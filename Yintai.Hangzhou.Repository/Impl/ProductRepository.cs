using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using Yintai.Architecture.Common.Data.EF;
using Yintai.Architecture.Common.Models;
using Yintai.Hangzhou.Data.Models;
using Yintai.Hangzhou.Model.Enums;
using Yintai.Hangzhou.Model.Filters;
using Yintai.Hangzhou.Repository.Contract;

namespace Yintai.Hangzhou.Repository.Impl
{
    public class ProductRepository : RepositoryBase<ProductEntity, int>, IProductRepository
    {
        private readonly ISpecialTopicProductRelationRepository _specialTopicProductRelationRepository;

        public ProductRepository(ISpecialTopicProductRelationRepository specialTopicProductRelationRepository)
        {
            _specialTopicProductRelationRepository = specialTopicProductRelationRepository;
        }

        #region methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private List<int> GetTopicRelationIds(int? id)
        {
            if (id == null || id.Value < 1)
            {
                return new List<int>(0);
            }

            var s = new List<int> { id.Value };

            return GetTopicRelationIds(s);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        private List<int> GetTopicRelationIds(List<int> ids)
        {
            if (ids == null || ids.Count == 0)
            {
                return new List<int>(0);
            }

            var entitys = _specialTopicProductRelationRepository.GetList(ids);

            if (entitys == null || entitys.Count == 0)
            {
                return new List<int>(0);
            }

            var result = entitys.Select(v => v.Product_Id).Distinct().ToList();

            return result;
        }

        private static Expression<Func<ProductEntity, bool>> Filter(DataStatus? dataStatus, Timestamp timestamp,
                                                                       ICollection<int> tagid,
                                                                       int? recommendUser, int? brandId = null, List<int> productIds = null)
        {
            var filter = PredicateBuilder.True<ProductEntity>();

            if (dataStatus != null)
            {
                filter = filter.And(v => v.Status == (int)dataStatus);
            }

            if (timestamp != null)
            {
                switch (timestamp.TsType)
                {
                    case TimestampType.New:
                        filter = filter.And(v => v.UpdatedDate > timestamp.Ts);
                        break;
                    case TimestampType.Old:
                    default:
                        filter = filter.And(v => v.UpdatedDate <= timestamp.Ts);
                        break;
                }
            }

            if (tagid != null && tagid.Count > 0)
            {
                filter = filter.And(v => tagid.Any(s => s == v.Tag_Id));
            }

            if (recommendUser != null)
            {
                filter = filter.And(v => v.RecommendUser == recommendUser.Value);
            }

            if (brandId != null)
            {
                filter = filter.And(v => v.Brand_Id == brandId);
            }

            if (productIds != null && productIds.Count > 0)
            {
                filter = filter.And(v => productIds.Any(s => s == v.Id));
            }

            return filter;
        }

        private Expression<Func<ProductEntity, bool>> Filter(ProductFilter productFilter)
        {
            if (productFilter == null)
            {
                return null;
            }

            List<int> pids = null;
            if (productFilter.TopicId != null && productFilter.TopicId > 0)
            {
                pids = GetTopicRelationIds(productFilter.TopicId);
            }

            return Filter(productFilter.DataStatus, productFilter.Timestamp, productFilter.TagIds,
                          productFilter.RecommendUser, productFilter.BrandId, pids);
        }

        /// <summary>
        /// 排序
        /// </summary>
        /// <param name="sort"></param>
        /// <returns></returns>
        private static Func<IQueryable<ProductEntity>, IOrderedQueryable<ProductEntity>> GetOrder(ProductSortOrder sort)
        {
            Func<IQueryable<ProductEntity>, IOrderedQueryable<ProductEntity>> order = null;
            switch (sort)
            {
                case ProductSortOrder.CreatedDateDesc:
                case ProductSortOrder.Default:
                default:
                    order = v => v.OrderByDescending(s => s.SortOrder).ThenByDescending(s => s.CreatedDate);
                    break;
            }

            return order;
        }

        #endregion

        public List<ProductEntity> GetPagedListForSearch(PagerRequest pagerRequest, out int totalCount, ProductSortOrder sortOrder, Timestamp timestamp,
                                        string productName, int? recommendUser, List<int> tagids, int? brandId)
        {
            var filter = Filter(DataStatus.Normal, timestamp, tagids, recommendUser, brandId);

            if (!String.IsNullOrWhiteSpace(productName))
            {
                filter = filter.And(v => v.Name.StartsWith(productName));
            }

            return
                base.Get(filter, out totalCount, pagerRequest.PageIndex, pagerRequest.PageSize, GetOrder(sortOrder))
                    .ToList();
        }

        public List<ProductEntity> GetPagedList(PagerRequest pagerRequest, out int totalCount, ProductSortOrder sortOrder, Timestamp timestamp,
                                 int? tagId, int? recommendUser, int? brandId)
        {
            List<int> tags;
            if (tagId == null)
            {
                tags = null;
            }
            else
            {
                tags = new List<int>(1) { tagId.Value };
            }

            return
                base.Get(Filter(DataStatus.Normal, timestamp, tags, recommendUser, brandId), out totalCount,
                         pagerRequest.PageIndex, pagerRequest.PageSize, GetOrder(sortOrder)).ToList();
        }

        public List<ProductEntity> GetList(List<int> ids)
        {
            return base.Get(v => ids.Any(s => s == v.Id) && v.Status == (int)DataStatus.Normal).ToList();
        }

        public ProductEntity SetSortOrder(ProductEntity entity, int sortOrder, int updateUser)
        {
            var parames = new List<SqlParameter>
                {
                    new SqlParameter("@SortOrder", sortOrder),
                    new SqlParameter("@Id", entity.Id),
                };


            const string sql = "UPDATE [dbo].[Product] SET [SortOrder] = @SortOrder WHERE Id = @Id;";

            var i = SqlHelper.ExecuteNonQuery(SqlHelper.GetConnection(), CommandType.Text, sql, parames.ToArray());

            if (i > 0)
            {
                return GetItem(entity.Id);
            }
            else
            {
                return null;
            }
        }

        public List<ProductEntity> GetPagedList(PagerRequest pagerRequest, out int totalCount, ProductSortOrder sortOrder, ProductFilter productFilter)
        {
            return
               base.Get(Filter(productFilter), out totalCount, pagerRequest.PageIndex, pagerRequest.PageSize, GetOrder(sortOrder))
                   .ToList();
        }

        public override ProductEntity GetItem(int key)
        {
            return base.Find(key);
        }
    }
}
