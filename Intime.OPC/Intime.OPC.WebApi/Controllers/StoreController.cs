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
using Intime.OPC.WebApi.Core;

namespace Intime.OPC.WebApi.Controllers
{
    public class StoreController : BaseController
    {
        private IStoreService _Service;
        public StoreController(IStoreService service)
        {
            _Service = service;
        }

        [HttpGet]
        public IHttpActionResult GetAll([UserId] int? userId)
        {
 
            try
            {
                var lst = _Service.GetAll();
                return Ok(lst);
            }
            catch (Exception ex)
            {
                this.GetLog().Error(ex);
                return BadRequest("获得门店信息失败");
            }
        }
    }
}
