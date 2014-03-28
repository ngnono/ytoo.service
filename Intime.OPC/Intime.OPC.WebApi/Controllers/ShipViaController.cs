using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Intime.OPC.Service;
using Intime.OPC.WebApi.Bindings;

namespace Intime.OPC.WebApi.Controllers
{
    public class ShipViaController:ApiController
    {
        private IShipViaService _shipViaService;
        public ShipViaController(IShipViaService shipViaService)
        {
            _shipViaService = shipViaService;
        }

        [HttpGet]
        public IHttpActionResult GetAll()
        {
 
            try
            {
                var lst = _shipViaService.GetAll();
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
