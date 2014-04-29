using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Web.Mvc;
using Yintai.Architecture.Common.Data.EF;
using Yintai.Hangzhou.Contract.DTO.Request;
using Yintai.Hangzhou.Contract.DTO.Response;
using Yintai.Hangzhou.Data.Models;
using Yintai.Hangzhou.Model.Enums;
using Yintai.Hangzhou.Repository.Contract;
using Yintai.Hangzhou.WebSupport.Mvc;
using com.intime.fashion.common.Extension;
using Yintai.Architecture.Common.Models;
using Yintai.Hangzhou.WebSupport.Binder;
using Yintai.Hangzhou.Contract.DTO.Response.Resources;
using Yintai.Hangzhou.Service.Logic;
using com.intime.fashion.common;

namespace Yintai.Hangzhou.WebApiCore.Areas.Ims.Controllers
{
    public class ComboController : RestfulController
    {
        private IEFRepository<IMS_ComboEntity> _comboRepo;
        private IEFRepository<IMS_Combo2ProductEntity> _combo2productRepo;
        private IResourceRepository _resourceRepo;
        private IEFRepository<IMS_AssociateItemsEntity> _associateItemRepo;
        public ComboController(IEFRepository<IMS_ComboEntity> comboRepo
            , IEFRepository<IMS_Combo2ProductEntity> combo2productRepo
            , IResourceRepository resourceRepo,
            IEFRepository<IMS_AssociateItemsEntity> associateItemRepo)
        {
            _comboRepo = comboRepo;
            _combo2productRepo = combo2productRepo;
            _resourceRepo = resourceRepo;
            _associateItemRepo = associateItemRepo;
        }
        [RestfulRoleAuthorize(UserLevel.DaoGou)]
        public ActionResult Create([InternalJsonArrayAttribute("image_ids,productids")] IMSComboCreateRequest request, int authuid)
        {
            if (request.Image_Ids == null ||
               request.Image_Ids.Length < 1)
                return this.RenderError(r => r.Message = "搭配必须图片");
            if (request.ProductIds == null ||
                request.ProductIds.Length < 1)
                return this.RenderError(r => r.Message = "搭配需要至少一个商品");
            if (string.IsNullOrEmpty(request.Desc))
                return this.RenderError(r => r.Message = "搭配需要描述");
            if (!Array.Exists<int>((int[])Enum.GetValues(typeof(ProductType)), s => request.Product_Type == s))
                return this.RenderError(r => r.Message = "搭配商品类型不正确");
            var products = Context.Set<ProductEntity>().Where(p => request.ProductIds.Any(l => l == p.Id) &&
                            ((p.ProductType.HasValue && p.ProductType == request.Product_Type) ||
                            (!p.ProductType.HasValue && request.Product_Type == (int)ProductType.FromSystem)));
            if (products.Count() < 1)
                return this.RenderError(r => r.Message = "商品类型不正确");
             var resources = Context.Set<ResourceEntity>().Where(r => request.Image_Ids.Any(image => image == r.Id));
             if (!resources.Any())
                 return this.RenderError(r => r.Message = "搭配必须图片");
            var associateEntity = Context.Set<IMS_AssociateEntity>().Where(ia => ia.UserId == authuid).First();
            var canOnline = ComboLogic.IfCanOnline(authuid);

            using (var ts = new TransactionScope())
            {
                //step1: create combo
                var comboEntity = _comboRepo.Insert(new IMS_ComboEntity()
                {
                    CreateDate = DateTime.Now,
                    CreateUser = authuid,
                    Desc = request.Desc,
                    OnlineDate = DateTime.Now,
                    Price = products.Sum(p => p.Price),
                    Private2Name = request.Private_To ?? string.Empty,
                    Status = canOnline?(int)DataStatus.Normal:(int)DataStatus.Default,
                    UpdateDate = DateTime.Now,
                    UpdateUser = authuid,
                    UserId = authuid,
                    ProductType = request.Product_Type,
                    ExpireDate = DateTime.Now.AddDays(ConfigManager.COMBO_EXPIRED_DAYS)
                });

                //step2: create combo2product
                foreach (var product in products)
                {
                    _combo2productRepo.Insert(new IMS_Combo2ProductEntity()
                    {
                        ComboId = comboEntity.Id,
                        ProductId = product.Id
                    });
                }

                //step2.1 associate combo
                _associateItemRepo.Insert(new IMS_AssociateItemsEntity()
                {
                    AssociateId = associateEntity.Id,
                    CreateDate = DateTime.Now,
                    CreateUser = authuid,
                    ItemId = comboEntity.Id,
                    ItemType = (int)ComboType.Product,
                    Status = canOnline ? (int)DataStatus.Normal : (int)DataStatus.Default,
                    UpdateDate = DateTime.Now,
                    UpdateUser = authuid
                });

                //step3: bind images
               
                foreach (var resource in resources)
                {
                    resource.SourceType = (int)SourceType.Combo;
                    resource.SourceId = comboEntity.Id;
                    _resourceRepo.Update(resource);
                }

                ts.Complete();
                return this.RenderSuccess<dynamic>(c => c.Data = new
                {
                    combo_id = comboEntity.Id
                });
            }

        }

