using System;
using System.Globalization;
using System.Linq;
using Yintai.Architecture.Common.Caching;
using Yintai.Architecture.Common.Models;
using Yintai.Hangzhou.Contract.DTO.Request.ProductComplex;
using Yintai.Hangzhou.Contract.DTO.Response;
using Yintai.Hangzhou.Contract.ProductComplex;
using Yintai.Hangzhou.Model.Enums;
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

        private ItemsCollectionResponse Get(PagerRequest pagerRequest, Timestamp timestamp, int userid)
        {
            var pager = new PagerRequest(pagerRequest.PageIndex, pagerRequest.PageSize, pagerRequest.PageSize * 2);

            int total1, total2;
            var productList = _productRepository.GetPagedList(pager, out total1, ProductSortOrder.CreatedDateDesc,
                                                                   timestamp, null, userid, null);

            var promotionList = _promotionRepository.GetPagedList(pager, out total2, PromotionSortOrder.CreatedDateDesc, null, null, null, userid);

            var d = MappingManager.ItemsInfoResponseMapping(productList, promotionList);

            var data = d.OrderByDescending(v => v.CreatedDate).Take(pagerRequest.PageSize).ToList();

            var reponse = new ItemsCollectionResponse(pagerRequest, total1 + total2) { Items = data };

            return reponse;
        }

        public ExecuteResult<ItemsCollectionResponse> GetProductList(GetItemsListRequest request)
        {
            var innerkey = String.Format("{0}_{1}_{2}", request.PagerRequest.ToString(), request.Timestamp.ToString(), request.UserModel.Id.ToString(CultureInfo.InvariantCulture));
            string cacheKey;
            var s = CacheKeyManager.StoreInfoKey(out cacheKey, innerkey);
            var r = CachingHelper.Get(
                delegate(out ItemsCollectionResponse data)
                {
                    var objData = CachingHelper.Get(cacheKey);
                    data = (objData == null) ? null : (ItemsCollectionResponse)objData;

                    return objData != null;
                },
                () => Get(request.PagerRequest, request.Timestamp, request.UserModel.Id),
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
