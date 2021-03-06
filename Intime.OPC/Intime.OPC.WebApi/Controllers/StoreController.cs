﻿using Intime.OPC.Domain.Models;
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

        [HttpPost]
        public IHttpActionResult GetAll([UserId] int? userId)
        {

            return DoFunction(() => _Service.GetAll(), "获得门店信息失败");
           
        }
    }
}
