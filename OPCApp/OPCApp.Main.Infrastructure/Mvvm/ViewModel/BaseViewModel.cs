using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MahApps.Metro.Controls;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Interfaces;
using Microsoft.Practices.Prism.Mvvm;
using OPCApp.Main.Infrastructure.Mvvm.View;

namespace OPCApp.Infrastructure.Mvvm
{
   public abstract  class BaseViewModel<T>:BindableBase,IViewModel<T>
   {
       private IBaseView _view;
       public BaseViewModel(IBaseView view)
       {
           _view = view;
           this.OKCommand=new DelegateCommand(this.OkAction);
           this.CancelCommand=new DelegateCommand(this.CancelAction);
           view.DataContext = this;
       }

       private T _model;
       private void CancelAction()
       {
           doCancelAction();
           _view.Cancel();
       }

       public System.Windows.Input.ICommand OKCommand
       {
           get; set; 
       }

       public System.Windows.Input.ICommand CancelCommand
       {
           get; set;
       }

       private void OkAction()
       {
           BeforeDoOKAction();
           _view.Close();
           AfterDoOKAction();
       }

       

       protected virtual bool BeforeDoOKAction()
       {
           return true;
       }
       protected virtual bool AfterDoOKAction()
       {
           return true;
       }

       protected virtual bool doCancelAction()
       {
           return true;
       }


        public T Model
        {
            get { return _model; } 
            set { SetProperty(ref _model, value); } 
        }
    }
}
