using System.Web.Mvc;
using System.Linq;
using Yintai.Architecture.Common.Models;
using Yintai.Architecture.Common.Web.Mvc.ActionResults;
using Yintai.Architecture.Common.Web.Mvc.Controllers;
using Yintai.Hangzhou.Contract.DTO.Request.ProductComplex;
using Yintai.Hangzhou.Contract.ProductComplex;
using Yintai.Hangzhou.Data.Models;
using Yintai.Hangzhou.Model;
using Yintai.Hangzhou.WebSupport.Binder;
using Yintai.Hangzhou.WebSupport.Mvc;
using Yintai.Hangzhou.Model.Enums;
using Yintai.Hangzhou.Contract.DTO.Response;
using Yintai.Hangzhou.Contract.DTO.Response.Resources;
using System.Collections.Generic;

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

            var linq = Context.Set<FavoriteEntity>().Where(f => f.FavoriteSourceType == (int)SourceType.Product && f.User_Id == request.UserModel.Id && f.Status != (int)DataStatus.Deleted)
                 .Join(Context.Set<ProductEntity>(), o => o.FavoriteSourceId, i => i.Id, (o, i) => new { F=o,P=i })
                 .GroupJoin(Context.Set<ResourceEntity>().Where(r => r.SourceType == (int)SourceType.Product && r.Type == (int)ResourceType.Image && r.Status != (int)DataStatus.Deleted)
                          , o => o.P.Id, i => i.SourceId, (o, i) => new { F=o.F,P = o.P, R = i.OrderByDescending(ri => ri.SortOrder).FirstOrDefault() });
            int totalCount = linq.Count();
            int skipCount = request.Page > 0 ? (request.Page - 1) * request.Pagesize : 0;
            linq = linq.OrderByDescending(l => l.F.CreatedDate).Skip(skipCount).Take(request.Pagesize);
            var result = linq.ToList().Select(l => new ItemsInfoResponse().FromEntity<ItemsInfoResponse>(l.P, o =>
            {
                o.SourceType = (int)SourceType.Product;
                if (l.R != null)
                {
                    o.Resources = new List<ResourceInfoResponse>();
                    o.Resources.Add(new ResourceInfoResponse().FromEntity<ResourceInfoResponse>(l.R));
                }

            }));
            
           return this.RenderSuccess<ItemsCollectionResponse>(m => m.Data = new ItemsCollectionResponse(request.PagerRequest, totalCount) { Items = result.ToList() });
            

        }
    }
}
