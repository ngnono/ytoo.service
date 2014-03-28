using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Web.Mvc;
using Yintai.Hangzhou.Contract.DTO.Response;
using Yintai.Hangzhou.Data.Models;
using Yintai.Hangzhou.Model.Enums;
using Yintai.Hangzhou.Repository.Contract;
using Yintai.Hangzhou.Service.Contract;
using Yintai.Hangzhou.WebSupport.Mvc;
using com.intime.fashion.common.Extension;

namespace Yintai.Hangzhou.WebApiCore.Areas.Ims.Controllers
{
    public class ProductController : RestfulController
    {
        private IResourceService _resourceService;
        private IProductRepository _productRepo;
        private IProductPropertyRepository _productPropertyRepo;
        private IProductPropertyValueRepository _productPropertyValueRepo;
        private IInventoryRepository _inventoryRepo;
        private IResourceRepository _resourceRepo;
        public ProductController(IResourceService resourceService
            , IProductRepository productRepo
            , IProductPropertyRepository productPropertyRepo
            , IProductPropertyValueRepository productPropertyValueRepo
            , IInventoryRepository inventoryRepo
            , IResourceRepository resourceRepo)
        {
            _resourceService = resourceService;
            _productRepo = productRepo;
            _productPropertyRepo = productPropertyRepo;
            _productPropertyValueRepo = productPropertyValueRepo;
            _inventoryRepo = inventoryRepo;
            _resourceRepo = resourceRepo;
        }
        [RestfulRoleAuthorize(UserLevel.DaoGou)]
        public ActionResult Create(Yintai.Hangzhou.Contract.DTO.Request.IMSProductCreateRequest request, int authuid)
        {
            if (!ModelState.IsValid)
            {
                var error = ModelState.Values.Where(v => v.Errors.Count() > 0).First();
                return this.RenderError(r => r.Message = error.Errors.First().ErrorMessage);
            }
            var files = Request.Files;
            if (files.Count != 1)
                return this.RenderError(r => r.Message = "商品没有图片！");
            var categoryEntity = Context.Set<TagEntity>()
                                .Where(t => t.Id == request.Category_Id)
                                .GroupJoin(Context.Set<CategoryPropertyEntity>().Join(Context.Set<CategoryPropertyValueEntity>(), o => o.Id, i => i.PropertyId, (o, i) => new { CP = o, CPV = i })
                                    , o => o.Id, i => i.CP.CategoryId, (o, i) => new { C = o, CP = i }).FirstOrDefault()
                                    ;
            if (categoryEntity == null)
                return this.RenderError(r => r.Message = "分类不存在");
            if (categoryEntity.C.SizeType == (int)CategorySizeType.LimitSize
                && request.Size_Ids.Length < 1)
                return this.RenderError(r => r.Message = "分类尺码必选");
            if (categoryEntity.C.SizeType == (int)CategorySizeType.FreeInput
                && string.IsNullOrEmpty(request.Size_Str))
                return this.RenderError(r => r.Message = "分类尺码必选");

            var brandEntity = Context.Set<BrandEntity>().Find(request.Brand_Id);
            if (brandEntity == null)
                return this.RenderError(r => r.Message = "品牌不存在");

            var assocateEntity = Context.Set<IMS_AssociateEntity>().Where(ia => ia.UserId == authuid).First();
            using (var ts = new TransactionScope())
            {
                //step1: create product 
                var productName = string.Format("{0}-{1}-{2}", brandEntity.Name, categoryEntity.C.Name, request.Sku_Code);
                var productEntity = _productRepo.Insert(new ProductEntity()
                {
                    Brand_Id = request.Brand_Id,
                    CreatedDate = DateTime.Now,
                    CreatedUser = authuid,
                    Description = productName,
                    FavoriteCount = 0,
                    InvolvedCount = 0,
                    Is4Sale = true,
                    IsHasImage = true,
                    MoreDesc = string.Empty,
                    Name = productName,
                    Price = request.Price,
                    ProductType = (int)ProductType.FromSelf,
                    RecommendedReason = string.Empty,
                    RecommendUser = authuid,
                    SkuCode = request.Sku_Code,
                    SortOrder = 1,
                    Status = (int)DataStatus.Default,
                    Tag_Id = request.Category_Id,
                    UnitPrice = 999999,
                    UpdatedDate = DateTime.Now,
                    UpdatedUser = authuid,
                    Store_Id = assocateEntity.StoreId


                });
                //step2: create product color property
                var propertyEntity = _productPropertyRepo.Insert(new ProductPropertyEntity()
                {
                    IsColor = true,
                    IsSize = false,
                    ProductId = productEntity.Id,
                    PropertyDesc = "颜色",
                    SortOrder = 1,
                    Status = (int)DataStatus.Normal,
                    UpdateDate = DateTime.Now,
                    UpdateUser = authuid
                });
                var propertyvalueEntity = _productPropertyValueRepo.Insert(new ProductPropertyValueEntity()
                {
                    CreateDate = DateTime.Now,
                    PropertyId = propertyEntity.Id,
                    Status = (int)DataStatus.Normal,
                    UpdateDate = DateTime.Now,
                    ValueDesc = "均色"
                });
                //step3: create product size property
                var sizePropertyEntity = _productPropertyRepo.Insert(new ProductPropertyEntity()
                {
                    IsColor = false,
                    IsSize = true,
                    ProductId = productEntity.Id,
                    SortOrder = 1,
                    PropertyDesc = "尺码",
                    Status = (int)DataStatus.Normal,
                    UpdateDate = DateTime.Now,
                    UpdateUser = authuid
                });

                if (categoryEntity.C.SizeType == (int)CategorySizeType.LimitSize)
                {
                    foreach (var size in request.Size_Ids)
                    {
                        var categoryPropertyValueEntity = categoryEntity.CP.Where(cp => cp.CPV.Id == size).FirstOrDefault();
                        if (categoryPropertyValueEntity != null)
                        {
                            var sizevalueEntity = _productPropertyValueRepo.Insert(new ProductPropertyValueEntity()
                            {
                                CreateDate = DateTime.Now,
                                PropertyId = sizePropertyEntity.Id,
                                UpdateDate = DateTime.Now,
                                ValueDesc = categoryPropertyValueEntity.CPV.ValueDesc,
                                Status = (int)DataStatus.Normal
                            });
                            _inventoryRepo.Insert(new InventoryEntity()
                            {
                                Amount = 1,
                                ProductId = productEntity.Id,
                                PColorId = propertyvalueEntity.Id,
                                PSizeId = sizevalueEntity.Id,
                                UpdateDate = DateTime.Now,
                                UpdateUser = authuid

                            });
                        }

                    }
                }
                else
                {
                    var valueDesc = categoryEntity.C.SizeType == (int)CategorySizeType.Common ? "均码" : request.Size_Str;
                    var sizevalueEntity = _productPropertyValueRepo.Insert(new ProductPropertyValueEntity()
                    {
                        CreateDate = DateTime.Now,
                        PropertyId = sizePropertyEntity.Id,
                        UpdateDate = DateTime.Now,
                        ValueDesc = valueDesc,
                        Status = (int)DataStatus.Normal
                    });
                    _inventoryRepo.Insert(new InventoryEntity()
                    {
                        Amount = 1,
                        ProductId = productEntity.Id,
                        PColorId = propertyvalueEntity.Id,
                        PSizeId = sizevalueEntity.Id,
                        UpdateDate = DateTime.Now,
                        UpdateUser = authuid

                    });
                }
                //step4: create image

                var resourceEntity = _resourceService.Save(files
                                        , authuid
                                        , -1
                                        , productEntity.Id
                                        , SourceType.Product);
                if (resourceEntity.Count > 0)
                {
                    ts.Complete();
                    return this.RenderSuccess<dynamic>(c => c.Data = new
                    {
                        id = productEntity.Id,
                        image = resourceEntity.First().Name.Image320Url(),
                        price = productEntity.Price
                    });
                }
                else
                {
                    return this.RenderError(r => r.Message = "图片上传出错");
                }


            }

        }

