// ***********************************************************************
// Assembly         : OPCApp.Main.Infrastructure
// Author           : Liuyh
// Created          : 03-13-2014 08:50:23
//
// Last Modified By : Liuyh
// Last Modified On : 03-15-2014 14:50:00
// ***********************************************************************
// <copyright file="BaseCollectionViewModel.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;
using System.Collections.ObjectModel;
using OPCApp.Infrastructure.DataService;
using Microsoft.Practices.Prism.Mvvm;
using OPCApp.Infrastructure.Mvvm.View;
using System.Collections.Generic;

namespace OPCApp.Infrastructure.Mvvm
{

    /// <summary>
    /// Class BaseCollectionViewModel.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class BaseListViewModel<T> : BindableBase, IViewModel where T : new()
    {
        /// <summary>
        /// The view key
        /// </summary>
        private string ViewKey = "";

        /// <summary>
        /// The _ collection
        /// </summary>
        private  ObservableCollection<T> _Collection;
        /// <summary>
        /// Initializes a new instance of the <see cref="BaseListViewModel{T}"/> class.
        /// </summary>
        /// <param name="viewKey">对应的view 导出时的键值</param>
        protected BaseListViewModel(string viewKey)
        {
           
            this.AddCommand=new DelegateCommand(this.AddAction);
            this.SearchCommand = new DelegateCommand(this.SearchAction);
            this.EditCommand = new DelegateCommand<T>(this.EditAction);
            this.DeleteCommand=new DelegateCommand<T>(this.DeleteAction);
            this.ViewKey = viewKey;
     
            this._Collection = new ObservableCollection<T>();
            this.Models= new ListCollectionView(_Collection);
            this.View.DataContext = this;
        }



        /// <summary>
        /// Searches the action.
        /// </summary>
        private  void SearchAction()
        {
            var service = GetDataService();
            var c=service.Search(this.GetFilter());
            foreach (var item in c.Result)
            {
                _Collection.Add(item);
            }
        }

        protected virtual IDictionary<string,object> GetFilter()
        {
            return null;
        }

        /// <summary>
        /// Gets the data service.
        /// </summary>
        /// <returns>IBaseDataService{`0}.</returns>
        protected abstract IBaseDataService<T> GetDataService();

        #region Commands

        /// <summary>
        /// 添加
        /// </summary>
        /// <value>The add command.</value>
        public ICommand AddCommand { get; set; }


        /// <summary>
        /// 修改
        /// </summary>
        /// <value>The edit command.</value>
        public ICommand EditCommand { get; set; }

        /// <summary>
        ///删除
        /// </summary>
        /// <value>The delete command.</value>
        public ICommand DeleteCommand { get; set; }
        /// <summary>
        /// Gets or sets the search command.
        /// </summary>
        /// <value>The search command.</value>
        public ICommand SearchCommand { get; set; }

        #endregion

        /// <summary>
        /// Gets or sets the edit view mode key.
        /// </summary>
        /// <value>The edit view mode key.</value>
        protected string EditViewModeKey { get; set; }

        /// <summary>
        /// Gets or sets the add view mode key.
        /// </summary>
        /// <value>The add view mode key.</value>
        protected string AddViewModeKey { get; set; }

        #region Methods

        #region Action
        /// <summary>
        /// Adds the action.
        /// </summary>
        private void AddAction()
        {
            var w = AppEx.Container.GetInstance<IViewModel>(AddViewModeKey);
            if (w.View.ShowDialog() == true)
            {
                var service = GetDataService();
                var resultMsg= service.Add((T)w.Model);
                if (resultMsg.IsSuccess)
                {
                    this._Collection.Clear();
                    if (this.SearchCommand.CanExecute(null))
                    {
                        this.SearchCommand.Execute(null);
                    }
                }
                else
                {
                    MessageBox.Show("添加失败", "失败", MessageBoxButton.OK, MessageBoxImage.Error);
                }

            }
        }

        /// <summary>
        /// Edits the action.
        /// </summary>
        /// <param name="model">The model.</param>
        public void EditAction(T model)
        {
            var w = AppEx.Container.GetInstance<IViewModel>(EditViewModeKey);
            w.Model = model;
            if (w.View.ShowDialog() == true)
            {
                var service = GetDataService();
                var resultMsg = service.Edit((T) w.Model);
                if (resultMsg.IsSuccess)
                {
                    this._Collection.Clear();
                    if (this.SearchCommand.CanExecute(null))
                    {
                        this.SearchCommand.Execute(null);
                    }
                }
                else
                {
                    MessageBox.Show("修改失败", "失败", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        /// <summary>
        /// Deletes the action.
        /// </summary>
        /// <param name="model">The model.</param>
        public void DeleteAction(T model)
        {
            MessageBoxResult msg=  MessageBox.Show("确定要删除吗？", "删除", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (msg==MessageBoxResult.Yes)
            {
                var service= GetDataService();
                var r= service.Delete(model);
                if (r.IsSuccess)
                {
                    _Collection.Remove(model);
                }
                else
                {
                    MessageBox.Show("删除失败", "删除", MessageBoxButton.OK, MessageBoxImage.Error);

                }
            }
        }

        #endregion
        

        #endregion
       // public IFilterViewModel FilterViewModel { get; set; }

        /// <summary>
        /// Gets or sets the models.
        /// </summary>
        /// <value>The models.</value>
        public ICollectionView Models { get; set; }


        /// <summary>
        /// The _view
        /// </summary>
        private IBaseView _view;


        /// <summary>
        /// Gets or sets the view.
        /// </summary>
        /// <value>The view.</value>
        public IBaseView View
        {
            get
            {
                if (_view==null)
                {
                    _view= AppEx.Container.GetInstance<IBaseView>(ViewKey);
                }
                return _view;
            }
            set { _view = value; }
        }



        /// <summary>
        /// Gets or sets the model.
        /// </summary>
        /// <value>The model.</value>
        /// <exception cref="System.NotImplementedException">
        /// </exception>
        public object Model
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }
    }
}
