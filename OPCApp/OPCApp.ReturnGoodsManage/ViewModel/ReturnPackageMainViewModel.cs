using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OPCApp.Domain.Customer;
namespace OPCApp.ReturnGoodsManage.ViewModel
{
   [Export("ReturnPackageMainViewModel", typeof(ReturnPackageMainViewModel))]
    public  class ReturnPackageMainViewModel
   {
       [Import] public ReturnPackageManageViewModel ReturnPackageManageViewModel;

       public ReturnPackageMainViewModel()
       {
               
       }
    }
}