        [RestfulRoleAuthorize(UserLevel.DaoGou)]
        public ActionResult Update(Yintai.Hangzhou.Contract.DTO.Request.IMSProductCreateRequest request, int authuid)
        {
            if (!ModelState.IsValid)
            {
                var error = ModelState.Values.Where(v => v.Errors.Count() > 0).First();
                return this.RenderError(r => r.Message = error.Errors.First().ErrorMessage);
            }
            var productEntity = Context.Set<ProductEntity>().Find(request.Id);
            if (productEntity == null)
                return this.RenderError(r => r.Message = "商品不存在");
            if (productEntity.CreatedUser != authuid)
                return this.RenderError(r => r.Message = "无权操作该商品");
            var files = Request.Files;
            bool needUpdateImage = false;
            if (files.Count == 1)
                needUpdateImage = true;
            var categoryEntity = Context.Set<TagEntity>()
                                .Where(t => t.Id == request.Category_Id)
                                .GroupJoin(Context.Set<CategoryPropertyEntity>().Join(Context.Set<CategoryPropertyValueEntity>(), o => o.Id, i => i.PropertyId, (o, i) => new { CP = o, CPV = i })
                                    , o => o.Id, i => i.CP.CategoryId, (o, i) => new { C = o, CP = i }).FirstOrDefault()
                                    ;
            if (categoryEntity == null)
                return this.RenderError(r => r.Message = "分类不存在");
            if (categoryEntity.C.SizeType == (int)CategorySizeType.LimitSize
                && request.Size_Ids.Length < 1)
                return this.RenderError(r => r.Message = "分类尺码必选");
            if (categoryEntity.C.SizeType == (int)CategorySizeType.FreeInput
                && string.IsNullOrEmpty(request.Size_Str))
                return this.RenderError(r => r.Message = "分类尺码必选");

            var brandEntity = Context.Set<BrandEntity>().Find(request.Brand_Id);
            if (brandEntity == null)
                return this.RenderError(r => r.Message = "品牌不存在");

            using (var ts = new TransactionScope())
            {
                //step1: update product 
                var productName = string.Format("{0}-{1}-{2}", brandEntity.Name, categoryEntity.C.Name, request.Sku_Code);
                productEntity.Brand_Id = request.Brand_Id;
                productEntity.UpdatedDate = DateTime.Now;
                productEntity.UpdatedUser = authuid;
                productEntity.SkuCode = request.Sku_Code;
                productEntity.Name = productName;
                productEntity.Description = productName;
                productEntity.Price = request.Price;
                productEntity.Tag_Id = request.Category_Id;
                _productRepo.Update(productEntity);

                //step2: update product size property
                var sizePropertyEntity = Context.Set<ProductPropertyEntity>().Where(pp => pp.ProductId == productEntity.Id && pp.IsSize == true).First();

                //2.1 soft delete all propertyvalues
                var propertValues = Context.Set<ProductPropertyValueEntity>().Where(ppv => ppv.PropertyId == sizePropertyEntity.Id);
                foreach (var propertyValue in propertValues)
                {
                    propertyValue.Status = (int)DataStatus.Deleted;
                    _productPropertyValueRepo.Update(propertyValue);
                }
                //2.2 update property thereus
                var colorProperty = Context.Set<ProductPropertyEntity>().Where(pp => pp.ProductId == productEntity.Id && pp.IsColor == true)
                                      .Join(Context.Set<ProductPropertyValueEntity>(), o => o.Id, i => i.PropertyId, (o, i) => i).First();
                foreach (var size in request.Size_Ids)
                {
                    var categoryPropertyValueEntity = categoryEntity.CP.Where(cp => cp.CPV.Id == size).FirstOrDefault();
                    if (categoryPropertyValueEntity != null)
                    {
                        var propertyValue = propertValues.Where(ppv => ppv.ValueDesc == categoryPropertyValueEntity.CPV.ValueDesc).FirstOrDefault();
                        if (propertyValue == null)
                        {
                            var sizevalueEntity = _productPropertyValueRepo.Insert(new ProductPropertyValueEntity()
                            {
                                CreateDate = DateTime.Now,
                                PropertyId = sizePropertyEntity.Id,
                                UpdateDate = DateTime.Now,
                                ValueDesc = categoryPropertyValueEntity.CPV.ValueDesc,
                                Status = (int)DataStatus.Normal
                            });
                            _inventoryRepo.Insert(new InventoryEntity()
                            {
                                Amount = 1,
                                ProductId = productEntity.Id,
                                PColorId = colorProperty.Id,
                                PSizeId = sizevalueEntity.Id,
                                UpdateDate = DateTime.Now,
                                UpdateUser = authuid

                            });
                        }
                        else
                        {
                            propertyValue.Status = (int)DataStatus.Normal;
                            _productPropertyValueRepo.Update(propertyValue);
                        }
                    }


                }
                bool canCommit = true;
                var currentResourceEntity = Context.Set<ResourceEntity>().Where(r => r.SourceType == (int)SourceType.Product && r.SourceId == (int)productEntity.Id && r.Status == (int)DataStatus.Normal).First();
                //step4: create image
                if (needUpdateImage)
                {

                    currentResourceEntity.Status = (int)DataStatus.Deleted;
                    currentResourceEntity.UpdatedDate = DateTime.Now;
                    _resourceRepo.Update(currentResourceEntity);

                    var resourceEntity = _resourceService.Save(files
                                       , authuid
                                       , -1
                                       , productEntity.Id
                                       , SourceType.Product);
                    if (resourceEntity.Count <= 0)
                        canCommit = false;
                    else
                    {
                        currentResourceEntity = resourceEntity.First();
                    }
                }

                if (canCommit)
                {
                    ts.Complete();
                    return this.RenderSuccess<dynamic>(c => c.Data = new
                    {
                        id = productEntity.Id,
                        image = currentResourceEntity.Name.Image320Url(),
                        price = productEntity.Price
                    });
                }
                else
                {
                    return this.RenderError(r => r.Message = "图片上传出错");
                }

            }
        }

