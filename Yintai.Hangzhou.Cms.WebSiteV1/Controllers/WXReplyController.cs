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
    public class WXReplyController:UserController
    {
        private IWXReplyRepository _wxRepo;
        public WXReplyController(IWXReplyRepository wxRepo)
        {
            _wxRepo = wxRepo;
        }
        public ActionResult List(PagerRequest request)
        {
            int totalCount;
           
            var data = _wxRepo.Get(w=>w.Status!=(int)DataStatus.Deleted,
                            out totalCount,
                            request.PageIndex,
                            request.PageSize,
                            w=>w.OrderByDescending(wi=>wi.UpdateDate));
            var vo = data.ToList().Select(d => new WXReplyViewModel().FromEntity<WXReplyViewModel>(d));
          
            var v = new Pager<WXReplyViewModel>(request, totalCount) { Data = vo.ToList() };

            return View("List", v);
        }
        public ActionResult Create()
        {
            return View();
        }


        public ActionResult Edit(int id)
        {
            var entity = _wxRepo.Find(id);
            if (entity == null)
                throw new ArgumentException("id not exist");

            var vo = new WXReplyViewModel().FromEntity<WXReplyViewModel>(entity);

            return View(vo);
        }

        [HttpPost]
        public ActionResult Create(FormCollection formCollection, WXReplyViewModel vo)
        {
            if (!ModelState.IsValid)
            {
                return View(vo);
            }

                var entity = vo.ToEntity<WXReplyEntity>();
              
                entity.UpdateDate = DateTime.Now;
                _wxRepo.Insert(entity);
              

                return RedirectToAction("Edit", new { id = entity.Id });

        }

        [HttpPost]
        public ActionResult Edit(FormCollection formCollection, WXReplyViewModel vo)
        {
            if (!ModelState.IsValid)
            {
                return View(vo);
            }
            var entity = _wxRepo.Find(vo.Id);
            entity.MatchKey = vo.MatchKey;
            entity.ReplyMsg = vo.ReplyMsg;
            entity.Status = vo.Status;
            entity.UpdateDate = DateTime.Now;
            _wxRepo.Update(entity);
           

            return RedirectToAction("list");
        }

        [HttpPost]
        public JsonResult Delete(int id)
        {
            var entity = _wxRepo.Find(id);
            if (entity == null)
            {
                ModelState.AddModelError("", "参数验证失败.");
                return FailResponse();
            }

            entity.UpdateDate = DateTime.Now;
            entity.Status = (int)DataStatus.Deleted;

            _wxRepo.Update(entity);

            return SuccessResponse();
        }
    }
}