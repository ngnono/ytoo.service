using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;
using OPCAPP.Common.Mvvm;
using System.Collections.ObjectModel;

namespace OPCApp.Infrastructure.Mvvm
{
    public abstract class ViewModel4Grid<T>:IBaseViewModel
    {
        protected ViewModel4Grid()
        {
            //this.AddCommand=new DelegateCommand<T>(this.AddAction();
        }

        #region Commands

        /// <summary>
        /// 添加
        /// </summary>
        /// <value>The add command.</value>
        public DelegateCommand<T> AddCommand { get; set; }


        public DelegateCommand<T> EditCommand { get; set; }

        public ICommand DeleteCommand { get; set; }

        #endregion

        #region Methods

        private void AddAction(T t)
        {
            //var t= DoAddAction();
            this.Models.Add(t);
        }

        #endregion
        public IFilterViewModel FilterViewModel { get; set; }

        public ObservableCollection<T> Models { get; set; }

        protected abstract T DoAddAction();

        protected abstract void DoEditAction(T t);

    }
}
