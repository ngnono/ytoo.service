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
    }
}
