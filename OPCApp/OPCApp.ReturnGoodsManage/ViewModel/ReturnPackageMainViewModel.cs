using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OPCApp.Domain.Customer;
using System.ComponentModel.Composition;
using Microsoft.Practices.Prism.Mvvm;
using OPCApp.ReturnGoodsManage.ViewModels;
namespace OPCApp.ReturnGoodsManage.ViewModel
{
   [Export("ReturnPackageMainViewModel", typeof(ReturnPackageMainViewModel))]
    public  class ReturnPackageMainViewModel:BindableBase
   {
       private ReturnPackageManageViewModel _returnPackageManageViewModel;
       [Import]
       public ReturnPackageManageViewModel ReturnPackageManageViewModel
       {
           get { return _returnPackageManageViewModel; }
           set { SetProperty(ref _returnPackageManageViewModel, value); }
       }
       private ReturnPackageVerifyViewViewModel _customerPackageVerifyViewModel;
       [Import]
       public ReturnPackageVerifyViewViewModel ReturnPackageVerifyViewViewModel
       {
           get { return _customerPackageVerifyViewModel; }
           set { SetProperty(ref _customerPackageVerifyViewModel, value); }
       }
       private ReturnPackagePrintExpressViewModel _returnPackagePrintExpressViewModel;
       [Import]
       public ReturnPackagePrintExpressViewModel ReturnPackagePrintExpressViewModel
       {
           get { return _returnPackagePrintExpressViewModel; }
           set { SetProperty(ref _returnPackagePrintExpressViewModel, value); }
       }
       public ReturnPackageMainViewModel()
       {
       }

    
   }
}
