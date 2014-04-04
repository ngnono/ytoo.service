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

namespace Yintai.Hangzhou.WebApiCore.Areas.Ims.Controllers
{
    public class ComboController:RestfulController
    {
        private IEFRepository<IMS_ComboEntity> _comboRepo;
        private IEFRepository<IMS_Combo2ProductEntity> _combo2productRepo;
        private IResourceRepository _resourceRepo;
        public ComboController(IEFRepository<IMS_ComboEntity> comboRepo
            ,IEFRepository<IMS_Combo2ProductEntity> combo2productRepo
            ,IResourceRepository resourceRepo)
        {
            _comboRepo = comboRepo;
            _combo2productRepo = combo2productRepo;
            _resourceRepo = resourceRepo;
        }
        [RestfulRoleAuthorize(UserLevel.DaoGou)]
        public ActionResult Create(IMSComboCreateRequest request,int authuid)
        {
            if (request.Image_Ids == null ||
               request.Image_Ids.Length < 1)
                return this.RenderError(r => r.Message = "搭配必须图片");
            if (request.ProductIds == null ||
                request.ProductIds.Length < 1)
                return this.RenderError(r => r.Message = "搭配需要至少一个商品");
            if (string.IsNullOrEmpty(request.Desc))
                return this.RenderError(r=>r.Message="搭配需要描述");
            if (string.IsNullOrEmpty(request.Private_To))
                return this.RenderError(r=>r.Message="搭配需要选择私人定制对象");
            if (!Array.Exists<int>((int[])Enum.GetValues(typeof(ProductType)), s=>request.Product_Type==s))
                return this.RenderError(r=>r.Message="搭配商品类型不正确");
            var products = Context.Set<ProductEntity>().Where(p=>request.ProductIds.Any(l=>l==p.Id) &&
                            ((p.ProductType.HasValue && p.ProductType==request.Product_Type) ||
                            (!p.ProductType.HasValue && request.Product_Type==(int)ProductType.FromSystem)));
            if (products.Count() < 1)
                return this.RenderError(r => r.Message = "商品类型不正确");

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
                    //todo: need replace with template
                    Private2Name = request.Private_To,
                    Status = (int)DataStatus.Normal,
                    UpdateDate = DateTime.Now,
                    UpdateUser = authuid,
                    UserId = authuid,
                    ProductType = request.Product_Type

                });

                //step2: create combo2product
                foreach (var product in products)
                {
                    _combo2productRepo.Insert(new IMS_Combo2ProductEntity() { 
                         ComboId = comboEntity.Id,
                         ProductId = product.Id
                    });
                }

                //step3: bind images
                var resources = Context.Set<ResourceEntity>().Where(r => request.Image_Ids.Any(image => image == r.Id));
                foreach (var resource in resources)
                {
                    resource.SourceType = (int)SourceType.Combo;
                    resource.SourceId = comboEntity.Id;
                    _resourceRepo.Update(resource);
                }

                ts.Complete();
                return this.RenderSuccess<dynamic>(c => c.Data = new { 
                    combo_id = comboEntity.Id
                });
            }
           
        }

        [RestfulRoleAuthorize(UserLevel.DaoGou)]
        public ActionResult Update(IMSComboUpdateRequest request, int authuid)
        {
           
            if (string.IsNullOrEmpty(request.Desc))
                return this.RenderError(r => r.Message = "搭配需要描述");
            if (string.IsNullOrEmpty(request.Private_To))
                return this.RenderError(r => r.Message = "搭配需要选择私人定制对象");
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

                comboEntity.Private2Name = request.Private_To;
                comboEntity.ProductType = request.Product_Type;
                comboEntity.Desc = request.Desc;
                comboEntity.UpdateDate = DateTime.Now;
                if (!noProduct)
                {
                    var products = Context.Set<ProductEntity>().Where(p=>request.ProductIds.Contains(p.Id));
                    comboEntity.Price = products.Sum(p => p.Price);
                    _combo2productRepo.Delete(icp=>icp.ComboId==comboEntity.Id);
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
                    var oldresources = Context.Set<ResourceEntity>().Where(r=>r.SourceType==(int)SourceType.Combo && r.SourceId==comboEntity.Id);
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
                return this.RenderSuccess<dynamic>(null);
            }

        }
        [RestfulAuthorize]
        public ActionResult Detail(int id,int authuid)
        {
            var comboEntity = Context.Set<IMS_ComboEntity>().Where(ic => ic.Id == id)
                              .GroupJoin(Context.Set<ResourceEntity>().Where(r => r.Status == (int)DataStatus.Normal && r.SourceType == (int)SourceType.Combo),
                                    o => o.Id,
                                    i => i.SourceId,
                                    (o, i) => new { C = o, CR = i.OrderByDescending(ir => ir.SortOrder) }).FirstOrDefault();
            if (comboEntity == null)
                return this.RenderError(r => r.Message = "搭配不存在");
            return this.RenderSuccess<IMSComboDetailResponse>(c => c.Data = new IMSComboDetailResponse().FromEntity<IMSComboDetailResponse>(comboEntity.C, oc => {
                oc.Images = comboEntity.CR.ToList().Select(cr => cr.Name.Image320Url());
                oc.Products = Context.Set<ProductEntity>().Join(Context.Set<IMS_Combo2ProductEntity>().Where(icp => icp.ComboId == id), oo => oo.Id, i => i.ProductId, (oo, i) => oo)
                            .GroupJoin(Context.Set<ResourceEntity>().Where(r => r.SourceType == (int)SourceType.Product && r.Type == (int)ResourceType.Image && r.Status == (int)DataStatus.Normal),
                                        o => o.Id,
                                        i => i.SourceId,
                                        (o, i) => new { P = o, PR = i.OrderByDescending(ir => ir.SortOrder).FirstOrDefault() })
                            .ToList().Select(p => new IMSProductDetailResponse().FromEntity<IMSProductDetailResponse>(p, po => {
                                po.ImageUrl = p.PR.Name;
                            }));

            }));
          
        }
    }
}
