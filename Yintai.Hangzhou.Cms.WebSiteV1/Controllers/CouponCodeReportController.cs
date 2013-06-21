using CrystalDecisions.Shared;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Yintai.Architecture.Common.Models;
using Yintai.Hangzhou.Cms.WebSiteV1.Models;
using Yintai.Hangzhou.Cms.WebSiteV1.Util;
using Yintai.Hangzhou.Data.Models;
using Yintai.Hangzhou.Repository.Contract;

namespace Yintai.Hangzhou.Cms.WebSiteV1.Controllers
{
    [AdminAuthorize]
    public class CouponCodeReportController:UserController
    {
        private IPromotionRepository _proRepo;
        public CouponCodeReportController(IPromotionRepository proRepo)
        {
            _proRepo = proRepo;
        }

        public ActionResult Search()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Search(PagerRequest request, CouponUsageOption search)
        {

            if (!ModelState.IsValid)
            {
                ViewBag.SearchOptions = search;
                return View();
            }
            var prods = doSearch(search);
            var v = new Pager<CouponCodeUsageViewModel>(request, prods.Count()) { Data = prods.ToList() };
            ViewBag.SearchOptions = search;
            return View(v);
        }
        public ActionResult Download(string reportname, string option)
        {
            CouponUsageOption search = JsonConvert.DeserializeObject<CouponUsageOption>(option);
            if (null == search)
                throw new ArgumentNullException("option");
            return RenderReport(reportname, r =>
            {
                r.SetDataSource(doSearch(search).ToList());

            }, ExportFormatType.Excel);
        }
        private IEnumerable<CouponCodeUsageViewModel> doSearch(CouponUsageOption search)
        {
            var dbContext = _proRepo.Context;
            var linq= dbContext.Set<CouponHistoryEntity>()
                    .Join(dbContext.Set<PromotionEntity>(),o=>o.FromPromotion,i=>i.Id,(o,i)=>new {C=o,P=i})
                    .Join(dbContext.Set<UserEntity>(),o=>o.C.User_Id,i=>i.Id,(o,i)=>new {C=o.C,P=o.P,U=i})
                    .Where(s=>(!search.PromotionId.HasValue 
                                    || s.C.FromPromotion == search.PromotionId.Value) &&
                               (!search.CreateDateFrom.HasValue || s.C.CreatedDate>=search.CreateDateFrom.Value) &&
                               (!search.CreateDateTo.HasValue || s.C.CreatedDate <= search.CreateDateTo.Value))
                    .OrderByDescending(s=>s.C.CreatedDate);
           

            var result = linq.ToList().Select(l => new CouponCodeUsageViewModel()
                {
                    Code = l.C.CouponId,
                     PromotionName = l.P.Name,
                     CustomerPhone = l.U.Mobile,
                      CustomerNick = l.U.Nickname,
                      PromotionDate = string.Format("{0}-{1}",l.P.StartDate,l.P.EndDate),
                      CreateDate = l.C.CreatedDate
                });
            return result;
        }
    }
}