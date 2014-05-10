using Intime.OPC.Modules.Dimension.Models;
using Intime.OPC.Modules.Dimension.Services;
using Microsoft.Practices.Prism.Interactivity;
using Microsoft.Practices.Prism.Interactivity.InteractionRequest;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using OPCApp.Common.Extensions;
using System.Windows.Input;
using Intime.OPC.Modules.Dimension.Common;

namespace Intime.OPC.Modules.Dimension.ViewModels
{
    [Export(typeof(BrandListViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class BrandListViewModel : DimensionListViewModel<Brand,BrandViewModel,IBrandService>
    {
        public BrandListViewModel()
        {
            EnableCommand = new DelegateCommand(() => OnEnable(true));
            DisableCommand = new DelegateCommand(() => OnEnable(false));
        }

        #region Commands

        public ICommand DisableCommand { get; private set; }

        public ICommand EnableCommand { get; private set; }

        #endregion

        #region Command handler

        private void OnEnable(bool enabled)
        {
            Models.ForEach(brand => 
            {
                if (brand.IsSelected && brand.Enabled != enabled)
                {
                    brand.Enabled = enabled;
                    Service.Update(brand);
                }
            });
        }
        #endregion
    }
}
