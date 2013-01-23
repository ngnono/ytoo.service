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
    public class FavoriteRepository : RepositoryBase<FavoriteEntity, int>, IFavoriteRepository
    {
        private static Expression<Func<FavoriteEntity, bool>> GetFiller(int userId, SourceType sourceType)
        {
            Expression<Func<FavoriteEntity, bool>> filter = null;
            switch (sourceType)
            {
                case SourceType.Product:
                    filter = v => v.User_Id == userId && v.FavoriteSourceType == (int)SourceType.Product && v.Status == 1;
                    break;
                case SourceType.Promotion:
                    filter = v => v.User_Id == userId && v.FavoriteSourceType == (int)SourceType.Promotion && v.Status == 1;
                    break;
                default:
                    filter = v => v.User_Id == userId && v.Status == 1;
                    break;
            }

            return filter;
        }

        private static Func<IQueryable<FavoriteEntity>, IOrderedQueryable<FavoriteEntity>> GetSort(FavoriteSortOrder sortOrder)
        {
            Func<IQueryable<FavoriteEntity>, IOrderedQueryable<FavoriteEntity>> orderBy = null;

            switch (sortOrder)
            {
                default:
                    orderBy = v => v.OrderByDescending(s => s.CreatedDate);
                    break;
            }

            return orderBy;
        }

        #region Overrides of RepositoryBase<FavoriteEntity,int>

        /// <summary>
        /// 查找key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public override FavoriteEntity GetItem(int key)
        {
            return base.Find(key);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="sourceType"></param>
        /// <param name="sourceid"></param>
        /// <returns></returns>
        public FavoriteEntity GetItem(int userid, SourceType sourceType, int sourceid)
        {
            return
                Get(
                    v =>
                    v.User_Id == userid && v.FavoriteSourceType == (int)sourceType && v.FavoriteSourceId == sourceid).
                    FirstOrDefault();
        }

        /// <summary>
        /// 分页
        /// </summary>
        /// <param name="pagerRequest"></param>
        /// <param name="totalCount"></param>
        /// <param name="sortOrder"> </param>
        /// <param name="sourceType"></param>
        /// <returns></returns>
        public List<FavoriteEntity> GetPagedList(int userid, PagerRequest pagerRequest, out int totalCount, FavoriteSortOrder sortOrder, SourceType sourceType)
        {
            return
                base.Get(GetFiller(userid, sourceType), out totalCount, pagerRequest.PageIndex, pagerRequest.PageSize,
                         GetSort(sortOrder)).ToList();
        }

        #endregion
    }
}
