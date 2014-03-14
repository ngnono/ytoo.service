using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;
using System.Collections.ObjectModel;
using OPCApp.Main.Infrastructure.DataService;
using Microsoft.Practices.Prism.Mvvm;
using OPCApp.Main.Infrastructure.Mvvm.View;

namespace OPCApp.Infrastructure.Mvvm
{
    public abstract class ViewModel4Grid<T> :BindableBase where T:new()
    {
        protected ViewModel4Grid()
        {
            this.Models = new ObservableCollection<T>();
            this.AddCommand=new DelegateCommand(this.AddAction);
            this.SearchCommand = new DelegateCommand(this.SearchAction);
        }

        private void SearchAction()
        {
            var service = GetDataService();
            var c=service.Search(null);
            foreach (var item in c.Result)
            {
                Models.Add(item);
            }
        }

        protected abstract IBaseDataService<T> GetDataService();
        protected abstract IViewModel<T> GetViewModel();
        #region Commands

        /// <summary>
        /// 添加
        /// </summary>
        /// <value>The add command.</value>
        public DelegateCommand AddCommand { get; set; }


        public DelegateCommand EditCommand { get; set; }

        public DelegateCommand DeleteCommand { get; set; }
        public DelegateCommand SearchCommand { get; set; }

        #endregion

        #region Methods

        private void AddAction()
        {
            var w = this.GetViewModel();
            if (w.View.ShowDialog()==true)
	        {

                this.Models.Add(w.Model);
	        }  
            //var t= DoAddAction();
           
        }

        #endregion
       // public IFilterViewModel FilterViewModel { get; set; }

       public ObservableCollection<T> Models { get; set; }
        //private List<T> _Models;
        ///*用户列表*/
        //public List<T> Models
        //{
        //    get { return this._Models; }
        //    set { SetProperty(ref this._Models, value); }
        //}
        public T SelectModel{get;set;}
        protected abstract T DoAddAction();

        protected abstract void DoEditAction(T t);

    }
}
