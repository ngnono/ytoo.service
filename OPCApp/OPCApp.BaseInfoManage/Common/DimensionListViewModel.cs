using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Interactivity.InteractionRequest;
using Microsoft.Practices.Prism.Mvvm;
using System.Windows.Threading;
using System.Collections.Generic;
using System.Windows;

namespace Intime.OPC.Modules.Dimension.Common
{
    public class DimensionListViewModel<TDimension, TDetailViewModel, TDimensionService> : BindableBase
        where TDimension : Intime.OPC.Modules.Dimension.Models.Dimension, new()
        where TDetailViewModel: ModalDialogViewModel<TDimension>, new()
        where TDimensionService : IService<TDimension>
    {
        private ObservableCollection<TDimension> models;

        public DimensionListViewModel()
        {
            SelectAllCommand = new DelegateCommand<bool?>(OnSelectAll);
            EditCommand = new DelegateCommand<int?>(OnEdit);
            CreateCommand = new DelegateCommand(OnCreate);
            DeleteCommand = new AsyncDelegateCommand(OnDelete, CanDelete);
            QueryCommand = new AsyncDelegateCommand<string>(OnQuery);

            EditRequest = new InteractionRequest<TDetailViewModel>();
            CreateRequest = new InteractionRequest<TDetailViewModel>();
        }

        #region Properties

        [Import]
        public TDimensionService Service { get; set; }

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

        protected bool CanExecute()
        {
            var selected = Models != null && Models.Any(model => model.IsSelected);
            if (!selected) MessageBox.Show("请选择至少一个对象", "警告", MessageBoxButton.OK, MessageBoxImage.Warning);

            return selected;
        }

        private bool CanDelete()
        {
            if (!CanExecute()) return false;

            return (MessageBox.Show("确定要删除吗？", "删除", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes);
        }

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
                Application.Current.Dispatcher.Invoke(() => { Models.Remove(selectedModel); });
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
                       var model = Service.Create(viewModel.Model);

                       models.Insert(0, model);
                   }
               });
        }

        private void OnEdit(int? id)
        {
            EditRequest.Raise(
                new TDetailViewModel { Title = "编辑", Model = Service.Query(id.Value) },
                (viewModel) => 
                {
                    if (viewModel.Accepted)
                    {
                        var updatedModel = Service.Update(viewModel.Model);
                        var modelToUpdate = models.Where(model => model.ID == viewModel.Model.ID).FirstOrDefault();
                        int index = Models.IndexOf(modelToUpdate);

                        models.Remove(modelToUpdate);
                        models.Insert(index, updatedModel);
                    }
                });
        }
        #endregion
    }
}