        [RestfulRoleAuthorize(UserLevel.DaoGou)]
        public ActionResult Update([InternalJsonArrayAttribute("image_ids,productids")]IMSComboUpdateRequest request, int authuid)
        {

            if (string.IsNullOrEmpty(request.Desc))
                return this.RenderError(r => r.Message = "搭配需要描述");
            if (!Array.Exists<int>((int[])Enum.GetValues(typeof(ProductType)), s => request.Product_Type == s))
                return this.RenderError(r => r.Message = "搭配商品类型不正确");

            var comboEntity = Context.Set<IMS_ComboEntity>().Find(request.Id);
            if (comboEntity == null)
                return this.RenderError(r => r.Message = "搭配不存在");

            bool noProduct = request.ProductIds == null || request.ProductIds.Length == 0;
            bool noImage = request.Image_Ids == null || request.Image_Ids.Length == 0;
            using (var ts = new TransactionScope())
            {
                //step1: 更新combo

                comboEntity.Private2Name = request.Private_To??string.Empty;
                comboEntity.ProductType = request.Product_Type;
                comboEntity.Desc = request.Desc;
                comboEntity.UpdateDate = DateTime.Now;
                if (!noProduct)
                {
                    var products = Context.Set<ProductEntity>().Where(p => request.ProductIds.Contains(p.Id));
                    comboEntity.Price = products.Sum(p => p.Price);
                    _combo2productRepo.Delete(icp => icp.ComboId == comboEntity.Id);
                    foreach (var product in products)
                    {
                        _combo2productRepo.Insert(new IMS_Combo2ProductEntity()
                        {
                            ComboId = comboEntity.Id,
                            ProductId = product.Id
                        });
                    }
                }
                _comboRepo.Update(comboEntity);



                //step3: bind images
                if (!noImage)
                {
                    var newresources = Context.Set<ResourceEntity>().Where(r => request.Image_Ids.Any(image => image == r.Id));
                    var oldresources = Context.Set<ResourceEntity>().Where(r => r.SourceType == (int)SourceType.Combo && r.SourceId == comboEntity.Id);
                    foreach (var oldResource in oldresources)
                    {
                        oldResource.Status = (int)DataStatus.Deleted;
                        oldResource.UpdatedDate = DateTime.Now;
                        _resourceRepo.Update(oldResource);
                    }
                    foreach (var resource in newresources)
                    {
                        resource.Status = (int)DataStatus.Normal;
                        resource.UpdatedDate = DateTime.Now;
                        resource.SourceType = (int)SourceType.Combo;
                        resource.SourceId = comboEntity.Id;
                        _resourceRepo.Update(resource);
                    }
                }

                ts.Complete();
                return this.RenderSuccess<dynamic>(c => c.Data = new { 
                    combo_id = request.Id
                });
            }

        }

