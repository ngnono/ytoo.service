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
    [Export(typeof(CounterViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class CounterViewModel : BindableBase
    {
        private readonly ICounterService counterService;

        [ImportingConstructor]
        public CounterViewModel(ICounterService counterService)
        {
            this.counterService = counterService;
        }
    }
}
