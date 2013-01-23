using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Yintai.Architecture.Common.Models;
using Yintai.Hangzhou.Data.Models;
using Yintai.Hangzhou.Model.Enums;
using Yintai.Hangzhou.Repository.Contract;

namespace Yintai.Hangzhou.Repository.Impl
{
    public class PointRepository : RepositoryBase<PointHistoryEntity, int>, IPointRepository
    {
        #region methods

        /// <summary>
        /// 过滤
        /// </summary>
        /// <returns></returns>
        private static Expression<Func<PointHistoryEntity, bool>> GetFilter(int? userId)
        {
            Expression<Func<PointHistoryEntity, bool>> filter = null;

            if (userId == null)
            {
                return filter;
            }
            else
            {
                filter = v => v.User_Id == userId.Value;
            }

            return filter;
        }

        /// <summary>
        /// 排序
        /// </summary>
        /// <param name="sort"></param>
        /// <returns></returns>
        private static Func<IQueryable<PointHistoryEntity>, IOrderedQueryable<PointHistoryEntity>> GetOrder(PointSortOrder sort)
        {
            Func<IQueryable<PointHistoryEntity>, IOrderedQueryable<PointHistoryEntity>> order = null;
            switch (sort)
            {
                case PointSortOrder.Default:
                default:
                    order = v => v.OrderByDescending(s => s.CreatedDate);
                    break;
            }

            return order;
        }

        #endregion

        public override PointHistoryEntity GetItem(int key)
        {
            return base.Find(key);
        }

        public List<PointHistoryEntity> GetPagedList(PagerRequest request, out int totalCount, int userId, PointSortOrder sortOrder)
        {
            return
                base.Get(GetFilter(userId), out totalCount, request.PageIndex, request.PageSize, GetOrder(sortOrder))
                    .ToList();
        }

        public List<PointHistoryEntity> GetPagedList(PagerRequest request, out int totalCount, PointSortOrder sortOrder)
        {
            return
                base.Get(GetFilter(null), out totalCount, request.PageIndex, request.PageSize, GetOrder(sortOrder))
                    .ToList();
        }
    }
}
