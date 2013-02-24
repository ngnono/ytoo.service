using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Yintai.Architecture.Common.Models;
using Yintai.Hangzhou.Cms.WebSiteCoreV1.Dto.Resource;
using Yintai.Hangzhou.Cms.WebSiteCoreV1.Models;
using Yintai.Hangzhou.Cms.WebSiteCoreV1.Util;
using Yintai.Hangzhou.Data.Models;
using Yintai.Hangzhou.Model.Enums;
using Yintai.Hangzhou.Repository.Contract;
using Yintai.Hangzhou.Service.Contract;
using Yintai.Hangzhou.WebSupport.Binder;
using Yintai.Hangzhou.WebSupport.Mvc;

namespace Yintai.Hangzhou.Cms.WebSiteCoreV1.Controllers
{
    [AdminAuthorize]
    public class ResourceController : UserController
    {
        private readonly IResourceRepository _resourceRepository;
        private readonly IResourceService _resourceService;

        public ResourceController(IResourceService resourceService, IResourceRepository resourceRepository)
        {
            this._resourceRepository = resourceRepository;
            this._resourceService = resourceService;
        }

        public ActionResult Index(PagerRequest request, int? sort, SourceType? sourceType, int? sourceId)
        {
            return List(request, sort, sourceType, sourceId);
        }

        public ActionResult List(PagerRequest request, int? sort, SourceType? sourceType, int? sourceId)
        {
            int totalCount;
            var sortOrder = (ResourceSortOrder)(sort ?? 0);

            List<ResourceEntity> data;
            if (sourceType == null)
            {
                data = _resourceRepository.GetPagedList(request, out totalCount, sortOrder);
            }
            else
            {
                data = _resourceRepository.GetPagedList(request, out totalCount, sortOrder, sourceType, sourceId);
            }
            var vo = MappingManager.ResourceViewMapping(data);

            var v = new ResourceCollectionViewModel(request, totalCount) { Resources = vo.ToList() };

            var dto = new ListDto
                {
                    ResourceCollectionViewModel = v,
                    Sort = sort,
                    SourceId = sourceId,
                    SourceType = sourceType
                };

            return View("List", dto);
        }

        public ActionResult Details(int? id, [FetchResource(KeyName = "id")]ResourceEntity entity)
        {
            if (id == null || entity == null)
            {
                ModelState.AddModelError("", "参数验证失败.");
                return View();
            }

            var vo = MappingManager.ResourceViewMapping(entity);

            return View(vo);
        }

        public ActionResult CreateProductResource([FetchProduct(KeyName = "sid")] ProductEntity productEntity)
        {
            var vo = new ResourceViewModel();
            vo.SourceId = productEntity.Id;
            vo.SourceType = (int)SourceType.Product;

            return Create(vo);
        }

        public ActionResult CreatePromotionResource([FetchProduct(KeyName = "sid")] PromotionEntity productEntity)
        {
            var vo = new ResourceViewModel();
            vo.SourceId = productEntity.Id;
            vo.SourceType = (int)SourceType.Product;

            return Create(vo);
        }

        public ActionResult CreateStoreResource([FetchProduct(KeyName = "sid")] StoreEntity productEntity)
        {
            var vo = new ResourceViewModel();
            vo.SourceId = productEntity.Id;
            vo.SourceType = (int)SourceType.StoreLogo;

            return Create(vo);
        }

        public ActionResult CreateBrandResource([FetchProduct(KeyName = "sid")] BrandEntity productEntity)
        {
            var vo = new ResourceViewModel();
            vo.SourceId = productEntity.Id;
            vo.SourceType = (int)SourceType.StoreLogo;

            return Create(vo);
        }

        public ActionResult Create(ResourceViewModel vo)
        {
            return View("Create", vo);
        }

        //private ActionResult Create()
        //{
        //    return View("Create");
        //}

        public ActionResult Delete(int? id, [FetchResource(KeyName = "id")]ResourceEntity entity)
        {
            if (id == null || entity == null)
            {
                ModelState.AddModelError("", "参数验证失败.");
                return View();
            }

            var vo = MappingManager.ResourceViewMapping(entity);

            return View(vo);
        }

        public ActionResult Edit(int? id, [FetchResource(KeyName = "id")]ResourceEntity entity)
        {
            if (id == null || entity == null)
            {
                ModelState.AddModelError("", "参数验证失败.");
                return View();
            }

            var vo = MappingManager.ResourceViewMapping(entity);

            return View(vo);
        }

        [HttpPost]
        public ActionResult Create(FormCollection formCollection, ResourceViewModel vo)
        {
            if (ControllerContext.HttpContext.Request.Files != null && vo.SourceId > 0 && vo.SourceType > 0)
            {
                //处理 图片
                //处理文件上传

                var ids = _resourceService.Save(ControllerContext.HttpContext.Request.Files, base.CurrentUser.CustomerId, -1, vo.SourceId, (SourceType)vo.SourceType);

                if (ids != null && ids.Count > 0)
                {
                    return Success("/" + RouteData.Values["controller"] + "/list");
                }
            }

            return View(vo);
        }

        [HttpPost]
        public ActionResult Edit(FormCollection formCollection, [FetchResource(KeyName = "id")]ResourceEntity entity, ResourceViewModel vo)
        {

            ModelState.AddModelError("", "暂不支持该方法.");
            return View(vo);

            if (entity == null || !ModelState.IsValid)
            {
                ModelState.AddModelError("", "参数验证失败.");
                return View(vo);
            }

            var newEntity = MappingManager.ResourceEntityMapping(vo);
            newEntity.CreatedUser = entity.CreatedUser;
            newEntity.CreatedDate = entity.CreatedDate;
            newEntity.Status = entity.Status;

            MappingManager.ResourceEntityMapping(newEntity, entity);

            _resourceRepository.Update(entity);


            return Success("/" + RouteData.Values["controller"] + "/details/" + entity.Id.ToString(CultureInfo.InvariantCulture));
        }

        [HttpPost]
        public ActionResult Delete(FormCollection formCollection, [FetchResource(KeyName = "id")]ResourceEntity entity)
        {
            if (entity == null)
            {
                ModelState.AddModelError("", "参数验证失败.");
                return View();
            }

            entity.UpdatedDate = DateTime.Now;
            entity.UpdatedUser = CurrentUser.CustomerId;
            entity.Status = (int)DataStatus.Deleted;

            this._resourceRepository.Delete(entity);

            return Success("/" + RouteData.Values["controller"] + "/" + RouteData.Values["action"]);
        }

        [HttpPost]
        public ActionResult SetOrder(FormCollection formCollection, [FetchResource(KeyName = "id")]ResourceEntity entity, int? order)
        {
            var jsonR = new JsonResult();

            if (order == null || entity == null)
            {
                jsonR.Data = new ExecuteResult { Message = "参数错误", StatusCode = StatusCode.ClientError };
            }
            else
            {
                var t = _resourceRepository.SetOrder(entity, order.Value);

                if (t != null)
                {
                    jsonR.Data = new ExecuteResult { Message = "OK", StatusCode = StatusCode.Success };
                }
                else
                {
                    jsonR.Data = new ExecuteResult { Message = "返回为空", StatusCode = StatusCode.UnKnow };
                }
            }

            return jsonR;
        }
    }
}
