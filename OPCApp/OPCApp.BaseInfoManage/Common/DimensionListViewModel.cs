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

namespace Intime.OPC.Modules.Dimension.Common
{
    public class DimensionListViewModel<TDimension, TDetailViewModel, IDimensionService> : BindableBase
        where TDimension : Intime.OPC.Modules.Dimension.Models.Dimension, new()
        where TDetailViewModel: ModalDialogViewModel<TDimension>, new()
        where IDimensionService : IService<TDimension>
    {
        private ObservableCollection<TDimension> models;

        public DimensionListViewModel()
        {
            SelectAllCommand = new DelegateCommand<bool?>(OnSelectAll);
            EditCommand = new DelegateCommand<int?>(OnEdit);
            CreateCommand = new DelegateCommand(OnCreate);
            DeleteCommand = new DelegateCommand(OnDelete);
            QueryCommand = new DelegateCommand<string>(OnQuery);

            EditRequest = new InteractionRequest<TDetailViewModel>();
            CreateRequest = new InteractionRequest<TDetailViewModel>();
        }

        #region Properties

        [Import]
        public IDimensionService Service { get; set; }

        public ObservableCollection<TDimension> Models 
        { 
            get {return models;}
            private set { SetProperty(ref models, value); }
        }

        #endregion

        public InteractionRequest<TDetailViewModel> EditRequest { get; set; }

        public InteractionRequest<TDetailViewModel> CreateRequest { get; set; }

        #region Commands

        public ICommand SelectAllCommand { get; private set; }

        public ICommand CreateCommand { get; private set; }

        public ICommand EditCommand { get; private set; }

        public ICommand DeleteCommand { get; private set; }

        public ICommand QueryCommand { get; private set; }

        #endregion

        #region Command handler

        private void OnSelectAll(bool? selected)
        {
            Models.ForEach(model => model.IsSelected = selected.Value);
        }

        private void OnQuery(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                Models = new ObservableCollection<TDimension>(Service.QueryAll());
            }
            else
            {
                Models = new ObservableCollection<TDimension>(Service.Query(name));
            }
        }

        private void OnDelete()
        {
            Models.ForEach(model =>
            {
                if (model.IsSelected) Service.Delete(model.ID);
            });

            while (Models.Any(brand => brand.IsSelected))
            {
                var selectedModel = Models.Where(model => model.IsSelected).FirstOrDefault();
                Models.Remove(selectedModel);
            }
        }

        private void OnCreate()
        {
            CreateRequest.Raise(
               new TDetailViewModel { Title = "新增", Model = new TDimension() },
               (viewModel) =>
               {
                   if (viewModel.Accepted)
                   {
                       var brand = Service.Create(viewModel.Model);

                       models.Insert(0, brand);
                   }
               });
        }

        private void OnEdit(int? id)
        {
            EditRequest.Raise(
                new TDetailViewModel { Title = "编辑", Model = Service.Query(id.Value) },
                (viewModel) => 
                {
                    if (viewModel.Accepted) Service.Update(viewModel.Model);
                });
        }
        #endregion
    }
}
