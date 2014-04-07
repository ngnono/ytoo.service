using System.Web.Http;
using Intime.OPC.Domain.Dto.Custom;
using Intime.OPC.Service;
using Intime.OPC.WebApi.Core;

namespace Intime.OPC.WebApi.Controllers
{
    public class RMAController : BaseController
    {
        private ISaleRMAService _saleRmaService;

        public RMAController(ISaleRMAService saleRmaService)
        {
            _saleRmaService = saleRmaService;
        }

        /// <summary>
        ///     客服退货 生成销售退货单
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult CreateSaleRMA([FromBody] RMAPost request)
        {
            var user = GetCurrentUserID();
            //todo 客服退货 生成销售退货单
            return DoAction(() => { _saleRmaService.CreateSaleRMA(user,request);}, "生成销售退货单失败");
        }
    }
}