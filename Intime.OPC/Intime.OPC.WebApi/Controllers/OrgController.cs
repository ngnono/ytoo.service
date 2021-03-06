﻿using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Intime.OPC.Domain;
using Intime.OPC.Domain.Models;
using Intime.OPC.Service;
using Intime.OPC.WebApi.Bindings;
using Intime.OPC.WebApi.Core;

namespace Intime.OPC.WebApi.Controllers
{
    public class OrgController : BaseController
    {
        private readonly IOrgService _orgServiceService;

        public OrgController(IOrgService orgService)
        {
            _orgServiceService = orgService;
        }

        [HttpPost]
        public IHttpActionResult GetAll()
        {
            return DoFunction(() =>
            {
                PageResult<OPC_OrgInfo> lst = _orgServiceService.GetAll(1, 10000);
                return  lst.Result;
            }, "获得组织结构信息失败");
        }

        [HttpPost]
        public IHttpActionResult AddOrg([FromBody]OPC_OrgInfo orgInfo)
        {
            return DoFunction(() =>
            {
                return _orgServiceService.AddOrgInfo(orgInfo);
                
            }, "获得品牌信息失败");
        }

        [HttpPut]
        public IHttpActionResult UpdateOrg([FromBody]OPC_OrgInfo orgInfo)
        {
            return DoFunction(() =>
            {
                bool bl = _orgServiceService.Update(orgInfo);
                return bl;
            }, "更新组织机构失败");
        }

        [HttpPut]
        public IHttpActionResult DeleteOrg([FromBody]int orgInfoId)
        {
            return DoFunction(() =>
            {
                bool bl = _orgServiceService.DeleteById(orgInfoId);
                return bl;
            }, "删除组织机构失败");
        }
    }
}