        [RestfulRoleAuthorize(UserLevel.DaoGou)]
        public ActionResult Delete(int id, int authuid)
        {

            var comboEntity = Context.Set<IMS_ComboEntity>().Find(id);
            if (comboEntity == null)
                return this.RenderError(r => r.Message = "搭配不存在");
            using (var ts = new TransactionScope())
            {
                comboEntity.Status = (int)DataStatus.Deleted;
                comboEntity.UpdateDate = DateTime.Now;
                comboEntity.UpdateUser = authuid;
                _comboRepo.Update(comboEntity);

                var associateItemEntity = Context.Set<IMS_AssociateItemsEntity>().Where(ia => ia.ItemType == (int)ComboType.Product &&
                            ia.ItemId == id).FirstOrDefault();
                if (associateItemEntity != null)
                {
                    associateItemEntity.Status = (int)DataStatus.Deleted;
                    associateItemEntity.UpdateDate = DateTime.Now;
                    associateItemEntity.UpdateUser = authuid;
                    _associateItemRepo.Update(associateItemEntity);
                }

                ts.Complete();
            }
                return this.RenderSuccess<dynamic>(null);
        }
        [RestfulAuthorize]
        public ActionResult Detail(int id, int authuid)
        {
            var comboEntity = Context.Set<IMS_ComboEntity>().Where(ic => ic.Id == id)
                              .Join(Context.Set<IMS_AssociateItemsEntity>().Where(iai => iai.ItemType == (int)ComboType.Product),
                                            o => o.Id,
                                            i => i.ItemId,
                                            (o, i) => new { C=o,A=i})
                              .GroupJoin(Context.Set<ResourceEntity>().Where(r => r.Status == (int)DataStatus.Normal && r.SourceType == (int)SourceType.Combo),
                                    o => o.C.Id,
                                    i => i.SourceId,
                                    (o, i) => new { C = o.C,A=o.A, CR = i.OrderByDescending(ir => ir.SortOrder) }).FirstOrDefault();
            if (comboEntity == null)
                return this.RenderError(r => r.Message = "搭配不存在");
            return this.RenderSuccess<IMSComboDetailResponse>(c => c.Data = new IMSComboDetailResponse().FromEntity<IMSComboDetailResponse>(comboEntity.C, oc =>
            {
                oc.StoreId = comboEntity.A.AssociateId;
                oc.Images = comboEntity.CR.ToList().Select(cr => new {id = cr.Id,name = cr.Name.Image320Url()});
                oc.Products = Context.Set<ProductEntity>()
                            .Join(Context.Set<IMS_Combo2ProductEntity>().Where(icp => icp.ComboId == id), oo => oo.Id, i => i.ProductId, (oo, i) => oo)
                            .GroupJoin(Context.Set<ResourceEntity>().Where(r => r.SourceType == (int)SourceType.Product && r.Type == (int)ResourceType.Image && r.Status == (int)DataStatus.Normal),
                                        o => o.Id,
                                        i => i.SourceId,
                                        (o, i) => new { P = o, PR = i.OrderByDescending(ir => ir.SortOrder).FirstOrDefault() })
                            .GroupJoin(Context.Set<InventoryEntity>(), o => o.P.Id, i => i.ProductId, (o, i) => new
                            {
                                P = o.P,
                                PR = o.PR,
                                PI = i.OrderByDescending(pi => pi.Amount).FirstOrDefault()
                            })
                            .ToList().Select(p => new IMSProductDetailResponse().FromEntity<IMSProductDetailResponse>(p.P, po =>
                            {
                                po.ImageUrl = p.PR == null ? string.Empty : p.PR.Name;
                                po.IsOnline = p.P.Status==(int)DataStatus.Normal && (p.P.Is4Sale??false)==true && p.PI!=null && p.PI.Amount>0;
                            }));
                oc.Is_Owner = authuid == comboEntity.C.UserId;
                oc.Is_Favored = Context.Set<FavoriteEntity>().Any(f => f.User_Id == authuid &&
                                    f.FavoriteSourceType == (int)SourceType.Combo &&
                                    f.FavoriteSourceId == comboEntity.C.Id &&
                                    f.Status == (int)DataStatus.Normal);

            }));

        }