        [RestfulAuthorize]
        public ActionResult Search(Yintai.Hangzhou.Contract.DTO.Request.IMSProductSearchRequest request)
        {
            var mockupResponse = new List<dynamic>();
            mockupResponse.Add(new
            {
                id = 1,
                desc = "系统商品1",
                price = 100.1,
                image = "",
                create_date = "2014-03-24 1:00:00",
                product_type = (int)ProductType.FromSystem
            });
            mockupResponse.Add(new
            {
                id = 1,
                desc = "系统商品2",
                price = 100.1,
                image = "",
                create_date = "2014-03-25 2:00:00",
                product_type = (int)ProductType.FromSystem
            });
            var response = new PagerInfoResponse<dynamic>(request.PagerRequest, mockupResponse.Count)
            {
                Items = mockupResponse
            };
            return this.RenderSuccess<PagerInfoResponse<dynamic>>(c => c.Data = response);
        }

        [RestfulAuthorize]
        public ActionResult Brands(Yintai.Hangzhou.Contract.DTO.Request.IMSProductBrandsRequest request)
        {
            var mockupResponse = new List<dynamic>();
            mockupResponse.Add(new
            {
                id = 1,
                name = "mockup系统品牌1"
            });
            mockupResponse.Add(new
            {
                id = 2,
                name = "mockup系统品牌2"
            });
            var response = new PagerInfoResponse<dynamic>(request.PagerRequest, mockupResponse.Count)
            {
                Items = mockupResponse
            };
            return this.RenderSuccess<PagerInfoResponse<dynamic>>(c => c.Data = response);
        }
    }
}
