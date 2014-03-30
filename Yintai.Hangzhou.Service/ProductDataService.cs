using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using Yintai.Architecture.Common.Caching;
using Yintai.Architecture.Common.Models;
using Yintai.Architecture.Common.Web;
using Yintai.Hangzhou.Contract.Coupon;
using Yintai.Hangzhou.Contract.DTO.Request.Coupon;
using Yintai.Hangzhou.Contract.DTO.Request.Product;
using Yintai.Hangzhou.Contract.DTO.Request.Promotion;
using Yintai.Hangzhou.Contract.DTO.Response.Coupon;
using Yintai.Hangzhou.Contract.DTO.Response.Product;
using Yintai.Hangzhou.Contract.Product;
using Yintai.Hangzhou.Contract.Promotion;
using Yintai.Hangzhou.Data.Models;
using Yintai.Hangzhou.Model;
using Yintai.Hangzhou.Model.Enums;
using Yintai.Hangzhou.Model.Filters;
using Yintai.Hangzhou.Repository.Contract;
using Yintai.Hangzhou.Service.Contract;
using Yintai.Hangzhou.Service.Manager;

namespace Yintai.Hangzhou.Service
{
    public class ProductDataService : BaseService, IProductDataService
    {
        private readonly IProductRepository _productRepository;
        private readonly IShareService _shareService;
        private readonly IFavoriteService _favoriteService;
        private readonly ICouponDataService _couponDataService;
        private readonly IResourceService _resourceService;
        private readonly ICouponService _couponService;
        private readonly IPromotionService _promotionService;
        private readonly IPromotionDataService _promotionDataService;
        private IPromotionRepository _promotionRepo;
        private IProductPropertyRepository _productpropertyRepo;
        private IProductPropertyValueRepository _productpropertyvalueRepo;
        private IProductCode2StoreCodeRepository _productcodemapRepo;


        public ProductDataService(IPromotionDataService promotionDataService, IPromotionService promotionService,
            ICouponService couponService, IResourceService resourceService, IProductRepository productRepository,
            IShareService shareService, IFavoriteService favoriteService, ICouponDataService couponDataService,
            IPromotionRepository promotionRepo,
            IProductPropertyRepository productpropertyRepo,
            IProductPropertyValueRepository productpropertyvalueRepo,
            IProductCode2StoreCodeRepository productcodemapRepo)
        {
            _promotionDataService = promotionDataService;
            _productRepository = productRepository;
            _shareService = shareService;
            _favoriteService = favoriteService;
            _couponDataService = couponDataService;
            _resourceService = resourceService;
            _couponService = couponService;
            _promotionService = promotionService;
            _promotionRepo = promotionRepo;
            _productpropertyRepo = productpropertyRepo;
            _productpropertyvalueRepo = productpropertyvalueRepo;
            _productcodemapRepo = productcodemapRepo;
        }

        private ProductInfoResponse IsR(ProductInfoResponse response, UserModel currentAuthUser, int productId)
        {
            if (response == null || currentAuthUser == null)
            {
                return response;
            }

            //�Ƿ��ղ�
            var favoriteEntity = _favoriteService.Get(currentAuthUser.Id, productId, SourceType.Product);
            if (favoriteEntity != null)
            {
                response.CurrentUserIsFavorited = true;
            }
            //�Ƿ��ȡ���Ż���
            var list = _couponService.Get(currentAuthUser.Id, productId, SourceType.Product);
            if (list != null && list.Count > 0)
            {
                response.CurrentUserIsReceived = true;
            }

            return response;
        }

        /// <summary>
        /// ��ȡ��Ʒ�б�
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public ExecuteResult<ProductCollectionResponse> GetProductList(GetProductListRequest request)
        {
            if (request == null)
            {
                return new ExecuteResult<ProductCollectionResponse>(null) { StatusCode = StatusCode.ClientError, Message = "��������" };
            }

            int? ruserId;
            List<int> tagIds = null;
            if (request.TagId != null)
            {
                tagIds = new List<int> { request.TagId.Value };
            }

            if (request.UserModel == null)
            {
                ruserId = null;
            }
            else
            {
                ruserId = request.UserModel.Id;
            }

            var filter = new ProductFilter
            {
                BrandId = request.BrandId,
                DataStatus = DataStatus.Normal,
                ProductName = null,
                RecommendUser = ruserId,
                TagIds = tagIds,
                Timestamp = request.Timestamp,
                TopicId = request.TopicId,
                PromotionId = request.PromotionId
            };

            ProductCollectionResponse r;
            int totalCount;

            if (request.Version > 2.09)
            {
                var produtEntities = _productRepository.Get(request.PagerRequest, out totalCount,
               request.ProductSortOrder, filter);

                var response = new ProductCollectionResponse(request.PagerRequest, totalCount)
                {
                    Products = MappingManager.ProductInfoResponseMapping(produtEntities).ToList()
                };

                r = response;
            }
            else
            {
                var p = _productRepository.GetPagedList(request.PagerRequest, out totalCount,
                                                        request.ProductSortOrder, filter);
                var response = new ProductCollectionResponse(request.PagerRequest, totalCount)
                {
                    Products = MappingManager.ProductInfoResponseMapping(p).ToList()
                };

                r = response;
            }



            var result = new ExecuteResult<ProductCollectionResponse> { Data = r };

            return result;
        }

