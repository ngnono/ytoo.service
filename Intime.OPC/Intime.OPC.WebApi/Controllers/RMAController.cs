using Intime.OPC.Domain.Dto.Custom;
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
    public class RMAController : BaseController
    {

        public RMAController()
        {
            

        }

        /// <summary>
        /// 客服退货 生成销售退货单
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult CreateSaleRMA([FromBody] RMAPost request)
        {
            //todo 客服退货 生成销售退货单
            return DoFunction(() => { return null; }, "生成销售退货单失败");

        }
    }
}
