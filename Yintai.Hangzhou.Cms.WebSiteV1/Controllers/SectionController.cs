using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Yintai.Architecture.Common.Models;
using Yintai.Hangzhou.Cms.WebSiteV1.Models;
using Yintai.Hangzhou.Cms.WebSiteV1.Util;
using Yintai.Hangzhou.Data.Models;
using Yintai.Hangzhou.Model.Enums;
using Yintai.Hangzhou.Repository.Contract;

namespace Yintai.Hangzhou.Cms.WebSiteV1.Controllers
{
    [AdminAuthorize]
    public class SectionController:UserController
    {
        private ISectionRepository _sectionRepo;
        private IBrandRepository _brandRepo;
        private IStoreRepository _storeRepo;
        public SectionController(ISectionRepository sectionRepo,
            IStoreRepository storeRepo,
            IBrandRepository brandRepo)
        {
            _sectionRepo = sectionRepo;
            _brandRepo = brandRepo;
            _storeRepo = storeRepo;
        }
        public ActionResult Index(PagerRequest request, SectionSearchOption search)
        {
            return List(request, search);
        }

        public ActionResult List(PagerRequest request,SectionSearchOption search)
        {
            int totalCount;
            var linq = _sectionRepo.Get(s => (!search.SId.HasValue || search.SId.Value == s.Id) &&
                                          (string.IsNullOrEmpty(search.Name) || s.Name.StartsWith(search.Name)) &&
                                          (!search.BrandId.HasValue || search.BrandId.Value == s.BrandId) &&
                                          (!search.StoreId.HasValue || search.StoreId.Value == s.StoreId) &&
                                          s.Status!=(int)DataStatus.Deleted
                                          , out totalCount
                                                , request.PageIndex
                                                , request.PageSize
                                                , e => e.OrderByDescending(o => o.CreateDate));
            var data = linq.Join(_storeRepo.GetAll(), o => o.StoreId, i => i.Id, (o, i) => new { S = o, Store = i })
                .Join(_brandRepo.GetAll(), o => o.S.BrandId, i => i.Id, (o, i) => new { S = o.S, Store = o.Store, B = i })
                .ToList()
                .Select(l => new SectionViewModel().FromEntity<SectionViewModel>(l.S, p=>{
                    p.Store = new StoreViewModel().FromEntity<StoreViewModel>(l.Store);
                    p.Brand = new BrandViewModel().FromEntity<BrandViewModel>(l.B);
                }));


            var v = new Pager<SectionViewModel>(request, totalCount) { Data = data.ToList() };

            return View("List", v);
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                ModelState.AddModelError("", "参数验证失败.");
                return View();
            }
            var section = _sectionRepo.Get(s => s.Id == id.Value && s.Status != (int)DataStatus.Deleted)
                .Join(_storeRepo.GetAll(), o => o.StoreId, i => i.Id, (o, i) => new { S = o, Store = i })
                .Join(_brandRepo.GetAll(), o => o.S.BrandId, i => i.Id, (o, i) => new { S = o.S, Store = o.Store, B = i }).FirstOrDefault();
            var vo = new SectionViewModel().FromEntity<SectionViewModel>(section.S, p =>
            {
                p.Store = new StoreViewModel().FromEntity<StoreViewModel>(section.Store);
                p.Brand = new BrandViewModel().FromEntity<BrandViewModel>(section.B);
            });

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
                ModelState.AddModelError("", "参数验证失败.");
                return View();
            }

            var vo = new SectionViewModel().FromEntity<SectionViewModel>(_sectionRepo.Find(id.Value));
                   

            return View(vo);
        }

        [HttpPost]
        public ActionResult Create(FormCollection formCollection, SectionViewModel vo)
        {
            if (!ModelState.IsValid)
            {
                return View(vo);
            }
            var entity = vo.ToEntity<SectionEntity>();
            entity.CreateUser = base.CurrentUser.CustomerId;
            entity.CreateDate = DateTime.Now;
            entity.UpdateDate = DateTime.Now;
            entity.UpdateUser = CurrentUser.CustomerId;
            entity.Status = (int)DataStatus.Normal;
            _sectionRepo.Insert(entity);

            return RedirectToAction("Edit", new { id = entity.Id });

        }

        [HttpPost]
        public ActionResult Edit(FormCollection formCollection, SectionViewModel vo)
        {
            if (!ModelState.IsValid)
            {
                return View(vo);
            }
            var entity = _sectionRepo.Find(vo.Id);
            entity.Name = vo.Name;
            entity.Location = vo.Location;
            entity.StoreId = vo.StoreId;
            entity.BrandId = vo.BrandId;
            entity.ContactPerson = vo.ContactPerson;
            entity.ContactPhone = vo.ContactPhone;

            entity.UpdateDate = DateTime.Now;
            entity.UpdateUser = CurrentUser.CustomerId;

            _sectionRepo.Update(entity);

            return RedirectToAction("details", new { id = entity.Id });
        }

        [HttpPost]
        public JsonResult Delete(int id)
        {
            var entity = _sectionRepo.Find(id);
            if (entity == null)
            {
                ModelState.AddModelError("", "参数验证失败.");
                return FailResponse();
            }

            entity.UpdateDate = DateTime.Now;
            entity.UpdateUser = CurrentUser.CustomerId;
            entity.Status = (int)DataStatus.Deleted;

            _sectionRepo.Update(entity);

            return SuccessResponse();
        }
        [HttpGet]
        public override JsonResult AutoComplete(string name)
        {
            return Json(_sectionRepo.AutoComplete(name)
                        .Where(entity => entity.Status != (int)DataStatus.Deleted &&
                                        (string.IsNullOrEmpty(name) ? true : entity.Name.StartsWith(name.Trim())))
                       .Take(10)
                , JsonRequestBehavior.AllowGet);
        }
    }
}