        public ExecuteResult<ProductCollectionResponse> RefreshProduct(GetProductRefreshRequest request)
        {
            if (request == null)
            {
                return new ExecuteResult<ProductCollectionResponse>(null) { StatusCode = StatusCode.ClientError, Message = "��������" };
            }

            List<int> tagIds = null;
            if (request.TagId != null)
            {
                tagIds = new List<int> { request.TagId.Value };
            }

            var filter = new ProductFilter
            {
                BrandId = request.BrandId,
                DataStatus = DataStatus.Normal,
                ProductName = null,
                RecommendUser = null,
                TagIds = tagIds,
                Timestamp = request.Timestamp,
                TopicId = request.TopicId,
                PromotionId = request.PromotionId
            };


            int totalCount;
            var produtEntities = _productRepository.Get(request.PagerRequest, out totalCount,
                request.ProductSortOrder, filter);

            var r = new ProductCollectionResponse(request.PagerRequest, totalCount)
            {
                Products = MappingManager.ProductInfoResponseMapping(produtEntities).ToList()
            };


            var result = new ExecuteResult<ProductCollectionResponse> { Data = r };

            return result;
        }

        public ExecuteResult<ProductInfoResponse> GetProductInfo(GetProductInfoRequest request)
        {
            if (request == null)
            {
                return new ExecuteResult<ProductInfoResponse>(null) { StatusCode = StatusCode.ClientError, Message = "��������" };
            }


            var entity = _productRepository.GetItem(request.ProductId);

            var r = MappingManager.ProductInfoResponseMapping(entity);


            if (r != null && request.CurrentAuthUser != null)
            {
                r = IsR(r, request.CurrentAuthUser, r.Id);
            }

            return new ExecuteResult<ProductInfoResponse>(r);
        }

        public ExecuteResult<ProductInfoResponse> CreateProduct(CreateProductRequest request)
        {
            if (request == null)
            {
                return new ExecuteResult<ProductInfoResponse>(null) { StatusCode = StatusCode.ClientError, Message = "��������" };
            }
            /*
            if (request.Is4Sale.HasValue && request.Is4Sale.Value == true && string.IsNullOrEmpty(request.UPCCode))
            {
                return new ExecuteResult<ProductInfoResponse>(null) { StatusCode = StatusCode.ClientError, Message = "��������Ʒ��Ҫ������Ʒ����" };
            }
            */
            if (request.RecommendUser == 0)
            {
                request.RecommendUser = request.AuthUid;

            }

            request.RecommendSourceId = request.RecommendUser;
            request.RSourceType = request.AuthUser.Level == UserLevel.Daren
                                              ? RecommendSourceType.Daren
                                              : RecommendSourceType.StoreManager;

            //�жϵ�ǰ�û��Ƿ��� ����Ա���� level �Ǵ��ˣ�
            var inEntity = MappingManager.ProductEntityMapping(request);
            inEntity.Is4Sale = request.Is4Sale;

            if (request.Files != null && request.Files.Count > 0)
            {
                foreach (string f in request.Files)
                {
                    if (request.Files[f].HasFile())
                    {
                        inEntity.IsHasImage = true;

                        break;
                    }
                }
            }
            if (!inEntity.IsHasImage)
                return new ExecuteResult<ProductInfoResponse>(null)
                {
                    StatusCode = StatusCode.ClientError
                    ,
                    Message = "û��ͼƬ��Ϣ"
                };
            inEntity.SortOrder = 0;
            ProductEntity entity = null;
            using (var ts = new TransactionScope())
            {
                entity = _productRepository.Insert(inEntity);
               
                //insert properties if any
                if (request.PropertyModel != null)
                {
                    foreach (var property in request.PropertyModel)
                    {
                        if (string.IsNullOrEmpty(property.PropertyName))
                            continue;
                        var productPropertyEntity = _productpropertyRepo.Insert(new ProductPropertyEntity()
                        {
                            ProductId = entity.Id,
                            PropertyDesc = property.PropertyName,
                            SortOrder = 0,
                            Status = (int)DataStatus.Normal,
                            UpdateDate = DateTime.Now,
                            UpdateUser = request.RecommendUser
                        });
                        foreach (var value in property.Values)
                        {
                            if (string.IsNullOrEmpty(value))
                                continue;
                            _productpropertyvalueRepo.Insert(new ProductPropertyValueEntity()
                            {
                                CreateDate = DateTime.Now,
                                PropertyId = productPropertyEntity.Id,
                                Status = (int)DataStatus.Normal,
                                UpdateDate = DateTime.Now,
                                ValueDesc = value
                            });
                        }
                    }
                }
                //���� ͼƬ
                //�����ļ��ϴ�
                if (request.Files != null && request.Files.Count > 0)
                {
                    List<ResourceEntity> listImage = null;
                    try
                    {
                        listImage = _resourceService.Save(request.Files, request.AuthUid, 0, entity.Id, SourceType.Product);

                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex);

                        //DEL �ϴ��Ĳ�Ʒ

                        _productRepository.Delete(entity);

                        throw;
                    }

                    if (listImage == null || listImage.Count == 0)
                    {
                        //set ishasimage
                        if (entity.IsHasImage)
                        {
                            _productRepository.SetIsHasImage(entity.Id, false, DataStatus.Default, request.AuthUid,
                                        "set ishasimage false");
                        }
                    }
                }
                ts.Complete();
            }
            return new ExecuteResult<ProductInfoResponse>(MappingManager.ProductInfoResponseMapping(entity));
        }

