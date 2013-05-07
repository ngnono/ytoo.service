using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yintai.Hangzhou.WebSupport.Mvc;

namespace Yintai.Hangzhou.WebApiCore
{
    public class ApiApplication : MvcApplication
    {
        public ApiApplication()
            : base("Yintai.Hangzhou.WebApiCore.Controllers")
        {
        }

        protected override void CApplication_Start()
        {
            //ImageMagickNET.MagickNet.InitializeMagick();
            base.CApplication_Start();
        }
    }
}
