using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Web.Mvc;
using Yintai.Architecture.Common.Models;
using Yintai.Hangzhou.Cms.WebSiteCoreV1.Models;
using Yintai.Hangzhou.Cms.WebSiteCoreV1.Util;
using Yintai.Hangzhou.Data.Models;
using Yintai.Hangzhou.Model.Enums;
using Yintai.Hangzhou.Model.Filters;
using Yintai.Hangzhou.Repository.Contract;

namespace Yintai.Hangzhou.Cms.WebSiteCoreV1.Controllers
{
    [AdminAuthorize]
    public class StorePromotionController : UserController
    {
        private IStorePromotionRepository _storepromotionRepo;
        private IPointOrderRuleRepository _ruleRepo;
        private IStorePromotionScopeRepository _scopeRepo;

        public StorePromotionController(IStorePromotionRepository storepromotionRepo,
            IPointOrderRuleRepository ruleRepo,
            IStorePromotionScopeRepository scopeRepo)
        {
            _storepromotionRepo = storepromotionRepo;
            _scopeRepo = scopeRepo;
            _ruleRepo = ruleRepo;
        }
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult List(PagerRequest request, StorePromotionSearchOption search)
        {
            int totalCount;
            var dbContext = _storepromotionRepo.Context;
            var linq = from e in dbContext.Set<StorePromotionEntity>()
                       let scope = from es in dbContext.Set<StorePromotionScopeEntity>()
                                   where es.StorePromotionId == e.Id && es.StoreId == search.StoreId.Value
                                   select es

                        where (!search.SId.HasValue || e.Id == search.SId.Value)
                                && (string.IsNullOrEmpty(search.Name) || e.Name.ToLower().StartsWith(search.Name.ToLower()))
                                && (!search.Status.HasValue || e.Status == (int)search.Status.Value)
                                && (!search.ActiveStartDate.HasValue || e.ActiveStartDate >= search.ActiveStartDate.Value)
                                && (!search.ActiveEndDate.HasValue || e.ActiveEndDate >= search.ActiveEndDate.Value)
                                && e.Status != (int)DataStatus.Deleted
                                && (!search.StoreId.HasValue || scope.Any())
                        select e;
            linq = linq.WhereWithPageSort<StorePromotionEntity>(
                                                 out totalCount
                                                , request.PageIndex
                                                , request.PageSize
                                                , e =>
                                                {
                                                    if (!search.SortBy.HasValue)
                                                        return e.OrderByDescending(o => o.CreateDate);
                                                    else
                                                    {
                                                        switch (search.SortBy.Value)
                                                        {
                                                            case GenericOrder.OrderByCreateUser:
                                                                return e.OrderByDescending(o => o.CreateUser);
                                                            case GenericOrder.OrderByName:
                                                                return e.OrderByDescending(o => o.Name);
                                                            case GenericOrder.OrderByCreateDate:
                                                            default:
                                                                return e.OrderByDescending(o => o.CreateDate);

                                                        }
                                                    }
                                                });

            var vo = from l in linq.ToList()
                     select new StorePromotionViewModel().FromEntity<StorePromotionViewModel>(l);

            var v = new Pager<StorePromotionViewModel>(request, totalCount) { Data = vo.ToList() };
            ViewBag.SearchOptions = search;
            return View("List", v);
        }


        public ActionResult Detail(int? id)
        {
            if (id == null)
            {
                ModelState.AddModelError("", "参数验证失败.");
                return View();
            }
            var entity = _storepromotionRepo.Find(id.Value);
            var vo = new StorePromotionViewModel().FromEntity<StorePromotionViewModel>(entity
                , model => model.Rules = new StorePromotionRuleViewModel().FromEntities<StorePromotionRuleViewModel>(
                        _ruleRepo.Get(r => r.StorePromotionId == model.Id
                            && r.Status != (int)DataStatus.Deleted).ToList())
                    );

            return View(vo);
        }

        public ActionResult Create()
        {
            return View();
        }


        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return View();
            }
            var entity = _storepromotionRepo.Find(id.Value);
            var vo = new StorePromotionViewModel().FromEntity<StorePromotionViewModel>(entity
                , model =>
                {
                    model.Rules = new StorePromotionRuleViewModel().FromEntities<StorePromotionRuleViewModel>(
                        _ruleRepo.Get(r => r.StorePromotionId == model.Id
                            && r.Status != (int)DataStatus.Deleted).ToList());
                    model.Scope = new StorePromotionScopeViewModel().FromEntities<StorePromotionScopeViewModel>(
                        _scopeRepo.Get(r => r.StorePromotionId == model.Id
                            && r.Status != (int)DataStatus.Deleted).ToList()
                        );
                }
                    );

