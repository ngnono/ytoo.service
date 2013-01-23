using System.Web.Mvc;

namespace Yintai.Hangzhou.WebApiCore.Controllers
{
    public class HomeController : Controller
    {
        //public ActionResult Index()
        //{
        //    ViewBag.Message = "Welcome to Yintai Hangzhou";

        //    return View();
        //}

        public ActionResult ServerT()
        {
            ViewBag.Message = "Welcome to Yintai Hangzhou";
            return View("About");
        }
    }
}
