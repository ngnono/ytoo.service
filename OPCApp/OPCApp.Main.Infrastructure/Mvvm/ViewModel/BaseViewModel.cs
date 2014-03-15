using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MahApps.Metro.Controls;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Interfaces;
using Microsoft.Practices.Prism.Mvvm;
using OPCApp.Infrastructure.Mvvm.View;

namespace OPCApp.Infrastructure.Mvvm
{
    
   public abstract  class BaseViewModel<T>:BindableBase,IViewModel
   {
       
       public BaseViewModel(string viewKey)
       {
           
           this.OKCommand=new DelegateCommand(this.OkAction);
           this.CancelCommand=new DelegateCommand(this.CancelAction);

           View = GetView(viewKey);
           View.DataContext = this;

       }

       protected IBaseView GetView(string key)
       {
           return AppEx.Container.GetInstance<IBaseView>(key);
       }

       private object _model;
       private void CancelAction()
       {
           doCancelAction();
           View.Cancel();
       }

       public ICommand OKCommand
       {
           get; set; 
       }

       public ICommand CancelCommand
       {
           get; set;
       }

       private void OkAction()
       {
           BeforeDoOKAction();
           View.CloseView();
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


        public object Model
        {
            get { return _model; } 
            set { SetProperty(ref _model, value); } 
        }


        public IBaseView View
        {
            get;
            set;
        }
   }
}
