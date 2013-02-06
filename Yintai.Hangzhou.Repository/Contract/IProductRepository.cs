using System.Collections.Generic;
using Yintai.Architecture.Common.Models;
using Yintai.Hangzhou.Data.Models;
using Yintai.Hangzhou.Model.Enums;
<<<<<<< HEAD
=======
<<<<<<< HEAD
using Yintai.Hangzhou.Model.Filters;
=======
>>>>>>> edit create coupon logic
>>>>>>> 3054968bf9d0c0d79ede151efa7ff13f4379c369

namespace Yintai.Hangzhou.Repository.Contract
{
    public interface IProductRepository : IRepository<ProductEntity, int>
    {
        /// <summary>
        /// 分页
        /// </summary>
        /// <param name="pagerRequest">page</param>
        /// <param name="totalCount">记录数</param>
        /// <param name="sortOrder">排序方式</param>
        /// <param name="timestamp">时间戳 </param>
        /// <param name="productName">产品名称</param>
        /// <param name="recommendUser">推荐人</param>
        /// <param name="tagids">Tag ids</param>
        /// <param name="brandId">品牌Id</param>
        /// <returns></returns>
        List<ProductEntity> GetPagedListForSearch(PagerRequest pagerRequest, out int totalCount, ProductSortOrder sortOrder, Timestamp timestamp, string productName, int? recommendUser, List<int> tagids, int? brandId);

        /// <summary>
        /// 分页
        /// </summary>
        /// <param name="pagerRequest">page</param>
        /// <param name="totalCount">记录数</param>
        /// <param name="sortOrder">排序方式</param>
        /// <param name="timestamp">时间戳 </param>
        /// <param name="tagId">tag id</param>
        /// <param name="recommendUser">推荐人</param>
        /// <param name="brandId">品牌</param>
        /// <returns></returns>
        List<ProductEntity> GetPagedList(PagerRequest pagerRequest, out int totalCount, ProductSortOrder sortOrder, Timestamp timestamp, int? tagId, int? recommendUser, int? brandId);

        /// <summary>
        /// list
        /// </summary>
        /// <param name="ids">product ids</param>
        /// <returns></returns>
        List<ProductEntity> GetList(List<int> ids);

        /// <summary>
        /// SetSortOrder
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="sortOrder"></param>
        /// <param name="updateUser"></param>
        /// <returns></returns>
<<<<<<< HEAD
        ProductEntity SetSortOrder(ProductEntity entity,int sortOrder,int updateUser);
=======
<<<<<<< HEAD
        ProductEntity SetSortOrder(ProductEntity entity, int sortOrder, int updateUser);

        /// <summary>
        /// 分页
        /// </summary>
        /// <param name="pagerRequest">page</param>
        /// <param name="totalCount">记录数</param>
        /// <param name="sortOrder">排序方式</param>
        /// <param name="productFilter">过滤选项</param>
        /// <returns></returns>
        List<ProductEntity> GetPagedList(PagerRequest pagerRequest, out int totalCount, ProductSortOrder sortOrder, ProductFilter productFilter);
=======
        ProductEntity SetSortOrder(ProductEntity entity,int sortOrder,int updateUser);
>>>>>>> edit create coupon logic
>>>>>>> 3054968bf9d0c0d79ede151efa7ff13f4379c369
    }
}
