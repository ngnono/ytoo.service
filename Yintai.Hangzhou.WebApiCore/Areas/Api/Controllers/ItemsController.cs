using System.Web.Mvc;
using Yintai.Architecture.Common.Models;
using Yintai.Architecture.Common.Web.Mvc.ActionResults;
using Yintai.Architecture.Common.Web.Mvc.Controllers;
using Yintai.Hangzhou.Contract.DTO.Request.ProductComplex;
using Yintai.Hangzhou.Contract.ProductComplex;
using Yintai.Hangzhou.Model;
using Yintai.Hangzhou.WebSupport.Binder;
using Yintai.Hangzhou.WebSupport.Mvc;

namespace Yintai.Hangzhou.WebApiCore.Areas.Api.Controllers
{
    public class ItemsController : RestfulController
    {
        private readonly IItemsDataService _itemsDataService;

        public ItemsController(IItemsDataService itemsDataService)
        {
            _itemsDataService = itemsDataService;
        }

        [RestfulAuthorize(true)]
        public ActionResult List(GetItemsListRequest request, int? authUid, UserModel authUser, [FetchUser(KeyName = "userid", IsCanMissing = true)]UserModel showUser)
        {
            if (showUser == null && authUid == null && authUser == null)
            {
                return new RestfulResult
                    {
                        Data = new ExecuteResult
                            {
                                StatusCode = StatusCode.ClientError,
                                Message = "用户参数错误"
                            }
                    };
            }

            request.UserModel = showUser ?? authUser;

            return new RestfulResult { Data = _itemsDataService.GetProductList(request) };
        }
    }
}
