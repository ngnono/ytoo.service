using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Yintai.Hangzhou.Contract.DTO.Request;
using Yintai.Hangzhou.Contract.DTO.Response;
using Yintai.Hangzhou.Model.Enums;
using Yintai.Hangzhou.WebSupport.Mvc;

namespace Yintai.Hangzhou.WebApiCore.Areas.Ims.Controllers
{
    public class UserController:RestfulController
    {
        [RestfulAuthorize]
        public ActionResult Favor_Store(PagerInfoRequest request)
        {
            var mockupResponse = new List<dynamic>();
            mockupResponse.Add(new
            {
                id = 1,
                name = "mockup店铺1",
                image = "",
                phone = "13000000"
            });
            mockupResponse.Add(new
            {
                id = 1,
                name = "mockup店铺2",
                image = "",
                phone = "13000000"
            });

            var response = new PagerInfoResponse<dynamic>(request.PagerRequest, mockupResponse.Count)
            {
                Items = mockupResponse
            };
            return this.RenderSuccess<PagerInfoResponse<dynamic>>(c => c.Data = response);
        }

        [RestfulAuthorize]
        public ActionResult Favor_Combo(PagerInfoRequest request)
        {
            var mockupResponse = new List<dynamic>();
            mockupResponse.Add(new
            {
                id = 1,
                type =1,
                name = "mockupCombo1",
                is_online = true,
                price = 200.1,
                image = ""
            });
            mockupResponse.Add(new
            {
                id = 1,
                type = 2,
                name = "mockupCombo1",
                is_online = true,
                price = 200.1,
                image = ""
            });

            var response = new PagerInfoResponse<dynamic>(request.PagerRequest, mockupResponse.Count)
            {
                Items = mockupResponse
            };
            return this.RenderSuccess<PagerInfoResponse<dynamic>>(c => c.Data = response);
        }
    }
}
