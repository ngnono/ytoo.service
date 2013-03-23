using System;
using System.Globalization;
using System.Linq;
using System.Transactions;
using System.Web.Mvc;
using Yintai.Architecture.Common.Models;
using Yintai.Hangzhou.Cms.WebSiteCoreV1.Manager;
using Yintai.Hangzhou.Cms.WebSiteCoreV1.Models;
using Yintai.Hangzhou.Cms.WebSiteCoreV1.Util;
using Yintai.Hangzhou.Data.Models;
using Yintai.Hangzhou.Model.Enums;
using Yintai.Hangzhou.Model.Filters;
using Yintai.Hangzhou.Repository.Contract;
using Yintai.Hangzhou.Service.Contract;
using Yintai.Hangzhou.WebSupport.Binder;
using Yintai.Hangzhou.WebSupport.Mvc;

namespace Yintai.Hangzhou.Cms.WebSiteCoreV1.Controllers
{
    [AdminAuthorize]
    public class BrandController : UserController
    {
        private readonly IBrandRepository _brandRepository;
        private IResourceService _resourceService;
        private IResourceRepository _resourceRepository;
        public BrandController(IBrandRepository brandRepository
            ,IResourceService resourceService
            ,IResourceRepository resourceRepository)
        {
            this._brandRepository = brandRepository;
            _resourceService = resourceService;
            _resourceRepository = resourceRepository;
        }

        public ActionResult Index(PagerRequest request, BrandListSearchOption search, int? sort)
        {
            return List(request,search, sort);
        }

        public ActionResult List(PagerRequest request,BrandListSearchOption search, int? sort)
        {
            int totalCount;
            var data = _brandRepository.Get(e => (!search.PId.HasValue || e.Id == search.PId.Value)
                                                   && (string.IsNullOrEmpty(search.Name) || e.Name.ToLower().StartsWith(search.Name.ToLower()))
                                                   && e.Status != (int)DataStatus.Deleted
                                             , out totalCount
                                             , request.PageIndex
                                             , request.PageSize
                                             , e =>
                                             {
                                                 if (!search.OrderBy.HasValue)
                                                     return e.OrderByDescending(o => o.CreatedDate);
                                                 else
                                                 {
                                                     switch (search.OrderBy.Value)
                                                     {
                                                         case GenericOrder.OrderByCreateUser:
                                                             return e.OrderByDescending(o => o.CreatedUser);
                                                         case GenericOrder.OrderByName:
                                                             return e.OrderByDescending(o => o.Name);
                                                         case GenericOrder.OrderByCreateDate:
                                                         default:
                                                             return e.OrderByDescending(o => o.CreatedDate);

                                                     }
                                                 }
                                             });
            var vo = MappingManager.BrandViewMapping(data.ToList());

            var v = new BrandCollectionViewModel(request, totalCount) { Brands = vo.ToList() };

            return View("List", v);
        }

        public ActionResult Details(int? id, [FetchBrand(KeyName = "id")]BrandEntity entity)
        {
            if (id == null || entity == null)
            {
                ModelState.AddModelError("", "参数验证失败.");
                return View();
            }

            var vo = MappingManager.BrandViewMapping(entity);

            return View(vo);
        }

        public ActionResult Create()
        {
            return View();
        }

        public ActionResult Delete(int? id, [FetchBrand(KeyName = "id")]BrandEntity entity)
        {
            if (id == null || entity == null)
            {
                ModelState.AddModelError("", "参数验证失败.");
                return View();
            }

            var vo = MappingManager.BrandViewMapping(entity);

            return View(vo);
        }

        public ActionResult Edit(int? id, [FetchBrand(KeyName = "id")]BrandEntity entity)
        {
            if (id == null || entity == null)
            {
                ModelState.AddModelError("", "参数验证失败.");
                return View();
            }

            var vo = MappingManager.BrandViewMapping(entity);

            return View(vo);
        }

        [HttpPost]
        public ActionResult Create(FormCollection formCollection, BrandViewModel vo)
        {
            if (ModelState.IsValid)
            {
                var entity = MappingManager.BrandEntityMapping(vo);
                entity.CreatedUser = base.CurrentUser.CustomerId;
                entity.UpdatedUser = base.CurrentUser.CustomerId;
                entity.WebSite = vo.WebSite ?? string.Empty;
                entity.Status = (int)DataStatus.Normal;
                entity.CreatedDate = DateTime.Now;
                entity.UpdatedDate = DateTime.Now;
                using (TransactionScope ts = new TransactionScope())
                {

                    entity = this._brandRepository.Insert(entity);
                    if (ControllerContext.HttpContext.Request.Files.Count > 0)
                    {
                        var resources = _resourceService.Save(ControllerContext.HttpContext.Request.Files
                            , CurrentUser.CustomerId
                            , -1, entity.Id
                            , SourceType.BrandLogo);
                        if (resources != null &&
                            resources.Count() > 0)
                        {
                            entity.Logo = resources[0].AbsoluteUrl;
                            _brandRepository.Update(entity);

                        }
                    }
                    ts.Complete();
                    return RedirectToAction("Details", new { id = entity.Id });
                }

            }

            return View(vo);
        }

        [HttpPost]
        public ActionResult Edit(FormCollection formCollection, [FetchBrand(KeyName = "id")]BrandEntity entity, BrandViewModel vo)
        {
            if (entity == null || !ModelState.IsValid)
            {
                ModelState.AddModelError("", "参数验证失败.");
                return View(vo);
            }

            entity.Name = vo.Name;
            entity.Group = vo.Group;
            entity.WebSite = vo.WebSite??string.Empty;
            entity.Description = vo.Description;
            entity.EnglishName = vo.EnglishName;
            entity.UpdatedDate = DateTime.Now;
            entity.UpdatedUser = CurrentUser.CustomerId;
            using (var ts = new TransactionScope())
            {

                if (ControllerContext.HttpContext.Request.Files.Count > 0)
                {
                    var oldImage = _resourceRepository.Get(r => r.SourceType == (int)SourceType.BrandLogo &&
                         r.SourceId == entity.Id).FirstOrDefault();
                    if (oldImage != null)
                        _resourceService.Del(oldImage.Id);
                    var resources = this._resourceService.Save(ControllerContext.HttpContext.Request.Files
                           , CurrentUser.CustomerId
                         , -1, entity.Id
                         , SourceType.BrandLogo);
                    if (resources != null &&
                        resources.Count() > 0)
                    {
                        entity.Logo = resources[0].AbsoluteUrl;
                    }

                }
                _brandRepository.Update(entity);

                ts.Complete();


            }
            return RedirectToAction("Details", new { id = entity.Id });
           
        }

        [HttpPost]
        public JsonResult Delete([FetchBrand(KeyName = "id")]BrandEntity entity)
        {
            if (entity == null)
            {
                
                return FailResponse();
            }

            entity.UpdatedDate = DateTime.Now;
            entity.UpdatedUser = CurrentUser.CustomerId;
            entity.Status = (int)DataStatus.Deleted;

            this._brandRepository.Delete(entity);

            return SuccessResponse();
        }
        [HttpGet]
        public override JsonResult AutoComplete(string name)
        {
            return Json(_brandRepository.AutoComplete(name).Where(entity => string.IsNullOrEmpty(name) ? true : entity.Name.StartsWith(name.Trim())).Take(10)
                ,JsonRequestBehavior.AllowGet);
        }
    }
}