        public ExecuteResult<ProductInfoResponse> UpdateProduct(UpdateProductRequest request)
        {
            if (request == null)
            {
                return new ExecuteResult<ProductInfoResponse>(null) { StatusCode = StatusCode.ClientError, Message = "��������" };
            }

            var entity = _productRepository.GetItem(request.ProductId);

            if (entity == null)
            {
                return new ExecuteResult<ProductInfoResponse>(null) { StatusCode = StatusCode.ClientError, Message = "��������,û���ҵ�ָ��product" };
            }

            if (request.RecommendUser == 0)
            {
                request.RecommendUser = request.AuthUid;
            }

            var source = MappingManager.ProductEntityMapping(request);
            source.CreatedDate = entity.CreatedDate;
            source.CreatedUser = entity.CreatedUser;
            source.Status = entity.Status;
            source.FavoriteCount = entity.FavoriteCount;
            source.InvolvedCount = entity.InvolvedCount;
            source.ShareCount = entity.ShareCount;

            MappingManager.ProductEntityMapping(source, entity);

            _productRepository.Update(entity);

            return new ExecuteResult<ProductInfoResponse>(MappingManager.ProductInfoResponseMapping(entity));
        }

        public ExecuteResult<ProductInfoResponse> DestroyProduct(DestroyProductRequest request)
        {
            if (request == null)
            {
                return new ExecuteResult<ProductInfoResponse>(null) { StatusCode = StatusCode.ClientError, Message = "��������" };
            }

            var entity = _productRepository.GetItem(request.ProductId);

            if (entity == null)
            {
                return new ExecuteResult<ProductInfoResponse>(null) { StatusCode = StatusCode.ClientError, Message = "��������,û���ҵ�ָ��product" };
            }

            entity.UpdatedDate = DateTime.Now;
            entity.UpdatedUser = request.AuthUid;
            entity.Status = (int)DataStatus.Deleted;

            _productRepository.Update(entity);

            return new ExecuteResult<ProductInfoResponse>(MappingManager.ProductInfoResponseMapping(entity));
        }

        public ExecuteResult<ProductInfoResponse> CreateResourceProduct(CreateResourceProductRequest request)
        {
            if (request == null)
            {
                return new ExecuteResult<ProductInfoResponse>(null) { StatusCode = StatusCode.ClientError, Message = "��������" };
            }

            var entity = _productRepository.GetItem(request.ProductId);

            if (entity == null)
            {
                return new ExecuteResult<ProductInfoResponse>(null) { StatusCode = StatusCode.ClientError, Message = "��������,û���ҵ�ָ��product" };
            }

            //Add
            if (request.Files != null && request.Files.Count > 0)
            {
                _resourceService.Save(request.Files, request.AuthUid, request.DefaultNum, entity.Id, SourceType.Product);
            }

            entity.UpdatedDate = DateTime.Now;
            entity.UpdatedUser = request.AuthUid;
            _productRepository.Update(entity);

            return new ExecuteResult<ProductInfoResponse>(MappingManager.ProductInfoResponseMapping(entity));
        }

