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
using Yintai.Hangzhou.Model.Filters;
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
        private IResourceRepository _resourceRepo;
        public StoreController(IStoreRepository storeRepository
            ,IResourceService resourceService
            ,IResourceRepository resourceRepo)
        {
            this._storeRepository = storeRepository;
            _resouceService = resourceService;
            _resourceRepo = resourceRepo;
        }

        public ActionResult Index(PagerRequest request, int? sort)
        {
            return List(request,sort);
        }

        public ActionResult List(PagerRequest request, int? sort)
        {
            int totalCount;
            var linq = _storeRepository.Get(e => e.Status != (int)DataStatus.Deleted
                                              , out totalCount
                                              , request.PageIndex
                                              , request.PageSize
                                              , e =>
                                              {
                                                  if (!sort.HasValue)
                                                      return e.OrderByDescending(o => o.CreatedDate);
                                                  else
                                                  {
                                                      switch (sort.Value)
                                                      {
                                                          case (int)GenericOrder.OrderByCreateUser:
                                                              return e.OrderByDescending(o => o.CreatedUser);
                                                          case (int)GenericOrder.OrderByName:
                                                              return e.OrderByDescending(o => o.Name);
                                                          case (int)GenericOrder.OrderByCreateDate:
                                                          default:
                                                              return e.OrderByDescending(o => o.CreatedDate);

                                                      }
                                                  }
                                              });
            var data = linq.GroupJoin(_resourceRepo.Get(r => r.SourceType == (int)SourceType.StoreLogo),
                            o => o.Id,
                            i => i.SourceId,
                            (o, i) => new {S = o,R = i });
            var stores = from d in data.ToList()
                         select new StoreViewModel().FromEntity<StoreViewModel>(d.S,
                         s=>s.Resources = new ResourceViewModel().FromEntities<ResourceViewModel>(d.R));

            var v = new StoreCollectionViewModel(request, totalCount) { Stores = stores.ToList() };

            return View("List", v);
        }

        public ActionResult Details(int? id, [FetchStore(KeyName = "id")]StoreEntity entity)
        {
            if (id == null || entity == null)
            {
                ModelState.AddModelError("", "参数验证失败.");
                return View();
            }
           
            var vo = new StoreViewModel().FromEntity<StoreViewModel>(entity
                ,model=>model.Resources=new ResourceViewModel().FromEntities<ResourceViewModel>(
                        _resourceRepo.Get(r=>r.SourceId==model.Id 
                            && r.SourceType == (int)SourceType.StoreLogo).ToList())
                    );
            
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

            var vo = new StoreViewModel().FromEntity<StoreViewModel>(entity
               , model => model.Resources = new ResourceViewModel().FromEntities<ResourceViewModel>(
                       _resourceRepo.Get(r => r.SourceId == model.Id
                           && r.SourceType == (int)SourceType.StoreLogo).ToList())
                   );

            return View(vo);
        }

        [HttpPost]
        public ActionResult Create(FormCollection formCollection, StoreViewModel vo)
        {
            if (ModelState.IsValid)
            {
                var entity = MappingManager.StoreEntityMapping(vo);
                entity.CreatedUser = base.CurrentUser.CustomerId;
                entity.UpdatedDate = DateTime.Now;
                entity.UpdatedUser = base.CurrentUser.CustomerId;
                entity.Status = (int)DataStatus.Normal;
                using (var ts = new TransactionScope())
                {
                    entity = this._storeRepository.Insert(entity);
                    if (ControllerContext.HttpContext.Request.Files.Count > 0)
                    {
                        _resouceService.Save(ControllerContext.HttpContext.Request.Files
                            , entity.CreatedUser
                            , 0
                            , entity.Id
                            , SourceType.StoreLogo);
                    }
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
            entity.UpdatedDate = DateTime.Now;
            entity.UpdatedUser = CurrentUser.CustomerId;
            using (TransactionScope ts = new TransactionScope())
            {
                this._storeRepository.Update(entity);
                if (ControllerContext.HttpContext.Request.Files.Count > 0)
                {
                    foreach (string fileName in ControllerContext.HttpContext.Request.Files)
                    {
                        var file = ControllerContext.HttpContext.Request.Files[fileName];
                        if (file == null || file.ContentLength == 0)
                            continue;
                        //remove existing resource
                        var resourceParts = fileName.Split('_');
                        if (resourceParts.Length > 1)
                        {
                            int resourceId = int.Parse(resourceParts[1]);
                            _resouceService.Del(resourceId);
                        }

                    }
                    //add new resource
                    _resouceService.Save(ControllerContext.HttpContext.Request.Files
                          , CurrentUser.CustomerId
                        , -1, entity.Id
                        , SourceType.StoreLogo);
                }
                ts.Complete();
            }
          


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

            this._storeRepository.Update(entity);

            return SuccessResponse();
        }
        [HttpGet]
        public override JsonResult AutoComplete(string name)
        {
            return Json(_storeRepository.AutoComplete(name).Where(entity=>entity.Status!=(int)DataStatus.Deleted && 
                                                                        (string.IsNullOrEmpty(name)?true:entity.Name.StartsWith(name.Trim()))).Take(10)
                , JsonRequestBehavior.AllowGet);
        }
        [OutputCache(NoStore=false,Duration=0)]
        [HttpGet]
        public  JsonResult Detail(int id)
        {
            return Json(_storeRepository.Find(id),JsonRequestBehavior.AllowGet);
        }
    }
}
