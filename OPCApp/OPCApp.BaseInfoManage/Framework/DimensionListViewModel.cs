using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Interactivity.InteractionRequest;
using Microsoft.Practices.Prism.Mvvm;
using OPCApp.Infrastructure;

namespace Intime.OPC.Modules.Dimension.Framework
{
    public class DimensionListViewModel<TDimension, TDetailViewModel, TDimensionService> : ViewModelBase
        where TDimension : Intime.OPC.Modules.Dimension.Models.Dimension, new()
        where TDetailViewModel : ModalDialogViewModel<TDimension>, new()
        where TDimensionService : IService<TDimension>
    {
        private ObservableCollection<TDimension> models;
        private IQueryCriteria queryCriteria;

        public DimensionListViewModel()
        {
            Models = new ObservableCollection<TDimension>();

            SelectAllCommand = new DelegateCommand<bool?>(OnSelectAll);
            EditCommand = new DelegateCommand<int?>(OnEdit);
            CreateCommand = new DelegateCommand(OnCreate);
            DeleteCommand = new AsyncDelegateCommand(OnDelete, OnException, CanDelete);
            QueryCommand = new AsyncDelegateCommand<string>(OnQuery, OnException);
            LoadNextPageCommand = new AsyncDelegateCommand(OnNextPageLoad, OnException);

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

        public ICommand LoadNextPageCommand { get; private set; }

        #endregion

        #region Command handler

        protected bool CanExecute()
        {
            var selected = models != null && models.Any(model => model.IsSelected);
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
            if (models == null && !models.Any()) return;

            models.ForEach(model => model.IsSelected = selected.Value);
        }

        private void OnQuery(string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                queryCriteria = new QueryByName { Name = name, PageIndex = 1, PageSize = 100 };
            }
            else
            {
                queryCriteria = new QueryAll { PageIndex = 1, PageSize = 100 };
            }

            Models = new ObservableCollection<TDimension>(Service.Query(queryCriteria));
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
                PerformActionOnUIThread(() => { Models.Remove(selectedModel); });
            }
        }

        private void OnCreate()
        {
            var detailViewModel = AppEx.Container.GetInstance<TDetailViewModel>();
            detailViewModel.Title = "新增";
            detailViewModel.Model = new TDimension();

            CreateRequest.Raise(detailViewModel, (viewModel) =>
            {
                if (viewModel.Accepted)
                {
                    Action create = () =>
                    {
                        var model = Service.Create(viewModel.Model);
                        models.Insert(0, model);
                    };

                    PerformAction(create);
                }
            });
        }

        private void OnEdit(int? id)
        {
            var detailViewModel = AppEx.Container.GetInstance<TDetailViewModel>();
            detailViewModel.Title = "编辑";
            detailViewModel.Model = Service.Query(id.Value);

            EditRequest.Raise(detailViewModel, (viewModel) => 
            {
                if (viewModel.Accepted)
                {
                    Action edit = () =>
                    {
                        var updatedModel = Service.Update(viewModel.Model);
                        var modelToUpdate = models.Where(model => model.ID == viewModel.Model.ID).FirstOrDefault();
                        int index = Models.IndexOf(modelToUpdate);

                        models.Remove(modelToUpdate);
                        models.Insert(index, updatedModel);
                    };

                    PerformAction(edit);
                }
            });
        }

        private void OnNextPageLoad()
        {
            if (queryCriteria == null) return;

            queryCriteria.PageIndex++;

            var moreModels = Service.Query(queryCriteria);
            foreach (var model in moreModels)
            {
                PerformActionOnUIThread(() => { Models.Add(model); });
            }
        }

        #endregion
    }
}
