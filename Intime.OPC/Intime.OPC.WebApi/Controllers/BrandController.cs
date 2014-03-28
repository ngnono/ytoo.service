using Intime.OPC.Domain.Models;
using Intime.OPC.Repository;
using Intime.OPC.Service;
using Intime.OPC.WebApi.Bindings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Intime.OPC.WebApi.Controllers
{
    public class BrandController:ApiController
    {
        private IBrandService _brandService;
        public BrandController(IBrandService brandService)
        {
            _brandService = brandService;
        }

        [HttpGet]
        public IHttpActionResult GetAll([UserId] int? userId)
        {
 
            try
            {
                var lst = _brandService.GetAll();
                return Ok(lst);
            }
            catch (Exception ex)
            {
                this.GetLog().Error(ex);
                return BadRequest("获得品牌信息失败");
            }
        }
    }
}
