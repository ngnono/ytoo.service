using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OPCApp.Domain.Customer;
namespace OPCApp.ReturnGoodsManage.ViewModel
{
   [Export("ReturnPackageManageViewModel", typeof(ReturnPackageManageViewModel))]
    public  class ReturnPackageManageViewModel
    {
       public PackageReceiveDto PackageReceiveDto { get; set; }

       public ReturnPackageManageViewModel()
       {
           PackageReceiveDto=new  PackageReceiveDto();
       }
    }
}
