using System;
using System.Collections.Generic;
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
using Yintai.Hangzhou.Repository.Contract;
using Yintai.Hangzhou.Service.Contract;
using Yintai.Hangzhou.WebSupport.Binder;
using Yintai.Hangzhou.WebSupport.Mvc;

namespace Yintai.Hangzhou.Cms.WebSiteCoreV1.Controllers
{
    [AdminAuthorize]
    public class PromotionController : UserController
    {
        private readonly IPromotionRepository _promotionRepository;
        private IStoreRepository _storeRepository;
        private ITagRepository _tagRepository;
        private IResourceService _resourceService;
        public PromotionController(IPromotionRepository promotionRepository
            ,IStoreRepository storeRepository
            ,ITagRepository tagRepository
            ,IResourceService resourceService)
        {
            this._promotionRepository = promotionRepository;
            _storeRepository = storeRepository;
            _tagRepository = tagRepository;
            _resourceService = resourceService;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult List(PagerRequest request,PromotionListSearchOption search)
        {
            int totalCount;
            var storeList = new List<int>();
            if (!string.IsNullOrEmpty(search.Store))
                storeList = _storeRepository.Get(s => s.Name.StartsWith(search.Store)).Select(s=>s.Id).ToList();
 
            var data = _promotionRepository.Get(e => (!search.PId.HasValue || e.Id== search.PId.Value)
                                                     && (string.IsNullOrEmpty(search.Name) || e.Name.ToLower().StartsWith(search.Name.ToLower()))
                                                     && (!search.Status.HasValue || e.Status == (int)search.Status.Value)
                                                     && e.Status != (int)DataStatus.Deleted
                                                     && (string.IsNullOrEmpty(search.Store) || storeList.Any(m=>m==e.Store_Id))
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
                                                           case GenricOrder.OrderByCreateUser:
                                                               return e.OrderByDescending(o => o.CreatedUser);
                                                           case GenricOrder.OrderByName:
                                                               return e.OrderByDescending(o => o.Name);
                                                           case GenricOrder.OrderByCreateDate:
                                                           default:
                                                               return e.OrderByDescending(o => o.CreatedDate);

                                                       }
                                                   }
                                               });
            

            var vo = MappingManager.PromotionViewMapping(data.ToList());

            var v = new PromotionCollectionViewModel(request, totalCount) { Promotions = vo.ToList() };
            ViewBag.SearchOptions = search;
            return View("List", v);
        }

        public ActionResult Details(int? id, [FetchPromotion(KeyName = "id")]PromotionEntity entity)
        {
            if (id == null || entity == null)
            {
                ModelState.AddModelError("", "参数验证失败.");
                return View();
            }

            var vo = MappingManager.PromotionViewMapping(entity);

            return View(vo);
        }

        public ActionResult Create()
        {
            return View();
        }
        
       
        public ActionResult Edit(int? id, [FetchPromotion(KeyName = "id")]PromotionEntity entity)
        {
            if (id == null || entity == null)
            {
                ModelState.AddModelError("", "参数验证失败.");
                return View();
            }

            var vo = MappingManager.PromotionViewMapping(entity);

            return View(vo);
        }

        [HttpPost]
        public ActionResult Create(FormCollection formCollection, PromotionViewModel vo)
        {
            if (ModelState.IsValid)
            {
                var entity = MappingManager.PromotionEntityMapping(vo);
                entity.CreatedUser = base.CurrentUser.CustomerId;
                entity.UpdatedUser = base.CurrentUser.CustomerId;
                entity.RecommendSourceId = CurrentUser.CustomerId;
                entity.RecommendSourceType = (int)RecommendSourceType.Default;
                entity.RecommendUser = CurrentUser.CustomerId;
                entity.Status = (int)DataStatus.Default;
                using (TransactionScope ts = new TransactionScope())
                {

                    entity = this._promotionRepository.Insert(entity);
                    var ids = _resourceService.Save(ControllerContext.HttpContext.Request.Files
                        , CurrentUser.CustomerId
                        , -1, entity.Id
                        , SourceType.Promotion);
                    ts.Complete();
                }

                return RedirectToAction("List");
            }

            return View(vo);
        }

        [HttpPost]
        public ActionResult Edit(FormCollection formCollection, [FetchPromotion(KeyName = "id")]PromotionEntity entity, PromotionViewModel vo)
        {
            if (entity == null || !ModelState.IsValid)
            {
                ModelState.AddModelError("", "参数验证失败.");
                return View(vo);
            }


            entity.UpdatedDate = DateTime.Now;
            entity.UpdatedUser = base.CurrentUser.CustomerId;
            entity.Store_Id = vo.Store_Id;
            entity.Status = vo.Status;
            entity.IsProdBindable = vo.IsProdBindable;
            entity.IsTop = vo.IsTop;
            entity.Name = vo.Name;
            entity.PublicationLimit = vo.PublicationLimit;
            entity.StartDate = vo.StartDate;
            entity.EndDate = vo.EndDate;
            entity.Description = vo.Description;
            using (TransactionScope ts = new TransactionScope())
            {
                this._promotionRepository.Update(entity);
                if (ControllerContext.HttpContext.Request.Files.Count > 0)
                {
                    this._resourceService.Save(ControllerContext.HttpContext.Request.Files
                          , CurrentUser.CustomerId
                        , -1, entity.Id
                        , SourceType.Promotion);
                }
                ts.Complete();
            }
            return RedirectToAction("List");
        }

        [HttpPost]
        public JsonResult Delete( [FetchPromotion(KeyName = "id")]PromotionEntity entity)
        {
            try
            {

                entity.UpdatedDate = DateTime.Now;
                entity.UpdatedUser = CurrentUser.CustomerId;
                entity.Status = (int)DataStatus.Deleted;

                this._promotionRepository.Update(entity);
                return SuccessResponse();
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return FailResponse();
            }

        }
      
        [HttpGet]
        public override JsonResult AutoComplete(string name)
        {
            return Json(_promotionRepository.AutoComplete(name).Where(entity => entity.IsProdBindable.HasValue && entity.IsProdBindable.Value)
            .Where(entity => string.IsNullOrEmpty(name) ? true : entity.Name.StartsWith(name.Trim()))
            .Take(10)
                , JsonRequestBehavior.AllowGet);
        }
    }
}
