using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Web.Mvc;
using Yintai.Architecture.Common.Data.EF;
using Yintai.Hangzhou.Contract.DTO.Request;
using Yintai.Hangzhou.Data.Models;
using Yintai.Hangzhou.Model.Enums;
using Yintai.Hangzhou.Repository.Contract;
using Yintai.Hangzhou.WebSupport.Mvc;

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
            var products = Context.Set<ProductEntity>().Where(p=>request.ProductIds.Any(l=>l==p.Id) && p.ProductType==request.Product_Type);
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
                foreach (var imageId in request.Image_Ids)
                {
                    var resourceEntity = Context.Set<ResourceEntity>().Find(imageId);
                    resourceEntity.SourceType = (int)SourceType.Combo;
                    resourceEntity.SourceId = comboEntity.Id;
                    _resourceRepo.Update(resourceEntity);
                }

                ts.Complete();
                 return this.RenderSuccess<dynamic>(null);
            }
           
        }

        [RestfulAuthorize]
        public ActionResult Detail(int id,int authuid)
        {
            return this.RenderSuccess<dynamic>(c => c.Data = new
            {
                id = 1,
                desc = "mockup detail详情",
                private_to = "东哥",
                image = new string[]{ "http://irss.ytrss.com/fileupload/img/product/20140321/3c955b75-aa89-4732-96ea-925f6dd853e3_320x0.jpg", 
                                    "http://irss.ytrss.com/fileupload/img/product/20140321/3c955b75-aa89-4732-96ea-925f6dd853e3_320x0.jpg"},
                price = 200.1,
                products = new dynamic[]{new {id = 1,
                                            product_type=1,
                                            image = "http://irss.ytrss.com/fileupload/img/product/20140321/3c955b75-aa89-4732-96ea-925f6dd853e3_320x0.jpg",
                                            price = 100.1},
                new {id = 2,
                                            product_type=1,
                                            image = "http://irss.ytrss.com/fileupload/img/product/20140321/3c955b75-aa89-4732-96ea-925f6dd853e3_320x0.jpg",
                                            price = 100.2},
                }
            });
        }
    }
}
