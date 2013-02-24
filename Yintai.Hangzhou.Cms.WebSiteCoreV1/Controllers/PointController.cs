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

        public ActionResult Index(PagerRequest request, int? sort)
        {
            return List(request, sort);
        }

        public ActionResult List(PagerRequest request, int? sort)
        {
            int totalCount;
            var sortOrder = (PointSortOrder)(sort ?? 0);

            var data = _pointRepository.GetPagedList(request, out totalCount, sortOrder);
            var vo = MappingManager.PointViewMapping(data);

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

        public ActionResult Delete(int? id, [FetchPoint(KeyName = "id")]PointHistoryEntity entity)
        {
            if (id == null || entity == null)
            {
                ModelState.AddModelError("", "参数验证失败.");
                return View();
            }

            var vo = MappingManager.PointViewMapping(entity);

            return View(vo);
        }

        public ActionResult Edit(int? id, [FetchPoint(KeyName = "id")]PointHistoryEntity entity)
        {
            if (id == null || entity == null)
            {
                ModelState.AddModelError("", "参数验证失败.");
                return View();
            }

            var vo = MappingManager.PointViewMapping(entity);

            return View(vo);
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

                entity = this._pointRepository.Insert(entity);
                //需要关联到 用户账户
                try
                {
                    if (vo.Amount > 0)
                    {
                        _userService.AddPoint(vo.User_Id, (int)vo.Amount, base.CurrentUser.CustomerId);
                    }
                    else
                    {
                        _userService.SubPoint(vo.User_Id, (int)vo.Amount, base.CurrentUser.CustomerId);
                    }
                }
                catch (Exception ex)
                {
                    //回滚

                    this._pointRepository.Delete(entity);

                    throw;
                }

                return Success("/" + RouteData.Values["controller"] + "/edit/" + entity.Id.ToString(CultureInfo.InvariantCulture));
            }

            return View(vo);
        }

        [HttpPost]
        public ActionResult Edit(FormCollection formCollection, [FetchPoint(KeyName = "id")]PointHistoryEntity entity, PointViewModel vo)
        {
            if (entity == null || !ModelState.IsValid)
            {
                ModelState.AddModelError("", "参数验证失败.");
                return View(vo);
            }

            if ((int)entity.Amount != (int)vo.Amount)
            {
                ModelState.AddModelError("", "参数验证失败.数量不能修改，请删除后重新添加");
                return View(vo);
            }

            var newEntity = MappingManager.PointHistoryEntityMapping(vo);
            newEntity.CreatedUser = entity.CreatedUser;
            newEntity.CreatedDate = entity.CreatedDate;
            newEntity.Status = entity.Status;

            MappingManager.PointHistoryEntityMapping(newEntity, entity);

            this._pointRepository.Update(entity);

            return Success("/" + RouteData.Values["controller"] + "/details/" + entity.Id.ToString(CultureInfo.InvariantCulture));
        }

        [HttpPost]
        public ActionResult Delete(FormCollection formCollection, [FetchPoint(KeyName = "id")]PointHistoryEntity entity)
        {
            if (entity == null)
            {
                ModelState.AddModelError("", "参数验证失败.");
                return View();
            }


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

            try
            {
                _pointRepository.Delete(entity);
            }
            catch (Exception ex)
            {
                //回滚

                if (entity.Amount > 0)
                {
                    _userService.AddPoint(entity.User_Id, (int)entity.Amount, base.CurrentUser.CustomerId);
                }
                else
                {
                    _userService.SubPoint(entity.User_Id, (int)entity.Amount, base.CurrentUser.CustomerId);
                }

                throw;
            }



            return Success("/" + RouteData.Values["controller"] + "/" + RouteData.Values["action"]);
        }
    }
}
