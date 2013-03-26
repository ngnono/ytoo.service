using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Yintai.Architecture.Common.Caching;
using Yintai.Architecture.Common.Models;
using Yintai.Hangzhou.Contract.DTO.Request.ProductComplex;
using Yintai.Hangzhou.Contract.DTO.Response;
using Yintai.Hangzhou.Contract.ProductComplex;
using Yintai.Hangzhou.Model.Enums;
using Yintai.Hangzhou.Model.Filters;
using Yintai.Hangzhou.Repository.Contract;
using Yintai.Hangzhou.Service.Manager;

namespace Yintai.Hangzhou.Service
{
    public class ItemsDataService : BaseService, IItemsDataService
    {
        private readonly IProductRepository _productRepository;
        private readonly IPromotionRepository _promotionRepository;

        public ItemsDataService(IProductRepository productRepository, IPromotionRepository promotionRepository)
        {
            _productRepository = productRepository;
            _promotionRepository = promotionRepository;
        }

        private ItemsCollectionResponse Get(float version, PagerRequest pagerRequest, Timestamp timestamp, int userid)
        {
            var pager = new PagerRequest(pagerRequest.PageIndex, pagerRequest.PageSize, pagerRequest.PageSize * 2);

            int total1, total2;
            IEnumerable<ItemsInfoResponse> infolist;

            if (version >= 2.1)
            {
                var productList = _productRepository.Get(pager, out total1, ProductSortOrder.CreatedDateDesc, new ProductFilter
                    {
                        Timestamp = timestamp,
                        RecommendUser = userid
                    });

                var promotionList = _promotionRepository.Get(pager, out total2, PromotionSortOrder.CreatedDateDesc, new PromotionFilter
                    {
                        RecommendUser = userid
                    });

                infolist = MappingManager.ItemsInfoResponseMapping(productList, promotionList, null, false);
            }
            else
            {
                var productList = _productRepository.GetPagedList(pager, out total1, ProductSortOrder.CreatedDateDesc,
                                                          timestamp, null, userid, null);

                var promotionList = _promotionRepository.GetPagedList(pager, out total2, PromotionSortOrder.CreatedDateDesc, null, null, null, userid);

                infolist = MappingManager.ItemsInfoResponseMapping(productList, promotionList);
            }

            var data = infolist.OrderByDescending(v => v.CreatedDate).Take(pagerRequest.PageSize).ToList();

            var reponse = new ItemsCollectionResponse(pagerRequest, total1 + total2) { Items = data };

            return reponse;
        }

        public ExecuteResult<ItemsCollectionResponse> GetProductList(GetItemsListRequest request)
        {
            var innerkey = String.Format("{0}_{1}_{2}_{3}", request.PagerRequest.ToString(), request.Timestamp.ToString(), request.UserModel.Id.ToString(CultureInfo.InvariantCulture), request.Version >= 2.1);
            string cacheKey;
            var s = CacheKeyManager.StoreInfoKey(out cacheKey, innerkey);
            var r = CachingHelper.Get(
                delegate(out ItemsCollectionResponse data)
                {
                    var objData = CachingHelper.Get(cacheKey);
                    data = (objData == null) ? null : (ItemsCollectionResponse)objData;

                    return objData != null;
                },
                () => Get(request.Version, request.PagerRequest, request.Timestamp, request.UserModel.Id),
                data =>
                CachingHelper.Insert(cacheKey, data, s));

            var result = new ExecuteResult<ItemsCollectionResponse>
            {
                Data = r
            };

            return result;
        }
    }
}
