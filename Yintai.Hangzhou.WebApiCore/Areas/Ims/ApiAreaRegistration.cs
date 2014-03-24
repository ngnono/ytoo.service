using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Yintai.Hangzhou.WebApiCore.Areas.Api2
{
    public class ImsAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Ims";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Ims_default",
                "ims/{controller}/{action}/{method}",
                new { action = "Index", method = UrlParameter.Optional,app=UrlParameter.Optional },
                new[] { "Yintai.Hangzhou.WebApiCore.Areas.Ims.Controllers" }
            );
        }
    }
}
