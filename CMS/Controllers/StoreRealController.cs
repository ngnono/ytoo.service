using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Yintai.Hangzhou.Repository.Contract;

namespace Yintai.Hangzhou.Cms.WebSiteV1.Controllers
{
    public class StoreRealController:UserController
    {
        private IStoreRealRepository _storeRepo;
        public StoreRealController(IStoreRealRepository storeRepo)
        {
            _storeRepo = storeRepo;
        }
        [HttpGet]
        public override JsonResult AutoComplete(string name)
        {
            return Json(_storeRepo.AutoComplete(name)
            .Where(entity => string.IsNullOrEmpty(name) ? true : entity.Name.StartsWith(name.Trim()))
            .Take(10)
                , JsonRequestBehavior.AllowGet);
        }
    }
}