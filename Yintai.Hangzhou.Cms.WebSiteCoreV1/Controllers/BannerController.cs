using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Web.Mvc;
using Yintai.Architecture.Common.Models;
using Yintai.Hangzhou.Cms.WebSiteCoreV1.Models;
using Yintai.Hangzhou.Data.Models;
using Yintai.Hangzhou.Model.Enums;
using Yintai.Hangzhou.Model.Filters;
using Yintai.Hangzhou.Repository.Contract;
using Yintai.Hangzhou.Service.Contract;

namespace Yintai.Hangzhou.Cms.WebSiteCoreV1.Controllers
{
    public class BannerController:UserController
    {
        private IBannerRepository _bannerRepo;
        private IPromotionRepository _proRepo;
        private IResourceRepository _resourceRepo;
        private IResourceService _resourceSer;
        public BannerController(IBannerRepository bannerRepo,
            IPromotionRepository proRepo,
            IResourceRepository resourceRepo,
            IResourceService resourceSer)
        {
            _bannerRepo = bannerRepo;
            _proRepo = proRepo;
            _resourceRepo = resourceRepo;
            _resourceSer = resourceSer;
        }
        public ActionResult Index(PagerRequest request, BannerSearchOption search)
        {
            return List(request,search);
        }

        public ActionResult List(PagerRequest request, BannerSearchOption search)
        {
            int totalCount;
            var linq = _bannerRepo.Get(e => (!search.PromotionId.HasValue || (e.SourceId == search.PromotionId.Value && e.SourceType == (int)SourceType.Promotion))
                                                     && (!search.Status.HasValue || e.Status == (int)search.Status.Value)
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
                                                           case GenericOrder.OrderByCreateDate:
                                                           default:
                                                               return e.OrderByDescending(o => o.CreatedDate);

                                                       }
                                                   }
                                               });

           var linq_All =  linq.Join(_proRepo.GetAll(),
                o => o.SourceId,
                i => i.Id,
                (o, i) => new { B = o, P = i })
                .GroupJoin(_resourceRepo.Get(r => r.SourceType == (int)SourceType.BannerPromotion),
                o => o.B.Id,
                i => i.SourceId,
                (o, i) => new { B = o.B, P = o.P, R = i.FirstOrDefault() });
            var vo = from l in linq_All.ToList()
                     select new BannerViewModel() { 
                         Id = l.B.Id,
                          SourceId = l.B.SourceId,
                          SourceType = l.B.SourceType,
                          Resource  = MappingManager.ResourceViewMapping(l.R),
                          Promotion = MappingManager.PromotionViewMapping(l.P),
                          SortOrder = l.B.SortOrder,
                           Status = l.B.Status
                     };

            var v = new BannerCollectionViewModel(request, totalCount) { Banners = vo.ToList() };
            ViewBag.SearchOptions = search;
            return View("List", v);
        }
        private BannerViewModel findBannerByid(int id)
        {
            var entity = _bannerRepo.Get(b => b.Id == id && b.SourceType == (int)SourceType.Promotion)
               .Join(_proRepo.GetAll(), o => o.SourceId, i => i.Id, (o, i) => new { B = o, P = i })
               .GroupJoin(_resourceRepo.Get(r => r.SourceType == (int)SourceType.BannerPromotion),
                      o => o.B.Id,
                      i => i.SourceId,
                      (o, i) => new { B = o.B, P = o.P, R = i.FirstOrDefault() });
            var vo = from l in entity.ToList()
                     select new BannerViewModel()
                     {
                         Id = l.B.Id,
                         SourceId = l.B.SourceId,
                         SourceType = l.B.SourceType,
                         Resource = MappingManager.ResourceViewMapping(l.R),
                         Promotion = MappingManager.PromotionViewMapping(l.P),
                         SortOrder = l.B.SortOrder,
                         Status = l.B.Status
                     };
            return vo.FirstOrDefault();
        }
        public ActionResult Details(int? id)
        {
            
            if (id == null)
            {
                ModelState.AddModelError("", "参数验证失败.");
                return View();
            }
            var banner = findBannerByid(id.Value);
            return View(banner);
        }

        public ActionResult Create()
        {
            return View();
        }


        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                ModelState.AddModelError("", "参数验证失败.");
                return View();
            }

            var banner = findBannerByid(id.Value);
            return View(banner);
        }

        [HttpPost]
        public ActionResult Create(FormCollection formCollection, BannerViewModel vo)
        {
            if (ModelState.IsValid)
            {
                var entity = new BannerEntity();
                entity.CreatedUser = base.CurrentUser.CustomerId;
                entity.UpdatedUser = base.CurrentUser.CustomerId;
                entity.CreatedDate = DateTime.Now;
                entity.UpdatedDate = DateTime.Now;
                entity.SourceId = vo.SourceId;
                entity.SourceType = (int)SourceType.Promotion;
                entity.SortOrder = vo.SortOrder;
                
                entity.Status = (int)DataStatus.Default;
                
                using (TransactionScope ts = new TransactionScope())
                {

                    entity = this._bannerRepo.Insert(entity);
                    var ids = _resourceSer.Save(ControllerContext.HttpContext.Request.Files
                        , CurrentUser.CustomerId
                        , -1, vo.SourceId //souurceId 请使用 promotionid
                        , SourceType.BannerPromotion);
                    ts.Complete();
                }

                return RedirectToAction("Edit", new {id=entity.Id });
            }

            return View(vo);
        }

        [HttpPost]
        public ActionResult Edit(FormCollection formCollection, BannerViewModel vo)
        {
            var entity = _bannerRepo.GetItem(vo.Id);
            if (!ModelState.IsValid || entity==null)
            {
                ModelState.AddModelError("", "参数验证失败.");
                return View(vo);
            }
           

            entity.UpdatedDate = DateTime.Now;
            entity.UpdatedUser = base.CurrentUser.CustomerId;
            entity.Status = vo.Status;
            entity.SortOrder = vo.SortOrder;
            entity.SourceId = vo.SourceId;
         
            using (TransactionScope ts = new TransactionScope())
            {
                _bannerRepo.Update(entity);
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
                            _resourceSer.Del(resourceId);
                        }

                    }
                    //add new resource
                    _resourceSer.Save(ControllerContext.HttpContext.Request.Files
                          , CurrentUser.CustomerId
                        , -1, entity.SourceId //souurceId 请使用 promotionid
                        , SourceType.BannerPromotion);
                }
                ts.Complete();
            }
            return RedirectToAction("Details", new { id=entity.Id});
        }

        [HttpPost]
        public JsonResult Delete(int? id)
        {
            try
            {
                var entity = _bannerRepo.GetItem(id.Value);
                entity.UpdatedDate = DateTime.Now;
                entity.UpdatedUser = CurrentUser.CustomerId;
                entity.Status = (int)DataStatus.Deleted;

                this._bannerRepo.Update(entity);
                return SuccessResponse();
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return FailResponse();
            }

        }
    }
}