        [RestfulAuthorize]
        public ActionResult Detail4P(int id, int authuid)
        {
            var linq = Context.Set<IMS_Combo2ProductEntity>().Where(icp => icp.ComboId == id)
                      .Join(Context.Set<ProductEntity>(),
                              o => o.ProductId,
                              i => i.Id,
                              (o, i) => i)
                      .Join(Context.Set<BrandEntity>(), o => o.Brand_Id, i => i.Id, (o, i) => new { P = o, B = i });


            var response = new PagerInfoResponse<GetProductInfo4PResponse>(new PagerRequest(), linq.Count())
            {
                Items = linq.ToList().Select(l => new GetProductInfo4PResponse().FromEntity<GetProductInfo4PResponse>(l.P, res =>
                {
                    
                    res.SaleColors = Context.Set<InventoryEntity>().Where(pi => pi.ProductId == l.P.Id &&
                                                l.P.Is4Sale.HasValue && l.P.Is4Sale==true &&
                                                l.P.Status == (int)DataStatus.Normal &&
                                                pi.Amount > 0).GroupBy(pi => pi.PColorId)
                                    .Select(pi => pi.Key)
                                    .Join(Context.Set<ProductPropertyValueEntity>(), o => o, i => i.Id, (o, i) => i)
                                    .GroupJoin(Context.Set<ResourceEntity>().Where(pr => pr.SourceType == (int)SourceType.Product && pr.Type == (int)ResourceType.Image && pr.SourceId == l.P.Id), o => o.Id, i => i.ColorId, (o, i) => new { C = o, CR = i.FirstOrDefault() })
                                    .ToList()
                                    .Select(color => new SaleColorPropertyResponse()
                                    {
                                        ColorId = color.C.Id,
                                        ColorName = color.C.ValueDesc,
                                        Resource = new ResourceInfoResponse().FromEntity<ResourceInfoResponse>(color.CR),
                                        Sizes = Context.Set<InventoryEntity>().Where(pi => pi.ProductId == l.P.Id && pi.PColorId == color.C.Id)
                                                .Join(Context.Set<ProductPropertyValueEntity>().Where(ppv=>ppv.Status==(int)DataStatus.Normal), o => o.PSizeId, i => i.Id, (o, i) => new { PI = o, PPV = i }).ToList()
                                                .Where(pi=>pi.PI.Amount>0)
                                                .Select(pi => new SaleSizePropertyResponse()
                                                {
                                                    SizeId = pi.PI.PSizeId,
                                                    SizeName = pi.PPV.ValueDesc,
                                                    Is4Sale = true
                                                })

                                    });
                    if (l.B != null)
                    {
                        res.BrandId = l.B.Id;
                        res.BrandName = l.B.Name;
                        res.Brand2Name = l.B.EnglishName;
                    }
                     })).ToList<GetProductInfo4PResponse>()
                };
            return this.RenderSuccess<PagerInfoResponse<GetProductInfo4PResponse>>(m => m.Data = response);

        }

        [RestfulAuthorize]
        public ActionResult ComputeAmount(int combo_id)
        {
            var linq = Context.Set<IMS_Combo2ProductEntity>().Where(icp => icp.ComboId == combo_id)
                     .Join(Context.Set<ProductEntity>().Where(p => p.Is4Sale == true && p.Status == (int)DataStatus.Normal
                                        && Context.Set<InventoryEntity>().Where(i=>i.Amount>0).Any(i=>i.ProductId==p.Id)),
                             o => o.ProductId,
                             i => i.Id,
                             (o, i) => i);
                     

            var model = OrderRule.ComputeAmount(linq, 1);

            return this.RenderSuccess<dynamic>(m => m.Data = new
            {
                totalquantity = model.TotalQuantity,
                totalfee = model.TotalFee,
                totalpoints = model.TotalPoints,
                totalamount = model.TotalAmount,
                extendprice = model.ExtendPrice
            });

        }

       
    }
}
