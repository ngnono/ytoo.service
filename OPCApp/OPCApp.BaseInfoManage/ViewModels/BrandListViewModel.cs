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
    [Export(typeof(BrandListViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class BrandListViewModel : BindableBase
    {
        private readonly IBrandService brandService;

        [ImportingConstructor]
        public BrandListViewModel(IBrandService brandService)
        {
            this.brandService = brandService;
        }
    }
}
