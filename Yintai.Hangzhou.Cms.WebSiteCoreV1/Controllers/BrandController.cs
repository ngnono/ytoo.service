using System;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using Yintai.Architecture.Common.Models;
using Yintai.Hangzhou.Cms.WebSiteCoreV1.Manager;
using Yintai.Hangzhou.Cms.WebSiteCoreV1.Models;
using Yintai.Hangzhou.Cms.WebSiteCoreV1.Util;
using Yintai.Hangzhou.Data.Models;
using Yintai.Hangzhou.Model.Enums;
using Yintai.Hangzhou.Repository.Contract;
using Yintai.Hangzhou.WebSupport.Binder;
using Yintai.Hangzhou.WebSupport.Mvc;

namespace Yintai.Hangzhou.Cms.WebSiteCoreV1.Controllers
{
    [AdminAuthorize]
    public class BrandController : UserController
    {
        private readonly IBrandRepository _brandRepository;
        public BrandController(IBrandRepository brandRepository)
        {
            this._brandRepository = brandRepository;
        }

        public ActionResult Index(PagerRequest request, int? sort)
        {
            return List(request, sort);
        }

        public ActionResult List(PagerRequest request, int? sort)
        {
            int totalCount;
            var sortOrder = (BrandSortOrder)(sort ?? 0);

            var data = _brandRepository.GetPagedList(request, out totalCount, sortOrder);
            var vo = MappingManager.BrandViewMapping(data);

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
                entity.Status = (int)DataStatus.Normal;
                entity.CreatedDate = DateTime.Now;
                entity.UpdatedDate = DateTime.Now;

                entity = this._brandRepository.Insert(entity);

                return Success("/" + RouteData.Values["controller"] + "/edit/" + entity.Id.ToString(CultureInfo.InvariantCulture));
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

            var newEntity = MappingManager.BrandEntityMapping(vo);
            newEntity.CreatedUser = entity.CreatedUser;
            newEntity.CreatedDate = entity.CreatedDate;
            newEntity.Status = entity.Status;
            newEntity.UpdatedDate = DateTime.Now;
            newEntity.UpdatedUser = base.CurrentUser.CustomerId;

            MappingManager.BrandEntityMapping(newEntity, entity);

            this._brandRepository.Update(entity);


            return Success("/" + RouteData.Values["controller"] + "/details/" + entity.Id.ToString(CultureInfo.InvariantCulture));
        }

        [HttpPost]
        public ActionResult Delete(FormCollection formCollection, [FetchBrand(KeyName = "id")]BrandEntity entity)
        {
            if (entity == null)
            {
                ModelState.AddModelError("", "参数验证失败.");
                return View();
            }

            entity.UpdatedDate = DateTime.Now;
            entity.UpdatedUser = CurrentUser.CustomerId;
            entity.Status = (int)DataStatus.Deleted;

            this._brandRepository.Delete(entity);

            return Success("/" + RouteData.Values["controller"] + "/" + RouteData.Values["action"]);
        }
        [HttpGet]
        public override JsonResult AutoComplete(string name)
        {
            return Json(_brandRepository.AutoComplete(name).Where(entity => string.IsNullOrEmpty(name) ? true : entity.Name.StartsWith(name.Trim())).Take(10)
                ,JsonRequestBehavior.AllowGet);
        }
    }
}
