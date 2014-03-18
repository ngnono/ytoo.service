using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using OPCApp.Infrastructure.DataService;
using OPCApp.Infrastructure.Mvvm.View;
using System.ComponentModel.DataAnnotations;

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
           var errors = Validate((T) Model);
           if (errors.Count>0)
           {
               StringBuilder strB=new StringBuilder();
               foreach (var err in errors)
               {
                   strB.AppendLine(err.ErrorMessage);
               }
               
               MessageBox.Show(strB.ToString(), "错误", MessageBoxButton.OK, MessageBoxImage.Error);
               return;
           }
           var service = GetDataService();
           var msg= service.Add((T)Model);
           if (!msg.IsSuccess)
           {
               MessageBox.Show(msg.Msg, "错误", MessageBoxButton.OK, MessageBoxImage.Error);
               return;
           }
           View.CloseView();
           AfterDoOKAction();
       }

       protected abstract IBaseDataService<T> GetDataService();

       protected IList<ValidationResult> Validate(T entity)
       {
           var results = new List<ValidationResult>();
           var context = new ValidationContext(entity, null, null);
           Validator.TryValidateObject(entity, context, results);
           return results;
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
