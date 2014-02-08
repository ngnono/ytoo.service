using CrystalDecisions.CrystalReports.Engine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    public class ReportController : UserController
    {
        private IProductRepository _productRepo;
        public ReportController(IProductRepository productRepo)
        {
            _productRepo = productRepo;
        }

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult ProductByBrand()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ProductByBrand(PagerRequest request, ReportByProductBrandOption search)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.SearchOptions = search;
                return View();
            }
            var prods = searchProduct(search);
            var v = new Pager<ProductByBrandReportViewModel>(request, prods.Count()) { Data = prods.ToList() };
            ViewBag.SearchOptions = search;
            return View(v);
        }
        public ActionResult StoreCouponUsage()
        {
            return View();
        }
        [HttpPost]
        public ActionResult StoreCouponUsage(PagerRequest request, StoreCouponUsageOption search)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.SearchOptions = search;
                return View();
            }
            var prods = searchCouponLog(search);
            var v = new Pager<StoreCouponUsageViewModel>(request, prods.Count()) { Data = prods.ToList() };
            ViewBag.SearchOptions = search;
            return View(v);
        }

        private IEnumerable<StoreCouponUsageViewModel> searchCouponLog(StoreCouponUsageOption search)
        {
            var dbContext = _productRepo.Context;
            return dbContext.Set<CouponLogEntity>().Where(p => p.Type == (int)CouponType.StorePromotion && p.Code == search.Code)
                .GroupJoin(dbContext.Set<StoreRealEntity>(),o=>o.ConsumeStoreNo,i=>i.StoreNo,(o,i)=>new {C=o,S=i.FirstOrDefault()})
                .OrderByDescending(c=>c.C.Id)
                .ToList()
                .Select(l => new StoreCouponUsageViewModel().FromEntity<StoreCouponUsageViewModel>(l.C,p=>{
                    p.StoreName = l.S == null ? string.Empty : l.S.Name;
                }));

        }
        public ActionResult Download(string reportname, ReportByProductBrandOption search)
        {
            ReportClass rptH = new ReportClass();
            rptH.FileName = ReportPath(reportname);
            var data = searchProduct(search).ToList();
            rptH.SetDataSource(data);
            rptH.SetParameterValue("datefrom", search.FromDate.ToReportFormat());
            rptH.SetParameterValue("dateto", search.ToDate.ToReportFormat());

            Stream stream = rptH.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            return File(stream, "application/pdf");
        }
        public ActionResult DownloadSCU(string reportname, StoreCouponUsageOption search)
        {
            return RenderReport(reportname, r => {
                r.SetDataSource(searchCouponLog(search).ToList());
            });
        }

        private string ReportPath(string reportname)
        {
            return Server.MapPath(string.Format("~/Content/report/{0}.rpt", reportname));
        }
        private IEnumerable<ProductByBrandReportViewModel> searchProduct(ReportByProductBrandOption option)
        {
            var dbContext = _productRepo.Context;
            var linq = dbContext.Set<ProductEntity>().Where(p => (!option.FromDate.HasValue || p.CreatedDate >= option.FromDate.Value) &&
                                                     (!option.ToDate.HasValue || p.CreatedDate < option.ToDate.Value))
                 .Join(dbContext.Set<StoreEntity>(), o => o.Store_Id, i => i.Id, (o, i) => new { P = o, S = i })
                 .Join(dbContext.Set<BrandEntity>(), o => o.P.Brand_Id, i => i.Id, (o, i) => new { P = o.P, S = o.S, B = i })
                 .GroupBy(l => new { l.P.Store_Id, l.S.Name, l.P.Brand_Id, BName = l.B.Name });
            return linq.Select(l => new ProductByBrandReportViewModel()
            {
                BrandId = l.Key.Brand_Id,
                BrandName = l.Key.BName,
                StoreId = l.Key.Store_Id,
                StoreName = l.Key.Name,
                Products = l.Count()

            }).OrderByDescending(l=>l.Products);
        }
    }
}
