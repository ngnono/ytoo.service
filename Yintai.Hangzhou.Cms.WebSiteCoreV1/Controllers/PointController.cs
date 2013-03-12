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
    public class PointController : UserController
    {
        private readonly IPointRepository _pointRepository;
        private readonly IUserService _userService;
        public PointController(IPointRepository pointRepository, IUserService userService)
        {
            _pointRepository = pointRepository;
            _userService = userService;
        }

        public ActionResult Index(PagerRequest request, PointListSearchOption search)
        {
            return List(request, search);
        }

        public ActionResult List(PagerRequest request, PointListSearchOption search)
        {
            int totalCount;
            var data = _pointRepository.Get(e => (!search.UId.HasValue || e.User_Id == search.UId.Value)
                                                  && e.Status != (int)DataStatus.Deleted
                                                  && (!search.PSourceType.HasValue || e.PointSourceType == (int)search.PSourceType.Value)
                                                  && (!search.PSourceId.HasValue || e.PointSourceId == (int)search.PSourceId.Value)
                                                  && (!search.PType.HasValue || e.Type == (int)search.PType.Value)

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
            
            var vo = MappingManager.PointViewMapping(data.ToList());

            var v = new PointCollectionViewModel(request, totalCount) { Points = vo.ToList() };

            return View("List", v);
        }

        public ActionResult Details(int? id, [FetchPoint(KeyName = "id")]PointHistoryEntity entity)
        {
            if (id == null || entity == null)
            {
                ModelState.AddModelError("", "参数验证失败.");
                return View();
            }

            var vo = MappingManager.PointViewMapping(entity);

            return View(vo);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(FormCollection formCollection, PointViewModel vo)
        {
            if (ModelState.IsValid)
            {
                var entity = MappingManager.PointHistoryEntityMapping(vo);
                entity.CreatedUser = base.CurrentUser.CustomerId;
                entity.UpdatedUser = base.CurrentUser.CustomerId;
                entity.Status = (int)DataStatus.Normal;
                using (var ts = new TransactionScope())
                {
                    entity = this._pointRepository.Insert(entity);

                    if (vo.Amount > 0)
                    {
                        _userService.AddPoint(vo.User_Id, (int)vo.Amount, base.CurrentUser.CustomerId);
                    }
                    else
                    {
                        _userService.SubPoint(vo.User_Id, (int)vo.Amount, base.CurrentUser.CustomerId);
                    }
                    ts.Complete();
                }

                return RedirectToAction("Details", new { id = entity.Id });
            }

            return View(vo);
        }

     
        [HttpPost]
        public JsonResult Delete([FetchPoint(KeyName = "id")]PointHistoryEntity entity)
        {
            if (entity == null)
            {
                return FailResponse();
            }

            using (var ts = new TransactionScope())
            {
                if (entity.Amount > 0)
                {
                    _userService.SubPoint(entity.User_Id, (int)entity.Amount, base.CurrentUser.CustomerId);
                }
                else
                {
                    _userService.AddPoint(entity.User_Id, (int)entity.Amount, base.CurrentUser.CustomerId);
                }

                entity.UpdatedDate = DateTime.Now;
                entity.UpdatedUser = CurrentUser.CustomerId;
                entity.Status = (int)DataStatus.Deleted;
                _pointRepository.Delete(entity);
                ts.Complete();
            }
            return SuccessResponse();
        }
    }
}
