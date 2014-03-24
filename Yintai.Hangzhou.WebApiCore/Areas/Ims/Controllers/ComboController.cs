using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Yintai.Hangzhou.WebSupport.Mvc;

namespace Yintai.Hangzhou.WebApiCore.Areas.Ims.Controllers
{
    public class ComboController:RestfulController
    {
        [RestfulAuthorize]
        public ActionResult Create(Yintai.Hangzhou.Contract.DTO.Request.IMSComboCreateRequest request)
        {
            return this.RenderSuccess<dynamic>(c => c.Data =  new { 
                 id = 1,
                 image = "http://irss.ytrss.com/fileupload/img/product/20140321/3c955b75-aa89-4732-96ea-925f6dd853e3_320x0.jpg",
                 price = 200.1
            });
        }

        [RestfulAuthorize]
        public ActionResult Detail(int id,int authuid)
        {
            return this.RenderSuccess<dynamic>(c => c.Data = new
            {
                id = 1,
                desc = "mockup detail详情",
                private_to = "东哥",
                image = new string[]{ "http://irss.ytrss.com/fileupload/img/product/20140321/3c955b75-aa89-4732-96ea-925f6dd853e3_320x0.jpg", 
                                    "http://irss.ytrss.com/fileupload/img/product/20140321/3c955b75-aa89-4732-96ea-925f6dd853e3_320x0.jpg"},
                price = 200.1,
                products = new dynamic[]{new {id = 1,
                                            product_type=1,
                                            image = "http://irss.ytrss.com/fileupload/img/product/20140321/3c955b75-aa89-4732-96ea-925f6dd853e3_320x0.jpg",
                                            price = 100.1},
                new {id = 2,
                                            product_type=1,
                                            image = "http://irss.ytrss.com/fileupload/img/product/20140321/3c955b75-aa89-4732-96ea-925f6dd853e3_320x0.jpg",
                                            price = 100.2},
                }
            });
        }
    }
}
