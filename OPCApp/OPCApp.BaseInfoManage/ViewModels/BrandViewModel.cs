using Intime.OPC.Modules.Dimension.Services;
using Microsoft.Practices.Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intime.OPC.Modules.Dimension.ViewModels
{
    [Export(typeof(BrandViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class BrandViewModel : BindableBase
    {
        private readonly IBrandService brandService;

        [ImportingConstructor]
        public BrandViewModel(IBrandService brandService)
        {
            this.brandService = brandService;
        }


    }
}