            return View(vo);
        }

        [HttpPost]
        public ActionResult Create(FormCollection formCollection, StorePromotionViewModel vo)
        {
            if (!ModelState.IsValid)
            {
                return View(vo);
            }
            var entity = vo.ToEntity<StorePromotionEntity>();
            entity.CreateDate = DateTime.Now;
            entity.CreateUser = base.CurrentUser.CustomerId;
            entity.UpdateDate = DateTime.Now;
            entity.UpdateUser = base.CurrentUser.CustomerId;
            entity.Status = (int)DataStatus.Default;
            entity.InScopeNotice = vo.ComposedScopeNotice;
            var scopeEntities = vo.Scope.Where(s => s.Status != (int)DataStatus.Deleted)
                                    .Select(s => s.ToEntity<StorePromotionScopeEntity>(se =>
                                    {
                                        se.Status = (int)DataStatus.Normal;
                                        se.UpdateUser = CurrentUser.CustomerId;
                                        se.UpdateDate = DateTime.Now;
                                        se.CreateDate = DateTime.Now;
                                        se.CreateUser = CurrentUser.CustomerId;
                                    }));
            var rulEntities = vo.Rules.Where(s => s.Status != (int)DataStatus.Deleted)
                            .Select(s => s.ToEntity<PointOrderRuleEntity>(sp =>
                            {
                                sp.Status = (int)DataStatus.Normal;
                                sp.UpdateUser = CurrentUser.CustomerId;
                                sp.UpdateDate = DateTime.Now;
                                sp.CreateDate = DateTime.Now;
                                sp.CreateUser = CurrentUser.CustomerId;
                            }));

            using (var ts = new TransactionScope())
            {
                entity = _storepromotionRepo.Insert(entity);
                foreach (var scopeE in scopeEntities)
                {
                    scopeE.StorePromotionId = entity.Id;
                    _scopeRepo.Insert(scopeE);
                }
                foreach (var ruleE in rulEntities)
                {
                    ruleE.StorePromotionId = entity.Id;
                    _ruleRepo.Insert(ruleE);
                }
                ts.Complete();
            }

            return RedirectToAction("Edit", new { id = entity.Id });

        }

        [HttpPost]
        public ActionResult Edit(FormCollection formCollection, StorePromotionViewModel vo)
        {
            if (!ModelState.IsValid)
            {
                return View(vo);
            }

            var entity = _storepromotionRepo.Find(vo.Id);
            entity.Name = vo.Name;
            entity.Description = vo.Description;
            entity.CouponStartDate = vo.CouponStartDate;
            entity.CouponEndDate = vo.CouponEndDate;
            entity.MinPoints = vo.MinPoints;
            entity.Notice = vo.Notice;
            entity.Status = vo.Status;
            entity.UsageNotice = vo.UsageNotice;
            entity.InScopeNotice = vo.ComposedScopeNotice;
            entity.PromotionType = vo.PromotionType;
            entity.AcceptPointType = vo.AcceptPointType;
            entity.ActiveStartDate = vo.ActiveStartDate;
            entity.ActiveEndDate = vo.ActiveEndDate;
            entity.UpdateDate = DateTime.Now;
            entity.UpdateUser = base.CurrentUser.CustomerId;

            var scopeEntities = vo.Scope.Where(s => s.Status != (int)DataStatus.Deleted)
                                    .Select(s => s.ToEntity<StorePromotionScopeEntity>(se =>
                                    {
                                        se.Status = (int)DataStatus.Normal;
                                        se.UpdateUser = CurrentUser.CustomerId;
                                        se.UpdateDate = DateTime.Now;
                                        se.CreateDate = DateTime.Now;
                                        se.CreateUser = CurrentUser.CustomerId;
                                    }));
            var rulEntities = vo.Rules.Where(s => s.Status != (int)DataStatus.Deleted)
                            .Select(s => s.ToEntity<PointOrderRuleEntity>(sp =>
                            {
                                sp.Status = (int)DataStatus.Normal;
                                sp.UpdateUser = CurrentUser.CustomerId;
                                sp.UpdateDate = DateTime.Now;
                                sp.CreateDate = DateTime.Now;
                                sp.CreateUser = CurrentUser.CustomerId;
                            }));

            using (var ts = new TransactionScope())
            {
                _storepromotionRepo.Update(entity);
                var oldScopes = _scopeRepo.Get(s => s.StorePromotionId == entity.Id && s.Status != (int)DataStatus.Deleted);
                foreach (var oscope in oldScopes)
                    _scopeRepo.Delete(oscope);
                foreach (var scopeE in scopeEntities)
                {
                    scopeE.StorePromotionId = entity.Id;
                    _scopeRepo.Insert(scopeE);
                }
                var oldRules = this._ruleRepo.Get(s => s.StorePromotionId == entity.Id && s.Status != (int)DataStatus.Deleted);
                foreach (var orule in oldRules)
                    _ruleRepo.Delete(orule);
                foreach (var ruleE in rulEntities)
                {
                    ruleE.StorePromotionId = entity.Id;
                    _ruleRepo.Insert(ruleE);
                }
                ts.Complete();
            }

            return RedirectToAction("detail", new { id = entity.Id });
        }

        [HttpPost]
        public JsonResult Delete(int? id)
        {
            if (!id.HasValue)
                throw new ApplicationException("internal error");
            var entity = _storepromotionRepo.Find(id.Value);

            entity.UpdateDate = DateTime.Now;
            entity.UpdateUser = CurrentUser.CustomerId;
            entity.Status = (int)DataStatus.Deleted;

            this._storepromotionRepo.Update(entity);

            return SuccessResponse();
        }

    }
}
