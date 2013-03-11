using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Web.Mvc;
using Yintai.Architecture.Common.Models;
using Yintai.Hangzhou.Cms.WebSiteCoreV1.Manager;
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
    public class StoreController : UserController
    {
        private readonly IStoreRepository _storeRepository;
        private IResourceService _resouceService;
        public StoreController(IStoreRepository storeRepository
            ,IResourceService resourceService)
        {
            this._storeRepository = storeRepository;
            _resouceService = resourceService;
        }

        public ActionResult Index(PagerRequest request, int? sort)
        {
            return List(request,sort);
        }

        public ActionResult List(PagerRequest request, int? sort)
        {
            int totalCount;
            var sortOrder = (StoreSortOrder)(sort ?? 0);

            var data = _storeRepository.GetPagedList(request, out totalCount, sortOrder);
            var vo = MappingManager.StoreViewMapping(data);

            var v = new StoreCollectionViewModel(request, totalCount) { Stores = vo.ToList() };

            return View("List", v);
        }

        public ActionResult Details(int? id, [FetchStore(KeyName = "id")]StoreEntity entity)
        {
            if (id == null || entity == null)
            {
                ModelState.AddModelError("", "参数验证失败.");
                return View();
            }

            var vo = MappingManager.StoreViewMapping(entity);

            return View(vo);
        }

        public ActionResult Create()
        {
            return View();
        }

   
        public ActionResult Edit(int? id, [FetchStore(KeyName = "id")]StoreEntity entity)
        {
            if (id == null || entity == null)
            {
                ModelState.AddModelError("", "参数验证失败.");
                return View();
            }

            var vo = MappingManager.StoreViewMapping(entity);

            return View(vo);
        }

        [HttpPost]
        public ActionResult Create(FormCollection formCollection, StoreViewModel vo)
        {
            if (ModelState.IsValid)
            {
                var entity = MappingManager.StoreEntityMapping(vo);
                entity.CreatedUser = base.CurrentUser.CustomerId;
                entity.UpdatedUser = base.CurrentUser.CustomerId;
                entity.Status = (int)DataStatus.Normal;
                using (var ts = new TransactionScope())
                {
                    entity = this._storeRepository.Insert(entity);
                    _resouceService.Save(ControllerContext.HttpContext.Request.Files
                        , entity.CreatedUser
                        , 0
                        , entity.Id
                        , SourceType.StoreLogo);
                    ts.Complete();
                }

                return RedirectToAction("Edit",new {id=entity.Id});
            }

            return View(vo);
        }

        [HttpPost]
        public ActionResult Edit(FormCollection formCollection, [FetchStore(KeyName = "id")]StoreEntity entity, StoreViewModel vo)
        {
            if (entity == null || !ModelState.IsValid)
            {
                ModelState.AddModelError("", "参数验证失败.");
                return View(vo);
            }

            entity.GpsAlt = vo.GpsAlt;
            entity.GpsLat = vo.GpsLat;
            entity.GpsLng = vo.GpsLng;
            entity.Latitude = vo.Latitude;
            entity.Longitude = vo.Longitude;
            entity.Name = vo.Name;
            entity.Tel = vo.Tel;
            entity.Description = vo.Description;
            entity.Longitude = vo.Longitude;

            this._storeRepository.Update(entity);


            return RedirectToAction("details",new {id=entity.Id});
        }

        [HttpPost]
        public JsonResult Delete([FetchStore(KeyName = "id")]StoreEntity entity)
        {
            if (entity == null)
            {
                ModelState.AddModelError("", "参数验证失败.");
                return FailResponse();
            }

            entity.UpdatedDate = DateTime.Now;
            entity.UpdatedUser = CurrentUser.CustomerId;
            entity.Status = (int)DataStatus.Deleted;

            this._storeRepository.Delete(entity);

            return SuccessResponse();
        }
        [HttpGet]
        public override JsonResult AutoComplete(string name)
        {
            return Json(_storeRepository.AutoComplete(name).Where(entity=>string.IsNullOrEmpty(name)?true:entity.Name.StartsWith(name.Trim())).Take(10)
                , JsonRequestBehavior.AllowGet);
        }
    }
}
