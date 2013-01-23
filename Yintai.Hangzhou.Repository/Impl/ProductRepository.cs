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
using Yintai.Hangzhou.Repository.Contract;

namespace Yintai.Hangzhou.Repository.Impl
{
    public class ProductRepository : RepositoryBase<ProductEntity, int>, IProductRepository
    {
        #region methods

        private static Expression<Func<ProductEntity, bool>> Filter(DataStatus? dataStatus, Timestamp timestamp,
                                                                       ICollection<int> tagid,
                                                                       int? recommendUser, int? brandId = null)
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

            return filter;
        }


        //private static Expression<Func<ProductEntity, bool>> GetFilter(DataStatus dataStatus, Timestamp timestamp, List<int> tagid,
        //                                                               int? recommendUser, int? brandId = null)
        //{
        //    Expression<Func<ProductEntity, bool>> filter = null;

        //    if (tagid == null || tagid.Count == 0)
        //    {
        //        if (recommendUser == null)
        //        {
        //            if (brandId == null)
        //            {
        //                switch (timestamp.TsType)
        //                {
        //                    case TimestampType.New:
        //                        filter = v => v.Status == (int)dataStatus && v.UpdatedDate > timestamp.Ts;
        //                        break;
        //                    case TimestampType.Old:
        //                    default:
        //                        filter = v => v.Status == (int)dataStatus && v.UpdatedDate <= timestamp.Ts;
        //                        break;
        //                }
        //            }
        //            else
        //            {
        //                switch (timestamp.TsType)
        //                {
        //                    case TimestampType.New:
        //                        filter = v => v.Status == (int)dataStatus && v.UpdatedDate > timestamp.Ts && v.Brand_Id == brandId.Value;
        //                        break;
        //                    case TimestampType.Old:
        //                    default:
        //                        filter = v => v.Status == (int)dataStatus && v.UpdatedDate <= timestamp.Ts && v.Brand_Id == brandId.Value;
        //                        break;
        //                }
        //            }
        //        }
        //        else
        //        {
        //            if (brandId == null)
        //            {
        //                switch (timestamp.TsType)
        //                {
        //                    case TimestampType.New:
        //                        filter = v => v.Status == (int)dataStatus && v.UpdatedDate > timestamp.Ts && v.RecommendUser == recommendUser.Value;
        //                        break;
        //                    case TimestampType.Old:
        //                    default:
        //                        filter = v => v.Status == (int)dataStatus && v.UpdatedDate <= timestamp.Ts && v.RecommendUser == recommendUser.Value;
        //                        break;
        //                }
        //            }
        //            else
        //            {
        //                switch (timestamp.TsType)
        //                {
        //                    case TimestampType.New:
        //                        filter = v => v.Status == (int)dataStatus && v.UpdatedDate > timestamp.Ts && v.RecommendUser == recommendUser.Value && v.Brand_Id == brandId.Value;
        //                        break;
        //                    case TimestampType.Old:
        //                    default:
        //                        filter = v => v.Status == (int)dataStatus && v.UpdatedDate <= timestamp.Ts && v.RecommendUser == recommendUser.Value && v.Brand_Id == brandId.Value;
        //                        break;
        //                }
        //            }
        //        }
        //    }
        //    else
        //    {
        //        if (recommendUser == null)
        //        {
        //            if (brandId == null)
        //            {
        //                switch (timestamp.TsType)
        //                {
        //                    case TimestampType.New:
        //                        filter = v => v.Status == (int)dataStatus && v.UpdatedDate > timestamp.Ts && tagid.Any(s => s == v.Tag_Id);
        //                        break;
        //                    case TimestampType.Old:
        //                    default:
        //                        filter = v => v.Status == (int)dataStatus && v.UpdatedDate <= timestamp.Ts && tagid.Any(s => s == v.Tag_Id);
        //                        break;
        //                }
        //            }
        //            else
        //            {
        //                switch (timestamp.TsType)
        //                {
        //                    case TimestampType.New:
        //                        filter = v => v.Status == (int)dataStatus && v.UpdatedDate > timestamp.Ts && tagid.Any(s => s == v.Tag_Id) && v.Brand_Id == brandId.Value;
        //                        break;
        //                    case TimestampType.Old:
        //                    default:
        //                        filter = v => v.Status == (int)dataStatus && v.UpdatedDate <= timestamp.Ts && tagid.Any(s => s == v.Tag_Id) && v.Brand_Id == brandId.Value;
        //                        break;
        //                }
        //            }

        //        }
        //        else
        //        {
        //            if (brandId == null)
        //            {
        //                switch (timestamp.TsType)
        //                {
        //                    case TimestampType.New:
        //                        filter = v => v.Status == (int)dataStatus && v.UpdatedDate > timestamp.Ts && tagid.Any(s => s == v.Tag_Id) && v.RecommendUser == recommendUser.Value;
        //                        break;
        //                    case TimestampType.Old:
        //                    default:
        //                        filter = v => v.Status == (int)dataStatus && v.UpdatedDate <= timestamp.Ts && tagid.Any(s => s == v.Tag_Id) && v.RecommendUser == recommendUser.Value;
        //                        break;
        //                }
        //            }
        //            else
        //            {
        //                switch (timestamp.TsType)
        //                {
        //                    case TimestampType.New:
        //                        filter = v => v.Status == (int)dataStatus && v.UpdatedDate > timestamp.Ts && tagid.Any(s => s == v.Tag_Id) && v.RecommendUser == recommendUser.Value && v.Brand_Id == brandId.Value;
        //                        break;
        //                    case TimestampType.Old:
        //                    default:
        //                        filter = v => v.Status == (int)dataStatus && v.UpdatedDate <= timestamp.Ts && tagid.Any(s => s == v.Tag_Id) && v.RecommendUser == recommendUser.Value && v.Brand_Id == brandId.Value;
        //                        break;
        //                }
        //            }
        //        }
        //    }

        //    return filter;
        //}

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
                    order = v => v.OrderByDescending(s => s.SortOrder).ThenByDescending(s=>s.CreatedDate);
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
            return base.Get(v => ids.Any(s => s == v.Id)).ToList();
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

        public override ProductEntity GetItem(int key)
        {
            return base.Find(key);
        }
    }
}
