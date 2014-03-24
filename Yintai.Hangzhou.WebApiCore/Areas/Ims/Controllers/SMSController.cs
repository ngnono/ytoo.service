using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Yintai.Hangzhou.WebSupport.Mvc;

namespace Yintai.Hangzhou.WebApiCore.Areas.Ims.Controllers
{
    public class SMSController:RestfulController
    {
        [RestfulAuthorize]
        public ActionResult Send(string phone, string text)
        {
            return this.RenderSuccess<dynamic>(null);
        }
    }
}
