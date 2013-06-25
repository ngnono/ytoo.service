using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Yintai.Architecture.Common;
using Yintai.Architecture.Common.Models;
using Yintai.Architecture.Common.Web.Mvc.ActionResults;
using Yintai.Architecture.Common.Web.Mvc.Controllers;
using Yintai.Hangzhou.Contract.DTO.Response;
using Yintai.Hangzhou.Data.Models;
using Yintai.Hangzhou.Model.Enums;

namespace Yintai.Hangzhou.WebApiCore.Areas.Api.Controllers
{
    public class EnvironmentController : RestfulController
    {
        public ActionResult ServerDateTime()
        {
            return new RestfulResult
            {
                Data = new ExecuteResult<string>(DateTime.Now.ToString(Define.DateDefaultFormat))
                    {
                        StatusCode = StatusCode.Success,
                        Message = "成功"
                    }
            };
        }
        /// <summary>
        /// return all payment methods
        /// </summary>
        /// <returns></returns>
        public ActionResult Payments()
        {
            var result = Context.Set<PaymentMethodEntity>().Where(p => p.Status == (int)DataStatus.Normal)
                        .ToList()
                       .Select(p => new PaymentResponse().FromEntity<PaymentResponse>(p));
            var response = new PagerInfoResponse<PaymentResponse>(null, result.Count())
            {
                Items = result.ToList()
            };
            return new RestfulResult { Data = new ExecuteResult<PagerInfoResponse<PaymentResponse>>(response) };
        }

        /// <summary>
        /// return all support shipping area
        /// </summary>
        /// <returns></returns>
        public ActionResult SupportShipments()
        {
            var linq = Context.Set<CityEntity>().Where(p => p.Status == (int)DataStatus.Normal && p.IsProvince == true)
                        .GroupJoin(Context.Set<CityEntity>().Where(p => p.Status == (int)DataStatus.Normal && p.IsProvince == false),
                                o => o.Id,
                                i => i.ParentId,
                                (o, i) => new { P = o, C = i });
            var result = linq.ToList()
                        .Select(l => new GetShipCityDetailResponse().FromEntity<GetShipCityDetailResponse>(l.P, r => {
                            r.ProvinceName = l.P.Name;
                            r.Cities = l.C.Select(c => new ShipCityModel() { 
                                 Id = c.Id,
                                  CityName = c.Name,
                                  ZipCode = c.ZipCode
                            });
                        }));
            var response = new PagerInfoResponse<GetShipCityDetailResponse>(null, result.Count())
            {
                Items = result.ToList()
            };
            return new RestfulResult { Data = new ExecuteResult<PagerInfoResponse<GetShipCityDetailResponse>>(response) };
        }
    }
}