        public ExecuteResult<ProductInfoResponse> DestroyResourceProduct(DestroyResourceProductRequest request)
        {
            if (request == null)
            {
                return new ExecuteResult<ProductInfoResponse>(null) { StatusCode = StatusCode.ClientError, Message = "��������" };
            }

            var entity = _productRepository.GetItem(request.ProductId);

            if (entity == null)
            {
                return new ExecuteResult<ProductInfoResponse>(null) { StatusCode = StatusCode.ClientError, Message = "��������,û���ҵ�ָ��product" };
            }

            var resources = _resourceService.Get(entity.Id, SourceType.Product);

            if (resources == null || resources.Count == 0)
            {
                return new ExecuteResult<ProductInfoResponse>(null) { StatusCode = StatusCode.ClientError, Message = "��������,û���ҵ�product����Դ" };
            }

            var resouce = resources.SingleOrDefault(v => v.Id == request.ResourceId);
            if (resouce == null)
            {
                return new ExecuteResult<ProductInfoResponse>(null) { StatusCode = StatusCode.ClientError, Message = "��������,û���ҵ�product��ָ����Դ" };
            }

            _resourceService.Del(resouce.Id);

            entity.UpdatedDate = DateTime.Now;
            entity.UpdatedUser = request.AuthUid;

            _productRepository.Update(entity);

            return new ExecuteResult<ProductInfoResponse>(MappingManager.ProductInfoResponseMapping(entity));
        }

        public ExecuteResult<ProductInfoResponse> CreateShare(CreateShareProductRequest request)
        {
            if (request == null)
            {
                return new ExecuteResult<ProductInfoResponse>(null) { StatusCode = StatusCode.ClientError, Message = "��������" };
            }
            var product = _productRepository.GetItem(request.ProductId);
            if (product == null)
            {
                return new ExecuteResult<ProductInfoResponse>(null) { StatusCode = StatusCode.ClientError, Message = "��������" };
            }

            _shareService.Create(new ShareHistoryEntity
            {
                CreatedDate = DateTime.Now,
                CreatedUser = request.AuthUid,
                Description = request.Description,
                Name = request.Name,
                ShareTo = (int)request.OsiteType,
                SourceId = request.ProductId,
                SourceType = (int)SourceType.Product,
                Stauts = 1,
                User_Id = request.AuthUid,
                UpdatedDate = DateTime.Now,
                UpdatedUser = request.AuthUid
            });

            //+1
            product = _productRepository.SetCount(ProductCountType.ShareCount, product.Id, 1);

            return new ExecuteResult<ProductInfoResponse>(MappingManager.ProductInfoResponseMapping(product));
        }

        public ExecuteResult<ProductInfoResponse> CreateFavor(CreateFavorProductRequest request)
        {
            if (request == null)
            {
                return new ExecuteResult<ProductInfoResponse>(null) { StatusCode = StatusCode.ClientError, Message = "��������" };
            }
            var product = _productRepository.GetItem(request.ProductId);
            if (product == null)
            {
                return new ExecuteResult<ProductInfoResponse>(null) { StatusCode = StatusCode.ClientError, Message = "��������" };
            }

            var favorentity = _favoriteService.Get(request.AuthUid, product.Id, SourceType.Product);
            if (favorentity != null)
            {
                return new ExecuteResult<ProductInfoResponse>(null) { StatusCode = StatusCode.ClientError, Message = "���Ѿ���ӹ��ղ���" };
            }

            _favoriteService.Create(new FavoriteEntity
            {
                CreatedDate = DateTime.Now,
                CreatedUser = request.AuthUid,
                Description = String.Empty,
                FavoriteSourceId = product.Id,
                FavoriteSourceType = (int)SourceType.Product,
                Status = 1,
                User_Id = request.AuthUid,
                Store_Id = product.Store_Id
            });

            //+1
            product = _productRepository.SetCount(ProductCountType.FavoriteCount, product.Id, 1);
            //TODO:û�и��µ�ǰ��isfavorite
            var response = MappingManager.ProductInfoResponseMapping(product);

            if (request.AuthUser != null)
            {
                response = IsR(response, request.AuthUser, product.Id);
            }

            return new ExecuteResult<ProductInfoResponse>(response);
        }

