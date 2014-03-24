using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Yintai.Hangzhou.WebSupport.Mvc;

namespace Yintai.Hangzhou.WebApiCore.Areas.Ims.Controllers
{
    public class ResourceController:RestfulController
    {
        [RestfulAuthorize]
        public ActionResult Upload(FormCollection form,int authuid)
        {
            return this.RenderSuccess<dynamic>(c => c.Data = new { 
                id = 1,
                url = "http://irss.ytrss.com/fileupload/img/product/20140321/3c955b75-aa89-4732-96ea-925f6dd853e3_320x0.jpg"
            });
        }
    }
}
