using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Yintai.Hangzhou.Cms.WebSiteV1.Controllers
{
   
    public class WxmenuController : UserController
    {
       
        public ActionResult Index()
        {
            return View();
        }
        
        public JsonResult Ajax_All()
        {
            return Json(new List<dynamic>(),JsonRequestBehavior.AllowGet);
        }

    }
}
