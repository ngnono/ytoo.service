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
    }
}
