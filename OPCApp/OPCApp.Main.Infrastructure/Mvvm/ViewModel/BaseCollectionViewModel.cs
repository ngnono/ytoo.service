using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using Microsoft.Expression.Interactivity.Layout;
using Microsoft.Practices.Prism.Commands;
using System.Collections.ObjectModel;
using OPCApp.Infrastructure.DataService;
using Microsoft.Practices.Prism.Mvvm;
using OPCApp.Infrastructure.Mvvm.View;

namespace OPCApp.Infrastructure.Mvvm
{

    public abstract class BaseCollectionViewModel<T> : BindableBase, IViewModel where T : new()
    {
        private string ViewKey = "";
 
        private  ObservableCollection<T> _Collection;
        protected BaseCollectionViewModel(string viewKey)
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

       

        private void SearchAction()
        {
            var service = GetDataService();
            var c=service.Search(null);
            foreach (var item in c.Result)
            {
                _Collection.Add(item);
            }
        }

        protected abstract IBaseDataService<T> GetDataService();

        #region Commands

        /// <summary>
        /// 添加
        /// </summary>
        /// <value>The add command.</value>
        public ICommand AddCommand { get; set; }


        public ICommand EditCommand { get; set; }

        public ICommand DeleteCommand { get; set; }
        public ICommand SearchCommand { get; set; }

        #endregion

        protected string EditViewModeKey { get; set; }

        protected string AddViewModeKey { get; set; }

        #region Methods

        #region Action
        private void AddAction()
        {
            var w = AppEx.Container.GetInstance<IViewModel>(AddViewModeKey);
            if (w.View.ShowDialog() == true)
            {

                this._Collection.Add((T)w.Model);
            }
        }

        public void EditAction(T model)
        {
            var w = AppEx.Container.GetInstance<IViewModel>(EditViewModeKey);
            w.Model = model;
            if (w.View.ShowDialog() == true)
            {
            }
        }
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

        public ICollectionView Models { get; set; }


        private IBaseView _view;


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
