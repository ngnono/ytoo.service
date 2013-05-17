using CrystalDecisions.CrystalReports.Engine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Yintai.Architecture.Common.Models;
using Yintai.Hangzhou.Cms.WebSiteCoreV1.Models;
using Yintai.Hangzhou.Cms.WebSiteCoreV1.Util;
using Yintai.Hangzhou.Data.Models;
using Yintai.Hangzhou.Repository.Contract;

namespace Yintai.Hangzhou.Cms.WebSiteCoreV1.Controllers
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
                 .GroupBy(l => new { l.P.Store_Id, l.S.Name, l.P.Brand_Id, SName = l.B.Name });
            return linq.Select(l => new ProductByBrandReportViewModel()
            {
                BrandId = l.Key.Brand_Id,
                BrandName = l.Key.Name,
                StoreId = l.Key.Store_Id,
                StoreName = l.Key.SName,
                Products = l.Count()

            }).OrderByDescending(l=>l.Products);
        }
    }
}
