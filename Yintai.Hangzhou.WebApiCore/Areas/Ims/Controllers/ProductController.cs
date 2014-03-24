using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Yintai.Hangzhou.Contract.DTO.Response;
using Yintai.Hangzhou.Model.Enums;
using Yintai.Hangzhou.WebSupport.Mvc;

namespace Yintai.Hangzhou.WebApiCore.Areas.Ims.Controllers
{
    public class ProductController:RestfulController
    {
        [RestfulAuthorize]
        public ActionResult Create(Yintai.Hangzhou.Contract.DTO.Request.IMSProductCreateRequest request)
        {
            return this.RenderSuccess<dynamic>(c => c.Data = new { 
                id= 1,
                image = "http://irss.ytrss.com/fileupload/img/product/20140321/3c955b75-aa89-4732-96ea-925f6dd853e3_320x0.jpg",
                price = 100.1
            });   
        }
        [RestfulAuthorize]
        public ActionResult Search(Yintai.Hangzhou.Contract.DTO.Request.IMSProductSearchRequest request)
        {
            var mockupResponse = new List<dynamic>();
            mockupResponse.Add(new
            {
                id = 1,
                desc = "系统商品1",
                price = 100.1,
                image = "",
                create_date = "2014-03-24 1:00:00",
                product_type = (int)ProductType.FromSystem
            });
            mockupResponse.Add(new
            {
                id = 1,
                desc = "系统商品2",
                price = 100.1,
                image = "",
                create_date = "2014-03-25 2:00:00",
                product_type = (int)ProductType.FromSystem
            });
            var response = new PagerInfoResponse<dynamic>(request.PagerRequest, mockupResponse.Count)
            {
                Items = mockupResponse
            };
            return this.RenderSuccess<PagerInfoResponse<dynamic>>(c => c.Data = response);   
        }

        [RestfulAuthorize]
        public ActionResult Brands(Yintai.Hangzhou.Contract.DTO.Request.IMSProductBrandsRequest request)
        {
            var mockupResponse = new List<dynamic>();
            mockupResponse.Add(new
            {
                id = 1,
                name = "mockup系统品牌1"
            });
            mockupResponse.Add(new
            {
                id = 2,
                name = "mockup系统品牌2"
            });
            var response = new PagerInfoResponse<dynamic>(request.PagerRequest, mockupResponse.Count)
            {
                Items = mockupResponse
            };
            return this.RenderSuccess<PagerInfoResponse<dynamic>>(c => c.Data = response);
        }
    }
}
