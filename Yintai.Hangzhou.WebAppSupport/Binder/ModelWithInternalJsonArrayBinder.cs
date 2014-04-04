using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Yintai.Hangzhou.WebSupport.Binder
{
    public class ModelWithInternalJsonArrayBinder:DefaultModelBinder
    {
        protected override void BindProperty(ControllerContext controllerContext, ModelBindingContext bindingContext, System.ComponentModel.PropertyDescriptor propertyDescriptor)
        {
            
            base.BindProperty(controllerContext, bindingContext, propertyDescriptor);
        }
    }
}
