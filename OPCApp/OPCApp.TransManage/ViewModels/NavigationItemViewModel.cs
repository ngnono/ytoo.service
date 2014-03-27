using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;

namespace OPCApp.TransManage.ViewModels
{
    public class NavigationItemViewModel : BindableBase
    {
        public DelegateCommand PrintInvoiceCommand { get; set; }
        public DelegateCommand StoreInCommand { get; set; }
        public DelegateCommand StoreOutCommand { get; set; }

        //public void OnSubmit() 
        //{

        //}
        //public void OnReset() 
    }
}