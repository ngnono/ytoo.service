using CrystalDecisions.CrystalReports.Engine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Mvc;

namespace Yintai.Hangzhou.Cms.WebSiteCoreV1.Controllers
{
    public class HomeController : UserController
    {
        public ActionResult Index()
        {
            ViewBag.Message = "欢迎光临 Yintai Hangzhou CMS。版本： 1.0.0.1 版后台";

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "内容管理平台";

            return View();
        }

        public ActionResult Print()
        { 
             ReportClass rptH = new ReportClass();
             rptH.FileName = Server.MapPath("~/Content/report/demo.rpt");
             List<dynamic> mock = new List<dynamic>();
                 for (int i=0;i<10;i++) 
                 {
                    mock.Add( new {Id = 1,Name="test"});
                 }

                 rptH.SetDataSource(mock);
            Stream stream = rptH.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
        return File(stream, "application/pdf");  
        }
    }
}
