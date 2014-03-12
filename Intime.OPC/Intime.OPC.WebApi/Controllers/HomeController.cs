using Intime.OPC.Service;
using Intime.OPC.Service.Support;
using Intime.OPC.WebApi.Bindings;
using System;
using System.Collections.Generic;
using System.Web.Http;
using System.Web.Mvc;

namespace Intime.OPC.WebApi.Controllers
{
    public class HomeController : Controller
    {
        public string Index()
        {
            return "<h1>API Server V1.0</h1>";
        }
    }
}
