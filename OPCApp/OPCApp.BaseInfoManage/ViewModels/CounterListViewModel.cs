using Intime.OPC.Modules.Dimension.Common;
using Intime.OPC.Modules.Dimension.Models;
using Intime.OPC.Modules.Dimension.Services;
using Microsoft.Practices.Prism.Commands;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows.Input;

namespace Intime.OPC.Modules.Dimension.ViewModels
{
    [Export(typeof(CounterListViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class CounterListViewModel : DimensionListViewModel<Counter,CounterViewModel,ICounterService>
    {
        public CounterListViewModel()
        {
            EnableCommand = new DelegateCommand(() => OnEnable(true));
            DisableCommand = new DelegateCommand(() => OnEnable(false));
        }

        #region Commands

        public ICommand DisableCommand { get; private set; }

        public ICommand EnableCommand { get; private set; }

        #endregion

        #region Command handler

        private void OnEnable(bool repealed)
        {
            Models.ForEach(counter => 
            {
                if (counter.IsSelected && counter.Repealed != repealed)
                {
                    counter.Repealed = repealed;
                    Service.Update(counter);
                }
            });
        }
        #endregion
    }
}
