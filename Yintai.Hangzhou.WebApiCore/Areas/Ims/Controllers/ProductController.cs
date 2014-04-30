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
using Yintai.Architecture.Common.Data.EF;
using Yintai.Hangzhou.WebSupport.Binder;

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
        private IEFRepository<ProductCode2StoreCodeEntity> _productCodeRepo;
        public ProductController(IResourceService resourceService
            , IProductRepository productRepo
            , IProductPropertyRepository productPropertyRepo
            , IProductPropertyValueRepository productPropertyValueRepo
            , IInventoryRepository inventoryRepo
            , IResourceRepository resourceRepo
            , IEFRepository<ProductCode2StoreCodeEntity> productCodeRepo)
        {
            _resourceService = resourceService;
            _productRepo = productRepo;
            _productPropertyRepo = productPropertyRepo;
            _productPropertyValueRepo = productPropertyValueRepo;
            _inventoryRepo = inventoryRepo;
            _resourceRepo = resourceRepo;
            _productCodeRepo = productCodeRepo;
        }
        [RestfulRoleAuthorize(UserLevel.DaoGou)]
        public ActionResult Create([InternalJsonArrayAttribute("size_ids")]Yintai.Hangzhou.Contract.DTO.Request.IMSProductCreateRequest request, int authuid)
        {
            if (!ModelState.IsValid)
            {
                var error = ModelState.Values.Where(v => v.Errors.Count() > 0).First();
                return this.RenderError(r => r.Message = error.Errors.First().ErrorMessage);
            }
            if (request.Image_Id <= 0)
                return this.RenderError(r => r.Message = "必须传入一个图片");
            var resourceEntity = Context.Set<ResourceEntity>().Find(request.Image_Id);
            if (resourceEntity == null)
                return this.RenderError(r=>r.Message="图片不正确");
            
            var categoryEntity = Context.Set<TagEntity>()
                                .Where(t => t.Id == request.Category_Id)
                                .GroupJoin(Context.Set<CategoryPropertyEntity>().Join(Context.Set<CategoryPropertyValueEntity>(), o => o.Id, i => i.PropertyId, (o, i) => new { CP = o, CPV = i })
                                    , o => o.Id, i => i.CP.CategoryId, (o, i) => new { C = o, CP = i }).FirstOrDefault()
                                    ;
            if (categoryEntity == null)
                return this.RenderError(r => r.Message = "分类不存在");
            var catSizeType = categoryEntity.C.SizeType ?? (int)CategorySizeType.FreeInput;
            if (categoryEntity.C.SizeType == (int)CategorySizeType.LimitSize
                && (request.Size_Ids == null || request.Size_Ids.Length < 1))
                return this.RenderError(r => r.Message = "分类尺码必选");
            if (categoryEntity.C.SizeType == (int)CategorySizeType.FreeInput
                && string.IsNullOrEmpty(request.Size_Str))
                return this.RenderError(r => r.Message = "分类尺码必选");

            var brandEntity = Context.Set<BrandEntity>().Find(request.Brand_Id);
            if (brandEntity == null)
                return this.RenderError(r => r.Message = "品牌不存在");
            
            request.UnitPrice = (!request.UnitPrice.HasValue || request.UnitPrice <= 0) ? request.Price : request.UnitPrice;
            if (request.UnitPrice < request.Price)
                return this.RenderError(r => r.Message = "商品原价必须大于等于销售价！");
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
                    Status = (int)DataStatus.Normal,
                    Tag_Id = request.Category_Id,
                    UnitPrice = request.UnitPrice,
                    UpdatedDate = DateTime.Now,
                    UpdatedUser = authuid,
                    Store_Id = assocateEntity.StoreId,
                    Favorable = string.Empty

                });
                _productCodeRepo.Insert(new ProductCode2StoreCodeEntity()
                {
                    Status = (int)DataStatus.Normal,
                    ProductId = productEntity.Id,
                    StoreId = assocateEntity.StoreId,
                    StoreProductCode = request.Sales_Code,
                    UpdateDate = DateTime.Now,
                    UpdateUser = authuid
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
                    var hasValidSize = false;
                    foreach (var size in request.Size_Ids)
                    {
                        var categoryPropertyValueEntity = categoryEntity.CP.Where(cp => cp.CPV.Id == size).FirstOrDefault();
                        if (categoryPropertyValueEntity != null)
                        {
                            hasValidSize = true;
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
                    if (!hasValidSize)
                        return this.RenderError(r => r.Message = "尺码传入不正确");
                }
                else
                {
                    var valueDesc = string.IsNullOrEmpty(request.Size_Str) ? "均码" : request.Size_Str;
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

                resourceEntity.SourceType = (int)SourceType.Product;
                resourceEntity.SourceId = productEntity.Id;
                resourceEntity.ColorId = propertyvalueEntity.Id;
                _resourceRepo.Update(resourceEntity);
                
                ts.Complete();
                return this.RenderSuccess<dynamic>(c => c.Data = new
                {
                    id = productEntity.Id,
                    image = resourceEntity.Name.Image320Url(),
                    price = productEntity.Price,
                    brand_name = brandEntity.Name,
                    category_name = categoryEntity.C.Name

                });
               
            }

        }

        [RestfulRoleAuthorize(UserLevel.DaoGou)]
        public ActionResult Update([InternalJsonArrayAttribute("size_ids")]Yintai.Hangzhou.Contract.DTO.Request.IMSProductCreateRequest request, int authuid)
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
            if (request.Image_Id >0 )
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

            bool isLimitSize = categoryEntity.C.SizeType == (int)CategorySizeType.LimitSize;
            var brandEntity = Context.Set<BrandEntity>().Find(request.Brand_Id);
            if (brandEntity == null)
                return this.RenderError(r => r.Message = "品牌不存在");
            request.UnitPrice = (!request.UnitPrice.HasValue || request.UnitPrice <= 0) ? request.Price : request.UnitPrice;
            if (request.UnitPrice < request.Price)
                return this.RenderError(r => r.Message = "商品原价必须大于等于销售价！");
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
                productEntity.UnitPrice = request.UnitPrice;
                productEntity.Tag_Id = request.Category_Id;
                _productRepo.Update(productEntity);

                var productcodeEntity = Context.Set<ProductCode2StoreCodeEntity>().Where(pc => pc.ProductId == request.Id && pc.Status == (int)DataStatus.Normal).First();
                productcodeEntity.StoreProductCode = request.Sales_Code;
                productcodeEntity.UpdateDate = DateTime.Now;
                _productCodeRepo.Update(productcodeEntity);

                //step2: update product size property
                var sizePropertyEntity = Context.Set<ProductPropertyEntity>().Where(pp => pp.ProductId == productEntity.Id && pp.IsSize == true).First();

                //2.1 soft delete all propertyvalues
                var propertValues = Context.Set<ProductPropertyValueEntity>().Where(ppv => ppv.PropertyId == sizePropertyEntity.Id);
                foreach (var propertyValue in propertValues)
                {
                    propertyValue.Status = (int)DataStatus.Deleted;
                    _productPropertyValueRepo.Update(propertyValue);
                }
                var inventories = Context.Set<InventoryEntity>().Where(i => i.ProductId == productEntity.Id);
                foreach (var inventory in inventories)
                {
                    inventory.Amount = 0;
                    _inventoryRepo.Update(inventory);
                }
                //2.2 update property thereus
                var colorProperty = Context.Set<ProductPropertyEntity>().Where(pp => pp.ProductId == productEntity.Id && pp.IsColor == true)
                                      .Join(Context.Set<ProductPropertyValueEntity>(), o => o.Id, i => i.PropertyId, (o, i) => i).First();
                if (isLimitSize)
                {
                    foreach (var size in request.Size_Ids)
                    {
                        var categoryPropertyValueEntity = categoryEntity.CP.Where(cp => cp.CPV.Id == size).FirstOrDefault();
                        if (categoryPropertyValueEntity != null)
                        {
                            var propertyValue = propertValues.Where(ppv => ppv.ValueDesc == categoryPropertyValueEntity.CPV.ValueDesc).FirstOrDefault();
                            UpdateSingleProperty(productEntity.Id, colorProperty.Id, request.Size_Str, sizePropertyEntity.Id, propertyValue);
                        }
                    }
                }
                else
                {
                    var propertyValue = propertValues.Where(ppv => ppv.ValueDesc == request.Size_Str).FirstOrDefault();
                    UpdateSingleProperty(productEntity.Id, colorProperty.Id, request.Size_Str, sizePropertyEntity.Id, propertyValue);

                }
                bool canCommit = true;
                var currentResourceEntity = Context.Set<ResourceEntity>().Where(r => r.SourceType == (int)SourceType.Product && r.SourceId == (int)productEntity.Id && r.Status == (int)DataStatus.Normal).First();
                //step4: create image
                if (needUpdateImage)
                {
                    currentResourceEntity.Status = (int)DataStatus.Deleted;
                    currentResourceEntity.UpdatedDate = DateTime.Now;
                    _resourceRepo.Update(currentResourceEntity);

                    var colorPropertyId = currentResourceEntity.ColorId;
                    currentResourceEntity = Context.Set<ResourceEntity>().Find(request.Image_Id);
                    currentResourceEntity.SourceType = (int)SourceType.Product;
                    currentResourceEntity.SourceId = productEntity.Id;
                    currentResourceEntity.ColorId = colorPropertyId;
                    _resourceRepo.Update(currentResourceEntity);
                    
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

        private void UpdateSingleProperty(int productId, int colorId, string size, int sizeId, ProductPropertyValueEntity propertyValue)
        {
            if (propertyValue == null)
            {
                var sizevalueEntity = _productPropertyValueRepo.Insert(new ProductPropertyValueEntity()
                {
                    CreateDate = DateTime.Now,
                    PropertyId = sizeId,
                    UpdateDate = DateTime.Now,
                    ValueDesc = size,
                    Status = (int)DataStatus.Normal
                });
                _inventoryRepo.Insert(new InventoryEntity()
                {
                    Amount = 1,
                    ProductId = productId,
                    PColorId = colorId,
                    PSizeId = sizevalueEntity.Id,
                    UpdateDate = DateTime.Now,
                    UpdateUser = 0

                });
            }
            else
            {
                var inventory = Context.Set<InventoryEntity>().Where(i => i.ProductId == productId && i.PColorId == colorId && i.PSizeId == sizeId).FirstOrDefault();
                if (inventory != null)
                {
                    inventory.Amount = 1;
                    _inventoryRepo.Update(inventory);
                }
                propertyValue.Status = (int)DataStatus.Normal;
                _productPropertyValueRepo.Update(propertyValue);
            }



        }


        [RestfulRoleAuthorize(UserLevel.DaoGou)]
        public ActionResult Detail(int id, int authuid)
        {
            var productEntity = Context.Set<ProductEntity>().Where(p => p.Id == id && p.Status != (int)DataStatus.Deleted)
                                .Join(Context.Set<TagEntity>(), o => o.Tag_Id, i => i.Id, (o, i) => new { P = o, C = i })
                                .Join(Context.Set<BrandEntity>(), o => o.P.Brand_Id, i => i.Id, (o, i) => new { P = o.P, C = o.C, B = i })
                                .Join(Context.Set<ProductCode2StoreCodeEntity>(), o => o.P.Id, i => i.ProductId, (o, i) => new { P = o.P, C = o.C, B = o.B, PC = i })
                                .GroupJoin(Context.Set<ResourceEntity>().Where(r => r.SourceType == (int)SourceType.Product && r.Status == (int)DataStatus.Normal),
                                            o => o.P.Id,
                                            i => i.SourceId,
                                            (o, i) => new { P = o.P, C = o.C, B = o.B, PC = o.PC, PR = i.FirstOrDefault() })
                                .FirstOrDefault();
            if (productEntity == null)
                return this.RenderError(r => r.Message = "商品不存在");
            if (productEntity.P.CreatedUser != authuid)
                return this.RenderError(r => r.Message = "无权操作该商品");
            var sizeValues = Context.Set<ProductPropertyEntity>().Where(pp => pp.ProductId == id && pp.IsSize == true && pp.Status == (int)DataStatus.Normal)
                             .GroupJoin(Context.Set<ProductPropertyValueEntity>().Where(ppv => ppv.Status == (int)DataStatus.Normal)
                                        , o => o.Id
                                        , i => i.PropertyId
                                        , (o, i) => i)
                             .FirstOrDefault().ToList();


            return this.RenderSuccess<Yintai.Hangzhou.Contract.DTO.Response.IMSProductSelfDetailResponse>(r =>
                            r.Data = new IMSProductSelfDetailResponse().FromEntity<IMSProductSelfDetailResponse>(productEntity.P, p =>
                            {
                                p.Brand_Name = productEntity.B.Name;
                                p.Category_Name = productEntity.C.Name;
                                p.SalesCode = productEntity.PC.StoreProductCode;
                                p.SizeType = productEntity.C.SizeType.Value;
                                if (sizeValues != null)
                                {
                                    if (productEntity.C.SizeType == (int)CategorySizeType.LimitSize)
                                    {
                                        var catSizeValues = Context.Set<CategoryPropertyEntity>().Where(cp => cp.CategoryId == productEntity.P.Tag_Id && cp.Status == (int)DataStatus.Normal && cp.IsSize == true)
                                                             .Join(Context.Set<CategoryPropertyValueEntity>().Where(cpv => cpv.Status == (int)DataStatus.Normal),
                                                                o => o.Id,
                                                                i => i.PropertyId,
                                                                (o, i) => i);
                                        p.Sizes = catSizeValues.ToList().Where(csv => sizeValues.Any(sv => sv.ValueDesc == csv.ValueDesc)).Select(csv => new IMSProductSizeResponse()
                                        {
                                            SizeName = csv.ValueDesc,
                                            SizeValueId = csv.Id
                                        });
                                    }
                                    else
                                    {
                                        var firstSize = sizeValues.FirstOrDefault();
                                        if (firstSize != null)
                                            p.Size_Str = firstSize.ValueDesc;
                                    }
                                }
                                if (productEntity.PR != null)
                                {
                                    p.ImageUrl = productEntity.PR.Name;
                                    p.Image_Id = productEntity.PR.Id;
                                }
                            }));
        }


    }
}
