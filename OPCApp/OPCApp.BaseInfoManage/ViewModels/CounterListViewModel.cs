using Intime.OPC.Modules.Dimension.Models;
using Intime.OPC.Modules.Dimension.Services;
using Microsoft.Practices.Prism.Interactivity;
using Microsoft.Practices.Prism.Interactivity.InteractionRequest;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Input;
using Intime.OPC.Modules.Dimension.Framework;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Intime.OPC.Modules.Dimension.ViewModels
{
    [Export(typeof(CounterListViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class CounterListViewModel : DimensionListViewModel<Counter,CounterViewModel,IService<Counter>>
    {
        public CounterListViewModel()
        {
            EnableCommand = new AsyncDelegateCommand(() => OnEnable(false), OnException, CanExecute);
            DisableCommand = new AsyncDelegateCommand(() => OnEnable(true), OnException, CanExecute);
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
                    counter = Service.Update(counter);
                }
            });
        }
        #endregion
    }
}
