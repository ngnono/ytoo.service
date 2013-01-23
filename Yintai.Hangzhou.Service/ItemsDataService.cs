using System;
using System.Linq;
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

        public ExecuteResult<ItemsCollectionResponse> GetProductList(GetItemsListRequest request)
        {
            var pager = new PagerRequest(request.PagerRequest.PageIndex, request.PagerRequest.PageSize, request.PagerRequest.PageSize * 2);

            int total1, total2;
            var productList = _productRepository.GetPagedList(pager, out total1, ProductSortOrder.CreatedDateDesc,
                                                                   request.Timestamp, null, request.UserModel.Id, null);

            var promotionList = _promotionRepository.GetPagedList(pager, out total2, PromotionSortOrder.CreatedDateDesc, null, null, null, request.UserModel.Id);

            var d = MappingManager.ItemsInfoResponseMapping(productList, promotionList);

            var data = d.OrderByDescending(v => v.CreatedDate).Take(request.PagerRequest.PageSize).ToList();

            var reponse = new ItemsCollectionResponse(request.PagerRequest, total1 + total2) { Items = data };

            var result = new ExecuteResult<ItemsCollectionResponse>(reponse);

            return result;
        }
    }
}
