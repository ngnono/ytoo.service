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
       
       public BaseViewModel(IBaseView view)
       {
           View = view;
           
           this.OKCommand=new DelegateCommand(this.OkAction);
           this.CancelCommand=new DelegateCommand(this.CancelAction);
           view.DataContext = this;

       }

       private T _model;
       private void CancelAction()
       {
           doCancelAction();
           View.Cancel();
       }

       public DelegateCommand OKCommand
       {
           get; set; 
       }

       public DelegateCommand CancelCommand
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


        public T Model
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