        public ExecuteResult<ProductInfoResponse> DestroyFavor(DestroyFavorProductRequest request)
        {
            if (request == null)
            {
                return new ExecuteResult<ProductInfoResponse>(null) { StatusCode = StatusCode.ClientError, Message = "��������" };
            }
            var product = _productRepository.GetItem(request.ProductId);
            if (product == null)
            {
                return new ExecuteResult<ProductInfoResponse>(null) { StatusCode = StatusCode.ClientError, Message = "��������" };
            }

            var favorentity = _favoriteService.Get(request.AuthUid, product.Id, SourceType.Product);

            if (favorentity == null)
            {
                return new ExecuteResult<ProductInfoResponse>(null) { StatusCode = StatusCode.ClientError, Message = "�ղز�����" };
            }

            //del shoucang
            _favoriteService.Del(favorentity);
            //del product share count
            product = _productRepository.SetCount(ProductCountType.FavoriteCount, product.Id, -1);

            var response = MappingManager.ProductInfoResponseMapping(product);

            if (request.AuthUser != null)
            {
                response = IsR(response, request.AuthUser, product.Id);
            }

            return new ExecuteResult<ProductInfoResponse>(response);
        }

        public ExecuteResult<ProductInfoResponse> CreateCoupon(CreateCouponProductRequest request)
        {
            if (request == null)
            {
                return new ExecuteResult<ProductInfoResponse>(null) { StatusCode = StatusCode.ClientError, Message = "��������" };
            }
            var product = _productRepository.GetItem(request.ProductId);
            if (product == null)
            {
                return new ExecuteResult<ProductInfoResponse>(null) { StatusCode = StatusCode.ClientError, Message = "��������" };
            }

            //�ж������v1.0�汾 �����������Ż�ȯ��

            ExecuteResult<CouponCodeResponse> coupon = null;
            if (request.Client_Version != "1.0")
            {
                //��ȡ��Ʒ�����Ļ
                var promotionEntity = _promotionService.GetFristNormalForProductId(product.Id);
                if (request.PromotionId == null || request.PromotionId.Value < 1)
                {

                    if (promotionEntity == null)
                    {
                        return new ExecuteResult<ProductInfoResponse>(null) { StatusCode = StatusCode.ClientError, Message = "��ǰ��Ʒû�вμӻ" };
                    }

                    request.PromotionId = promotionEntity.Id;
                }
                else
                {
                    //�ж�
                    if (!_promotionService.Exists(request.PromotionId ?? 0, request.ProductId))
                    {
                        return new ExecuteResult<ProductInfoResponse>(null) { StatusCode = StatusCode.ClientError, Message = "��ǰ��Ʒû�вμӸû" };
                    }
                }
                using (var ts = new TransactionScope())
                {
                    coupon = _couponDataService.CreateCoupon(new CouponCouponRequest
                    {
                        AuthUid = request.AuthUser.Id,
                        PromotionId = promotionEntity.Id,
                        ProductId = request.ProductId,
                        SourceType = (int)SourceType.Promotion,
                        Token = request.Token,
                        AuthUser = request.AuthUser,
                        Method = request.Method,
                        Client_Version = request.Client_Version
                    });

                    if (!coupon.IsSuccess)
                    {
                        return new ExecuteResult<ProductInfoResponse>(null)
                        {
                            Message = coupon.Message,
                            StatusCode = coupon.StatusCode
                        };
                    }

                    promotionEntity = _promotionRepo.SetCount(PromotionCountType.InvolvedCount, promotionEntity.Id, 1);

                    product = _productRepository.SetCount(ProductCountType.InvolvedCount, product.Id, 1);

                    var response = MappingManager.ProductInfoResponseMapping(product);
                    response.CouponCodeResponse = coupon.Data;

                    if (request.AuthUser != null)
                    {
                        response = IsR(response, request.AuthUser, product.Id);
                    }
                    ts.Complete();
                    return new ExecuteResult<ProductInfoResponse>(response);


                }
            }
            else
            {
                return new ExecuteResult<ProductInfoResponse>(null) { StatusCode = StatusCode.ClientError, Message = "��������" };
            }

        }

        public ExecuteResult<ProductCollectionResponse> Search(SearchProductRequest request)
        {
            return new ExecuteResult<ProductCollectionResponse>(null) { StatusCode = StatusCode.ClientError, Message = "��֧�ָ÷���" };
        }
    }
}